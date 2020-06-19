using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class CommandThresholdMechanic : ICommandThresholdMechanic
    {
        public IEnumerable<string> Update(
            GameStateData gameStateData,
            int repeatedCmdCount = 1,
            string thresholdCommand = DefaultKeys.Shoot)
        {
            if (gameStateData == null)
                return new List<string>();
            if (gameStateData.Commands == null)
                return new List<string>();
            if (!gameStateData.Commands.Any() || repeatedCmdCount < 1 || string.IsNullOrWhiteSpace(thresholdCommand))
                return gameStateData.Commands;

            var cmdListCached = gameStateData.Commands.ToList();
            var cachedCmd = string.Empty;
            var cachedCmdCounter = 0;
            return cmdListCached.Select(currentCmd =>
            {
                if (!string.IsNullOrWhiteSpace(cachedCmd))
                {
                    if (currentCmd == cachedCmd && currentCmd == thresholdCommand)
                    {
                        cachedCmdCounter += 1;
                    }
                    else
                    {
                        cachedCmdCounter = 0;
                    }
                }
                cachedCmd = currentCmd;
                return cachedCmdCounter > repeatedCmdCount ? string.Empty : currentCmd;
            }).Where(finalCmd => !string.IsNullOrWhiteSpace(finalCmd));
        }
    }
}