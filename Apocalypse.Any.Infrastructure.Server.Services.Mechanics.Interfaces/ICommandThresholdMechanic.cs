using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Domain.Common.Model.Network;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces
{
    public interface ICommandThresholdMechanic
    {
        IEnumerable<string> Update(GameStateData gameStateData, int repeatedCmdCount = 1, string thresholdCommand = DefaultKeys.Shoot);
    }
}