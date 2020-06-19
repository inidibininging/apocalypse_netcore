using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    public class ProcessInventoryRightState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine
                 .SharedContext
                 .DataLayer
                 .Players
                 .ToList()
                 .ForEach(player =>
                 {
                     //get selected item of player (just in case) / for now...
                     var previousSelectedItem = machine
                                                 .SharedContext
                                                 .DataLayer
                                                 .PlayerItems
                                                 .FirstOrDefault(pItem => pItem.OwnerName == player.Name && pItem.Selected);
                     machine
                    .SharedContext
                    .IODataLayer
                    .GetGameStateByLoginToken(player.LoginToken)?
                    .Commands.ForEach(cmd =>
                    {
                        if (cmd == DefaultKeys.InventoryRight
                            && machine
                                .SharedContext
                                .DataLayer
                                .PlayerItems.Count(pItem => pItem.OwnerName == player.Name) != 1)
                        {
                            //all these can generate a bug. what if there is only one item for the playeR?
                            previousSelectedItem.Selected = false;
                            var lastItem = machine
                                            .SharedContext
                                            .DataLayer
                                            .PlayerItems
                                                .Where(pItem => pItem.OwnerName == player.Name)
                                                .OrderByDescending(pItem => pItem.Order)
                                                .FirstOrDefault();

                            var nextItem = default(Item);

                            if (previousSelectedItem.Order >= 1)
                                nextItem = machine
                                            .SharedContext
                                            .DataLayer
                                            .PlayerItems
                                                .FirstOrDefault(pItem => pItem.OwnerName == player.Name &&
                                                                pItem.Order == previousSelectedItem.Order + 1);

                            if (previousSelectedItem == lastItem
                                && previousSelectedItem.Order > 1
                                && lastItem.Order > 1)
                                nextItem = machine
                                            .SharedContext
                                            .DataLayer
                                            .PlayerItems
                                                .Where(pItem => pItem.OwnerName == player.Name && pItem.Order == 1)
                                                .FirstOrDefault();
                            if (nextItem != null)
                                nextItem.Selected = true;
                        }
                    });
                 });
        }
    }
}