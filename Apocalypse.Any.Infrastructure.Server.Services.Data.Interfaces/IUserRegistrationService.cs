using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IUserRegistrationService
    {
        /// <summary>
        /// Registers a user and returns a login token
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>Login token as a string</returns>
        string Register(UserData userData);
    }
}