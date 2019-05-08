using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace front_end.Controllers
{
    public class ChartsController : Controller
    {
        public ActionResult Pie()
        {
            Console.WriteLine("Pie()");
            return View();
        }

        public ActionResult Line()
        {
            Console.WriteLine("Line()");
            return View();
        }

        public ActionResult Bar()
        {
            Console.WriteLine("Bar()");
            return View();
        }


    } 
}

