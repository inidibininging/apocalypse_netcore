using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy;
using States.Core.Infrastructure.Services;
using System;
using System.Linq;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.ItemMechanics
{
    public class DropItemsState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine.SharedContext
                 .DataLayer
                 .Enemies
                 .Where(enemy => enemy.Stats.Health <= enemy.Stats.GetMinAttributeValue())
                 .Select(enemy =>
            {
                //Console.WriteLine(nameof(DropItemsState));
                var item = machine
                       .SharedContext
                       .Factories
                       .ItemFactory[nameof(DropProxyMechanic<CharacterEntity>)]
                       .Create(enemy);
                return machine
                       .SharedContext
                       .Factories
                       .ItemFactory[nameof(DropProxyMechanic<CharacterEntity>)]
                       .Create(enemy);
            })
            .Where(item => item != null)
            .ToList()
            .ForEach(item => machine
                        .SharedContext
                        .DataLayer
                        .Items
                        .Add(item));
        }
    }
}