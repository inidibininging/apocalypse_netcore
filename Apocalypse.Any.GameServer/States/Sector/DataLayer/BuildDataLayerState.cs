using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class BuildDataLayerState<TDataLayer> : IState<string, IGameSectorLayerService>
        where TDataLayer : IGameSectorDataLayer<PlayerSpaceship, EnemySpaceship, Item, Projectile, CharacterEntity, CharacterEntity, ImageData>
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
                machine.SharedContext.DataLayer = null;
            }

            machine.SharedContext.DataLayer = Activator.CreateInstance<TDataLayer>();
        }
    }
}