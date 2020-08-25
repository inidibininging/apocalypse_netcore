using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.EnemyMechanics
{
    public class RemoveProjectilesCounterOfDeadEnemiesState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            var livingEnemies = machine
                                .SharedContext
                                .DataLayer
                                .Enemies
                                .Where(e => e.Stats.Health > e.Stats.GetMinAttributeValue())
                                .Select(e => e.Id);

            foreach (var counterLayer in machine.SharedContext.DataLayer.GetLayersByType<IdentifiableShortCounterThreshold>())
                counterLayer.Remove<IdentifiableShortCounterThreshold>(c => !livingEnemies.Contains(c.Id));                                       
        }
    }
}
