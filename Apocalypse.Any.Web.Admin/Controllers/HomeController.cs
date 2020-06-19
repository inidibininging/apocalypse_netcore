using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Apocalypse.Any.Web.Admin.Models;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace Apocalypse.Any.Web.Admin.Controllers
{
    public class HomeController : Controller
    {
        public RedisSimpleConnectionConfiguration Config { get; set; } = new RedisSimpleConnectionConfiguration()
        {
            Host = "localhost",
            Port = 6379
        };


        public IActionResult Index()
        {
            using (var conn = ConnectionMultiplexer.Connect($"{Config.Host}:{Config.Port}"))
            {
                ViewData["World.Sectors"] = JsonConvert.DeserializeObject<List<string>>(conn.GetDatabase().StringGet("World.Sectors"));
                Console.WriteLine(ViewData["World.Sectors"]);
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        //public IActionResult Sector()
        //{
        //    ViewData["Message"] = "Lol";

        //    return View();
        //}
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
