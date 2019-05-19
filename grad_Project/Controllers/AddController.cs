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
    public class AddController : Controller
    {
        private static string link = null;
        private static string api = null;
        private static string name = null;
        private static List<int> availableLocations = null;
        private static Configuration currentConfiguration = null;
        private static Configuration newConfiguration = null;

        public ActionResult Index()
        {
            link = null;
            api = null;
            name = null;
            availableLocations = null;
            currentConfiguration = null;
            newConfiguration = null;

            Console.WriteLine("Add: Index()");
            return View ();
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            Console.WriteLine("Add: Index(): FormCollection");

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
                        Console.WriteLine("Add: Index(): Modify data success.");
                    }
                    else
                    {
                        Console.WriteLine("Add: Index(): Modify data failed.");
                    }
                }

                response.Close();

                newConfiguration.link = link;

                HomeController.AddConfiguration(name, newConfiguration);

                return RedirectToRoute("Homepage");
            }

            if (link == null)
            {
                name = collection["application-name"];
                link = collection["link"];
                if (!link.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) &&
                    !link.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                {
                    link = "http://" + link;
                }

                Console.WriteLine("Link: {0}", link);
            }   

            ViewBag.Link = link;
            ViewBag.AppName = name;

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

                        for(int i = 0; i < currentConfiguration.configuration.Count;  i++)
                        {
                            currentConfiguration.configuration[i].readableName =
                                ConvertStringToReadable(currentConfiguration.configuration[i].name);
                        }

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

                var dataName = collection["data-name-select"];
                data.name = dataName;

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
                    if (entry.name == dataName)
                    {
                        data.text = entry.text;
                        data.labels = entry.labels;
                        break;
                    }
                }

                data.readableName = ConvertStringToReadable(dataName);

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

        private string ConvertStringToReadable(string dataName)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            foreach (char c in dataName)
            {
                if (char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }

            string readableText = builder.ToString();
            readableText = readableText.Substring(0, 1).ToUpper() + readableText.Substring(1);
            return readableText;
        }
    }
}