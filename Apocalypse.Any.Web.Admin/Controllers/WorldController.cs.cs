using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apocalypse.Any.Web.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Apocalypse.Any.Web.Admin.Controllers
{
    public class WorldController : Controller
    {
        //TODO: pack this into a service with the connection ???
        public RedisSimpleConnectionConfiguration Config { get; set; } = new RedisSimpleConnectionConfiguration()
        {
            Host = "localhost",
            Port = 6379
        };
        public IActionResult Index()
        {
            return RedirectToAction(nameof(CLI));
        }
        public IActionResult CLI()
        {
            CLIViewModel model = new CLIViewModel();
            using (var conn = ConnectionMultiplexer.Connect($"{Config.Host}:{Config.Port}"))
            {
                var cli = conn.GetDatabase().StringGet("World.CLI");
                if(cli.HasValue)
                    model.Messages = JsonConvert.DeserializeObject<List<string>>(cli);
                else
                    model.Messages = new List<string>();
            }
            return View(nameof(CLI),model);
        }

        [HttpPost]
        public IActionResult CLIPass(CLIViewModel model)
        {
            if(string.IsNullOrEmpty(model.Expression))
                return RedirectToAction(nameof(CLI));
            using (var conn = ConnectionMultiplexer.Connect($"{Config.Host}:{Config.Port}"))
            {
                var cli = conn.GetDatabase().StringGet("World.CLI");
                if(cli.HasValue)
                {
                    var cliList = JsonConvert.DeserializeObject<List<string>>(cli);
                    cliList.Add(model.Expression);
                    conn.GetDatabase().StringSet("World.CLI",JsonConvert.SerializeObject(cliList));
                }
                else
                {
                    conn.GetDatabase().StringSet("World.CLI",JsonConvert.SerializeObject(new List<string>() { model.Expression }));
                }
            }
            return RedirectToAction(nameof(CLI));
        }
    }
}
