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

        public ActionResult Index() {
            Console.WriteLine("Index()");
            ViewModelVM vm = new ViewModelVM
            {
                configurations = configurations
            };

            return View(vm);
        }

        public static void AddConfiguration(Configuration configuration)
        {
            if (configuration != null)
            {
                if (configurations.ContainsKey(configuration.link))
                {
                    configurations[configuration.link] = configuration.configuration;
                } else
                {
                    configurations.Add(configuration.link, configuration.configuration);
                }
            }
        }
    }

}