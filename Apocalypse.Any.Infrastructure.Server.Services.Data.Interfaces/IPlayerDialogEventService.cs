using Apocalypse.Any.Domain.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IPlayerDialogEventService
    {
        void RegisterEvent(string dialogId, Action<DialogNode> callback);
        void UnregisterEvent(string dialogId);
    }
}
