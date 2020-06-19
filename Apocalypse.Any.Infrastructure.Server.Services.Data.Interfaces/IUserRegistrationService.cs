using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IUserRegistrationService
    {
        string Register(UserData userData);
    }
}