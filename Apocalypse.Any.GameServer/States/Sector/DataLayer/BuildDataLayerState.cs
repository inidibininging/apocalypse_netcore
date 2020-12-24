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
using Apocalypse.Any.Domain.Common.Model.Language;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class BuildDataLayerState<TDataLayer> : IState<string, IGameSectorLayerService>
        where TDataLayer : IExpandedGameSectorDataLayer<PlayerSpaceship, EnemySpaceship, Item, Projectile, CharacterEntity, CharacterEntity, ImageData>
    {

        public BuildDataLayerState(string dialogLocationRelationLayerName,
                                    string dropPlayerItemEventName,
                                    string playerRegisteredEventName,
                                    string playerBankLayerName,
                                    Func<IGenericTypeFactory<IntBank>> bankFactory)
        {
            DialogLocationRelationLayerName = dialogLocationRelationLayerName ?? throw new ArgumentNullException(nameof(dialogLocationRelationLayerName));
            DropPlayerItemEventName = dropPlayerItemEventName ?? throw new ArgumentNullException(nameof(dropPlayerItemEventName));
            PlayerRegisteredEventName = playerRegisteredEventName ?? throw new ArgumentNullException(nameof(playerRegisteredEventName));
            PlayerBankLayerName = playerBankLayerName ?? throw new ArgumentNullException(nameof(playerBankLayerName));
            BankFactory = bankFactory ?? throw new ArgumentNullException(nameof(bankFactory));
        }

        public string DialogLocationRelationLayerName { get; }
        public string DropPlayerItemEventName { get; }
        public string PlayerRegisteredEventName { get; }
        public string PlayerBankLayerName { get; }
        public Func<IGenericTypeFactory<IntBank>> BankFactory { get; }

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

            //language related stuff
            machine.SharedContext.DataLayer.Layers.Add(new GenericInMemoryDataLayer<TagVariable>(null));
            machine.SharedContext.DataLayer.Layers.Add(new GenericInMemoryDataLayer<ReferenceVariable>(null));
            
            //dialog related stuff
            machine.SharedContext.DataLayer.Layers.Add(new GenericInMemoryDataLayer<IdentifiableCircularLocation>(null));
            machine.SharedContext.DataLayer.Layers.Add(new DynamicRelationLayer<IdentifiableCircularLocation, DialogNode>(DialogLocationRelationLayerName));
            machine.SharedContext.DataLayer.Layers.Add(new DynamicRelationLayer<Item, DialogNode>(DropPlayerItemEventName));
            machine.SharedContext.DataLayer.Layers.Add(new DynamicRelationLayer<PlayerSpaceship, IntBank>(DropPlayerItemEventName));

            //bank related stuff
            machine.SharedContext.DataLayer.Layers.Add(new GenericInMemoryDataLayer<IntBank>(PlayerBankLayerName));
            machine.SharedContext.DataLayer.Layers.Add(new DynamicRelationLayer<Item, IntBank>(DropPlayerItemEventName)); 

            //add event handlers
            var eventHandlers = new GenericInMemoryDataLayer<IIdentifiableNotifiableModel>(DropPlayerItemEventName);
            eventHandlers.Add(new PlayerItemDialogNotifier(DropPlayerItemEventName, PlayerBankLayerName, () => machine.SharedContext));
            eventHandlers.Add(new CreatePlayerBankOnPlayerRegistrationNotifier(PlayerRegisteredEventName,  PlayerBankLayerName, BankFactory, () => machine.SharedContext));
            
            machine.SharedContext.DataLayer.Layers.Add(eventHandlers);


            machine.SharedContext.DataLayer.Layers.Add(new EventQueue(DropPlayerItemEventName));
            machine.SharedContext.DataLayer.Layers.Add(new EventQueue(PlayerRegisteredEventName)); 



        }
    }
}
