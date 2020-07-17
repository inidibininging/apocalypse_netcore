using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Concurrent;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class BuildDataLayerState<TDataLayer> : IState<string, IGameSectorLayerService>
        where TDataLayer : IExpandedGameSectorDataLayer<PlayerSpaceship, EnemySpaceship, Item, Projectile, CharacterEntity, CharacterEntity, ImageData>
    {
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
            machine.SharedContext.DataLayer.Layers.Add(new GenericInMemoryDataLayer<IdentifiableCircularLocation>());
            machine.SharedContext.DataLayer.Layers.Add(new DynamicRelationLayer<IdentifiableCircularLocation, DialogNode>());
        }
    }
}