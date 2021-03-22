using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Apocalypse.Any.Services.Authentication.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ExampleAuthController : ControllerBase
    {
        private readonly IUserAuthenticationService _authenticationService;
        private readonly ILogger<string> _logger;
        public ExampleAuthController(ILogger<string> logger, IUserAuthenticationService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public UserDataRole GetRoles(UserData userData)
        {
            var result = _authenticationService.GetRoles(userData);
            _logger.LogInformation($"{nameof(GetRoles)} {result.ToString()}");
            return result;
        } 
        

        [HttpPost]
        public string Register(UserData userData)         
        {
            var result = _authenticationService.Register(userData);
            _logger.LogInformation($"{nameof(GetRoles)} {result}");
            return result;
        } 

        [HttpPost]
        public string GetLoginByToken(UserData userData)         
        {
            var result = _authenticationService.GetLoginToken(userData);
            _logger.LogInformation($"{nameof(GetLoginByToken)} {result}");
            return result;
        } 
        
        [HttpPost]
        public UserDataWithLoginToken GetByLoginTokenHack(LoginTokenHolder loginToken)         
        {
            var result = _authenticationService.GetByLoginTokenHack(loginToken.LoginToken);
            _logger.LogInformation($"{nameof(GetLoginByToken)} {result}");
            return result;
        } 
        
        
    }
}