using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{

    /// <summary>
    /// This class triggers the "player selects an item in a dialog"-event, by writing 
    /// </summary>
    public class CreatePlayerSelectsItemDialogEventState : IState<string, IGameSectorLayerService>
    {
        public string PlayerSelectsDialogItemEventName { get; }

        public CreatePlayerSelectsItemDialogEventState(string playerSelectsDialogItemEventName)
        {
            PlayerSelectsDialogItemEventName = playerSelectsDialogItemEventName;
        }
        

        /// <summary>
        /// Creates events for clicked dialog events
        /// </summary>
        /// <param name="machine"></param>
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            
            var playerDialogs = machine
                                .SharedContext
                                .DataLayer
                                .Players
                                .Select(p => new Tuple<PlayerSpaceship, DialogNode>(p, machine.SharedContext.PlayerDialogService.GetDialogNodeByLoginToken(p.LoginToken)))
                                .Where(d => d != null);
            
            var itemDialogRelations = machine
                                 .SharedContext
                                 .DataLayer
                                 .GetLayersByName(PlayerSelectsDialogItemEventName)
                                 .SelectMany(l => l.DataAsEnumerable<DynamicRelation>())
                                 .Where(relation => (relation.Entity1 == typeof(Item) &&
                                                    relation.Entity2 == typeof(DialogNode)) ||
                                                    (relation.Entity2 == typeof(Item) &&
                                                    relation.Entity1 == typeof(DialogNode)));


            if (!itemDialogRelations.Any())
                return;

            foreach (var playerDialog in playerDialogs)
            {
                if (playerDialog.Item2 == null)
                    continue;

                foreach (var itemDialogRelation in itemDialogRelations)
                {
                    
                    if (itemDialogRelation.Entity1 == typeof(Item))
                    {                        
                        //check if (dialog id matches the player dialog id
                        if (itemDialogRelation.Entity2Id != playerDialog.Item2.Id)
                            continue;

                        var item = machine
                                    .SharedContext
                                    .DataLayer
                                    .Items
                                    .Where(i => i.Id == itemDialogRelation.Entity1Id)
                                    .FirstOrDefault();
                        
                        //item was removed.
                        if (item == null)
                            continue;

                        if (item.OwnerName != playerDialog.Item1.DisplayName)
                            continue;

                        //fire an event
                        foreach (var eventLayer in machine
                                                    .SharedContext
                                                    .DataLayer
                                                    .Layers
                                                    .Where(l => l.Name == PlayerSelectsDialogItemEventName))
                        {
                            eventLayer.Add(new EventQueueArgument()
                            {
                                DynamicRelationId = itemDialogRelation.Id,
                                EventName = PlayerSelectsDialogItemEventName,
                                Id = Guid.NewGuid().ToString()
                            });
                        }
                    }
                    if (itemDialogRelation.Entity2 == typeof(Item))
                    {
                        //check if (dialog id matches the player dialog id
                        if (itemDialogRelation.Entity1Id != playerDialog.Item2.Id)
                            continue;

                        var item = machine
                                    .SharedContext
                                    .DataLayer
                                    .Items
                                    .Where(i => i.Id == itemDialogRelation.Entity2Id)
                                    .FirstOrDefault();

                        //item was removed.
                        if (item == null)
                            continue;

                        if (item.OwnerName != playerDialog.Item1.DisplayName)
                            continue;

                        //fire an event
                        foreach (var eventLayer in machine
                                                    .SharedContext
                                                    .DataLayer
                                                    .Layers
                                                    .Where(l => l.Name == PlayerSelectsDialogItemEventName))
                        {
                            eventLayer.Add(new EventQueueArgument()
                            {
                                DynamicRelationId = itemDialogRelation.Id,
                                EventName = PlayerSelectsDialogItemEventName,
                                Id = Guid.NewGuid().ToString()
                            });
                        }
                    }
                }
            }
        }
    }
}
