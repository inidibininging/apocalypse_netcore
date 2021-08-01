using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Apocalypse.Any.Services.Language.ScriptProvider.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ScriptProviderController : ControllerBase
    {
        // Yeah, I know, hard coded path. Horrible. Will change if needed. AND ITS UNTESTED OMG OMG
        private const String FileToRead = "/home/develop/src/apocalypse_netcore/apocalypse.echse";

        private readonly ILogger<ScriptProviderController> _logger;

        public ScriptProviderController(ILogger<ScriptProviderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return !System.IO.File.Exists(FileToRead) ? String.Empty : System.IO.File.ReadAllText(FileToRead);
        }
    }
}