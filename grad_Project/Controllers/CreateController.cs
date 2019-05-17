using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using front_end.Models;
using Newtonsoft.Json;

namespace front_end.Controllers
{
    public class CreateController : Controller
    {
        private static string link = null;
        private static string api = null;
        private static List<int> availableLocations = null;
        private static Configuration currentConfiguration = null;
        private static Configuration newConfiguration = null;

        public ActionResult Index()
        {
            link = null;
            api = null;
            availableLocations = null;
            currentConfiguration = null;
            newConfiguration = null;

            Console.WriteLine("Create: Index()");
            return View ();
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            Console.WriteLine("Create: Index(): FormCollection");

            if (!string.IsNullOrEmpty(collection["submit"]))
            {
                WebRequest webRequest = WebRequest.Create("http://localhost:8080/modifyConfiguration/");
                webRequest.ContentType = "application/json";
                webRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(newConfiguration);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                WebResponse response = webRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    ResponseModel deserialized = JsonConvert.DeserializeObject<ResponseModel>(result);

                    if (deserialized.success)
                    {
                        Console.WriteLine("Create: Index(): Modify data success.");
                    }
                    else
                    {
                        Console.WriteLine("Create: Index(): Modify data failed.");
                    }
                }

                response.Close();

                newConfiguration.link = link;

                HomeController.AddConfiguration(newConfiguration);

                return RedirectToRoute("Homepage");
            }

            if (link == null)
            {
                link = collection["link"];
                if (!link.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) &&
                    !link.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                {
                    link = "http://" + link;
                }

                Console.WriteLine("Link: {0}", link);
            }   

            ViewBag.Link = link;

            if (currentConfiguration == null)
            {
                WebRequest webRequest = WebRequest.Create(link);
                webRequest.ContentType = "application/json";
                webRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    string json = "";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                WebResponse response = webRequest.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    ResponseModel deserialized = JsonConvert.DeserializeObject<ResponseModel>(result);

                    if (deserialized.success)
                    {
                        string[] splitString = link.Split('/');

                        api = splitString.Last().Any() ?
                            splitString.Last() : splitString[splitString.Count()-2];

                        api = "/" + api + "/";

                        currentConfiguration = new Configuration
                        {
                            link = api,
                            configuration = deserialized.data
                        };

                        if (newConfiguration == null)
                        {
                            newConfiguration = new Configuration
                            {
                                link = api,
                                configuration = new List<Data>()
                            };
                        }
                        availableLocations = new List<int>
                        {
                            1,
                            2,
                            3,
                            4
                        };

                        ViewBag.Current = currentConfiguration.configuration;
                        ViewBag.Configuration = newConfiguration.configuration;
                        ViewBag.AvailableLocations = availableLocations;
                    }
                }

                response.Close();
            }
            else
            {
                Data data = new Data();

                var name = collection["data-name-select"];
                data.name = name;

                var modifiable = collection["modifiable-select"];
                data.modifiable = modifiable == "1";

                int locationOption = int.Parse(collection["location-select"]);

                if (locationOption != 0)
                {   
                    availableLocations.Remove(locationOption);
                }

                data.screenLocation = locationOption;

                var typeOption = collection["type-select"];
                data.visual = typeOption;

                foreach(Data entry in currentConfiguration.configuration)
                {
                    if (entry.name == name)
                    {
                        data.text = entry.text;
                        data.labels = entry.labels;
                        break;
                    }
                }

                newConfiguration.configuration.Add(data);
                ViewBag.Configuration = newConfiguration.configuration;
                ViewBag.Current = currentConfiguration.configuration;
                ViewBag.AvailableLocations = availableLocations;
            }

            try {
                return View ();
            } catch {
                return Index ();
            }
        }

        public ActionResult GetChartData(DataSourceLoadOptions loadOptions)
        {
            Console.WriteLine("GetChartData()");

            List<ChartData> chartData = new List<ChartData>();

            foreach(Data data in currentConfiguration.configuration)
            {
                for (int i = 0;  i < data.values.Count; i++)
                {
                    chartData.Add(new ChartData
                    {
                        name = data.name,
                        value = data.values[i],
                        arg = i
                    });
                }
            }

            var result = DataSourceLoader.Load(chartData, loadOptions);
            var resultJson = JsonConvert.SerializeObject(result);
            return Content(resultJson, "application/json");
        }
    }
}