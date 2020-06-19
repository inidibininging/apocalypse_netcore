using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.ItemMechanics
{
    public class ConsumeItemExperienceState : IState<string, IGameSectorLayerService>
    {
        public int ItemBagSize { get; set; } = 36;
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            //consume all 36 items (the player slot size is 36) and push one level up
            var playerItems = machine.SharedContext.DataLayer.Items.GroupBy(plyr => plyr.OwnerName);
            var playerItemsToConsume = playerItems.Where(grp => grp.Count() > ItemBagSize);

            machine
                .SharedContext
                .DataLayer
                .Players
                .Select(plyr => new { Player = plyr, ExperienceGain = playerItemsToConsume.FirstOrDefault(grp => grp.Key == plyr.Name)?.Sum(itm => itm.Stats.Experience) })
                .ToList()
                .ForEach(playerItemsResult => playerItemsResult.Player.Stats.Experience += playerItemsResult.ExperienceGain.HasValue ? playerItemsResult.ExperienceGain.Value : 0);

            //remove consumed items
            machine
                .SharedContext
                .DataLayer
                .Items = new ConcurrentBag<Domain.Common.Model.Item>(
                    machine
                    .SharedContext
                    .DataLayer
                    .Items
                    .Except(playerItemsToConsume.SelectMany(grp => grp)));

        }
    }
}
