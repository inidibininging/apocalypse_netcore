using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IPlayerDialogService
    {
        DialogNode GetDialogNodeByLoginToken(string loginToken);
        void SwitchDialogNodeByLoginToken(string loginToken, string id);        
    }
}