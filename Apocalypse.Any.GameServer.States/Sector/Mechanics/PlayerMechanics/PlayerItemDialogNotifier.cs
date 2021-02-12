using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.GameServer.Domain;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Common;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    public class PlayerItemDialogNotifier : IIdentifiableNotifiableModel
    {       
        private Func<IGameSectorLayerService> GetGameSector { get; set; }
        public string EventName { get; }
        public string Id { get => EventName; set => throw new InvalidOperationException(); }
        public string DestinationLayerNameOfItem { get; }

        public PlayerItemDialogNotifier(string eventName, string destinationLayerNameOfItem, Func<IGameSectorLayerService> getGameSector)
        {
            EventName = !string.IsNullOrEmpty(eventName) ? eventName : throw new ArgumentNullException(nameof(eventName));            
            DestinationLayerNameOfItem = !string.IsNullOrEmpty(destinationLayerNameOfItem) ? destinationLayerNameOfItem : throw new ArgumentNullException(nameof(destinationLayerNameOfItem));
            GetGameSector = getGameSector ?? throw new ArgumentNullException(nameof(getGameSector));
        }

        
        public void Notify(string topic, EventQueueArgument content)
        {
            var gameSectorLayerService = GetGameSector();            
            if (gameSectorLayerService == null)
                return;

            if (topic != EventName)
            {
                return;
            }

            foreach (var relation in gameSectorLayerService
                                        .DataLayer
                                        .Layers
                                        .SelectMany(l => l.DataAsEnumerable<DynamicRelation>())
                                        .Where(dr => dr.Id == content.ReferenceId))
            {
                Item item = null;
                DialogNode dialog = null;
                Player player = null;

                if(relation.Entity1 == typeof(Item) &&
                    relation.Entity2 == typeof(DialogNode))
                {
                    item = gameSectorLayerService
                                    .DataLayer
                                    .Items
                                    .FirstOrDefault(i => i.Id == relation.Entity1Id);
                    if (item == null)
                    {
                        //what happens if there is no item?
                        throw new InvalidOperationException($"{nameof(PlayerItemDialogNotifier)} cannot notify. Item is missing");
                    }

                    dialog = gameSectorLayerService
                                    .DataLayer
                                    .GetLayersByType<DialogNode>()
                                    .SelectMany(l => l.DataAsEnumerable<DialogNode>())
                                    .FirstOrDefault(d => d.Id == relation.Entity2Id);
                    if (dialog == null)
                    {
                        //what happens if there is no dialog?
                        throw new InvalidOperationException($"{nameof(PlayerItemDialogNotifier)} cannot notify. Dialog is missing");
                    }

                    player = gameSectorLayerService
                                    .DataLayer
                                    .Players
                                    .FirstOrDefault(p => p.Id == item.OwnerName);
                    if (player == null)
                    {
                        //what happens if there is no player?
                        throw new InvalidOperationException($"{nameof(PlayerItemDialogNotifier)} cannot notify. Player is missing");
                    }


                    //disown item from player                    
                    item.OwnerName = null;
                    item.InInventory = false;

                    gameSectorLayerService
                        .PlayerDialogService
                        .SwitchDialogNodeByLoginToken(player.LoginToken, null);

                }

                if (relation.Entity2 == typeof(Item) &&
                    relation.Entity1 == typeof(DialogNode))
                {
                    item = gameSectorLayerService
                                    .DataLayer
                                    .Items
                                    .FirstOrDefault(i => i.Id == relation.Entity2Id);
                    if (item == null)
                    {
                        //what happens if there is no item?
                        throw new InvalidOperationException($"{nameof(PlayerItemDialogNotifier)} cannot notify. Item is missing");
                    }

                    dialog = gameSectorLayerService
                                    .DataLayer
                                    .GetLayersByType<DialogNode>()
                                    .SelectMany(l => l.DataAsEnumerable<DialogNode>())
                                    .FirstOrDefault(d => d.Id == relation.Entity1Id);
                    if (dialog == null)
                    {
                        //what happens if there is no dialog?
                        throw new InvalidOperationException($"{nameof(PlayerItemDialogNotifier)} cannot notify. Dialog is missing");
                    }

                    player = gameSectorLayerService
                                    .DataLayer
                                    .Players
                                    .FirstOrDefault(p => p.Id == item.OwnerName);
                    if (player == null)
                    {
                        //what happens if there is no player?
                        throw new InvalidOperationException($"{nameof(PlayerItemDialogNotifier)} cannot notify. Player is missing");
                    }

                    //disown item from player
                    item.OwnerName = null;
                    item.InInventory = false;

                    gameSectorLayerService
                        .PlayerDialogService
                        .SwitchDialogNodeByLoginToken(player.LoginToken, null);


                }

                /* 
                * create a money dependency on the player, based on the item specs (factory)
                * todo: needs an implementation of recurring jobs
                */
                //transfer item to ... "the bank"
                //get the players bank
                var playerBank = gameSectorLayerService
                                    .DataLayer
                                    .Layers
                                    .Where(l => l.DisplayName == DestinationLayerNameOfItem && l.GetValidTypes().Any(t => t == typeof(IntBank)))
                                    .SelectMany(l => l.DataAsEnumerable<IntBank>())
                                    .FirstOrDefault(b => b.OwnerName == player.LoginToken);

                if (playerBank == null)
                {
                    //what happens if there is no player bank
                    throw new InvalidOperationException($"{nameof(PlayerItemDialogNotifier)} cannot notify. Player bank of {player.DisplayName} doesn't exist");
                }


                //get the player bank relations
                var playerBankRelationLayers = gameSectorLayerService
                                                .DataLayer
                                                .Layers
                                                .OfType<DynamicRelationLayer<Item, IntBank>>()
                                                .AsEnumerable<IGenericTypeDataLayer>()
                                                .Union(gameSectorLayerService
                                                .DataLayer
                                                .Layers
                                                .OfType<DynamicRelationLayer<IntBank, Item>>());

                if (playerBankRelationLayers.Any())
                {
                    /* if a relation between the player's bank and an item exists, 
                     * cool, lets update it 
                     * else, create one
                     * 
                     * one problem here can be if the item is transferred to another players bank
                     */
                    var playerBankItemRelation = playerBankRelationLayers
                                                        .SelectMany(l => l.DataAsEnumerable<DynamicRelation>())
                                                        .Where(dr => (dr.Entity1 == typeof(Item) &&
                                                                              dr.Entity1Id == item.Id &&
                                                                              dr.Entity2 == typeof(IntBank)) ||
                                                                              (dr.Entity1 == typeof(IntBank) &&
                                                                              dr.Entity2Id == item.Id &&
                                                                              dr.Entity2 == typeof(Item)));
                    
                    if (!playerBankItemRelation.Any())
                    {
                        foreach(var playerBankRelationLayer in playerBankRelationLayers)
                            playerBankRelationLayer.Add(new DynamicRelation()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Entity1 = typeof(Item),
                                Entity1Id = item.Id,
                                Entity2 = typeof(IntBank),
                                Entity2Id = playerBank.Id
                            });
                    }
                    else
                    {
                        //update what?
                    }
                }
                else
                {

                    
                    //playerBankRelationLayer.Add(new DynamicRelation()
                    //{
                    //    Id = Guid.NewGuid().ToString(),
                    //    Entity1 = typeof(Item),
                    //    Entity1Id = item.Id,
                    //    Entity2 = typeof(IntBank),
                    //    Entity2Id = playerBank.Id
                    //});
                }
            }
                            
        }
        
    }
}
