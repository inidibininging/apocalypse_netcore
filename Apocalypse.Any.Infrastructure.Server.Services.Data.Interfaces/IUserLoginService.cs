using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IUserLoginService
    {
        string GetLoginToken(UserData userData);
    }
}