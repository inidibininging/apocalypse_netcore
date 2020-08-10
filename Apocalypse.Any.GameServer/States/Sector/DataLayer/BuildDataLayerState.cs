using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.GameServer.Domain;
using Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics;
using Apocalypse.Any.Infrastructure.Server.PubSub;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class BuildDataLayerState<TDataLayer> : IState<string, IGameSectorLayerService>
        where TDataLayer : IExpandedGameSectorDataLayer<PlayerSpaceship, EnemySpaceship, Item, Projectile, CharacterEntity, CharacterEntity, ImageData>
    {
        
        public BuildDataLayerState(IEnumerable<string> eventNames,
                                    string dialogLocationRelationLayerName)
        {
            EventNames = eventNames;
            DialogLocationRelationLayerName = dialogLocationRelationLayerName;

        }

        public IEnumerable<string> EventNames { get; }
        public string DialogLocationRelationLayerName { get; }

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (machine.SharedContext.DataLayer != null)
            {
                machine.SharedContext.DataLayer.Enemies.Clear();
                machine.SharedContext.DataLayer.Items.Clear();
                machine.SharedContext.DataLayer.Players.Clear();
                machine.SharedContext.DataLayer.PlayerItems.Clear();
                machine.SharedContext.DataLayer.Projectiles.Clear();
                machine.SharedContext.DataLayer.GeneralCharacter.Clear();
                machine.SharedContext.DataLayer.ImageData.Clear();                
                machine.SharedContext.DataLayer.Layers.Clear();
                machine.SharedContext.DataLayer = null;
            }

            machine.SharedContext.DataLayer = Activator.CreateInstance<TDataLayer>();
            machine.SharedContext.DataLayer.Layers.Add(new ExampleDialogService(new RandomPortraitGeneratorFactory()));
            machine.SharedContext.DataLayer.Layers.Add(new GenericInMemoryDataLayer<IdentifiableCircularLocation>(null));
            machine.SharedContext.DataLayer.Layers.Add(new GenericInMemoryDataLayer<IntBank>("The Bank"));
            machine.SharedContext.DataLayer.Layers.Add(new DynamicRelationLayer<IdentifiableCircularLocation, DialogNode>(DialogLocationRelationLayerName));

            machine.SharedContext.DataLayer.Layers.Add(new DynamicRelationLayer<Item, DialogNode>("DropPlayerItemDialogEvent"));
            machine.SharedContext.DataLayer.Layers.Add(new DynamicRelationLayer<Item, IntBank>("PlayerRegisteredEvent")); 
            var eventHandlers = new GenericInMemoryDataLayer<IIdentifiableNotifiableModel>("DropPlayerItemDialogEvent");
            eventHandlers.Add(new PlayerItemDialogNotifier("DropPlayerItemDialogEvent", "The Bank",() => machine.SharedContext));
            machine.SharedContext.DataLayer.Layers.Add(eventHandlers);
            
            foreach (var eventName in EventNames)
            {
                machine.SharedContext.DataLayer.Layers.Add(new EventQueue(eventName));
            }

            
        }
    }
}