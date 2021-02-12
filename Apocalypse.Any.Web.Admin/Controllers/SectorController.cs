using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Web.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Apocalypse.Any.Web.Admin.Controllers
{
    public class SectorController : Controller
    {
        //TODO: pack this into a service with the connection ???
        public RedisSimpleConnectionConfiguration Config { get; set; } = new RedisSimpleConnectionConfiguration()
        {
            Host = "localhost",
            Port = 6379
        };

        public IActionResult Index(string tag)
        {
            using (var conn = ConnectionMultiplexer.Connect($"{Config.Host}:{Config.Port}"))
            {
                var maxEnemies = conn.GetDatabase().StringGet($"{tag}.MaxEnemies");
                var maxPlayers = conn.GetDatabase().StringGet($"{tag}.MaxPlayers");
                var boundary = JsonConvert.DeserializeObject<SectorBoundary>(conn.GetDatabase().StringGet($"{tag}.SectorBoundaries"));
                Console.WriteLine(boundary);

                ViewData["Tag"] = tag;
                ViewData["MaxEnemies"] = maxEnemies;
                ViewData["MaxPlayers"] = maxPlayers;
                ViewData["SectorBoundary"] = boundary;
            }

            return View();
        }

        public IActionResult Enemies(string tag) => Entity<Enemy>(tag);
        public IActionResult Players(string tag) => Entity<Player>(tag);
        public IActionResult Items(string tag) => Entity<Item>(tag);
        public IActionResult GeneralCharacter(string tag) => Entity<CharacterEntity>(tag);

        protected IActionResult Entity<T>(string tag)
        where T : CharacterEntity
        {
            List<T> gameSectorLayerService;
            using (var conn = ConnectionMultiplexer.Connect($"{Config.Host}:{Config.Port}"))
            {
                var entity = conn.GetDatabase().StringGet($"{tag}.{Map.First(map => map.Singular == typeof(T).Name).Plural}");                
                gameSectorLayerService = JsonConvert.DeserializeObject<List<T>>(entity);
            }
            return View(Map.First(map => map.Singular == typeof(T).Name).Plural,
            new CharacterEntityListViewModel<T>(){
                Tag = tag,
                Entities = gameSectorLayerService
            });
        }

        private DataGrammarMap Map
        {
            get => new DataGrammarMap()
            {
                new DataSingularPluralModel(){ Singular = "Player", Plural = "Players" },
                new DataSingularPluralModel(){ Singular = "Enemy", Plural = "Enemies" },
                new DataSingularPluralModel(){ Singular = "Item", Plural = "Items" },
                new DataSingularPluralModel(){ Singular = "GeneralCharacter", Plural = "GeneralCharacter" },
            };
        }

    }
}

