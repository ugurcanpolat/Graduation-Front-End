using front_end.Models;
using front_end.DAL;
using front_end.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;

namespace front_end.Controllers {
    public class HomeController : Controller {

        private static Dictionary<string, List<Data>> configurations = new Dictionary<string, List<Data>>();
        private static Dictionary<string, string> names = new Dictionary<string, string>();

        public ActionResult Index() {
            Console.WriteLine("Index()");
            ViewModelVM vm = new ViewModelVM
            {
                names = names,
                configurations = configurations
            };

            return View(vm);
        }

        public static void AddConfiguration(string name, Configuration configuration)
        {
            if (configuration != null)
            {
                if (configurations.ContainsKey(configuration.link))
                {
                    names[configuration.link] = name;
                    configurations[configuration.link] = configuration.configuration;
                } else
                {
                    names.Add(configuration.link, name);
                    configurations.Add(configuration.link, configuration.configuration);
                }
            }
        }
    }

}