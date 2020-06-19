using System;
using System.Collections.Concurrent;
using System.Linq;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.EnemyMechanics
{
    public class RemoveDeadEnemiesMechanicsState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            var livingEnemies = (from enemy in machine.SharedContext.DataLayer.Enemies
                                where enemy.Stats.Health > 0
                                select enemy);
            if(machine.SharedContext.DataLayer.Enemies.Except(livingEnemies).Any())
                Console.WriteLine("DEAD ENEMIES FOUND");
            machine.SharedContext.DataLayer.Enemies = new ConcurrentBag<EnemySpaceship>(livingEnemies);
        }
    }
}
