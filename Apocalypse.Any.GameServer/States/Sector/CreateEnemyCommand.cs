using Apocalypse.Any.Core;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;

using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using States.Core.Infrastructure.Services;
using System;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class CreateEnemyCommand : ICommand<IGameSectorLayerService>, IState<string, IGameSectorLayerService>
    {
        public RandomEnemySpaceshipFactory EnemySpaceshipFactory { get; set; }
        public ICharacterNameGenerator<EnemySpaceship> EnemyNameGenerator { get; set; }
        public string BaseEnemyName { get; set; } = "LowEnemy";

        public bool CanExecute(IGameSectorLayerService parameters)
        {
            if (parameters.CurrentStatus != GameSectorStatus.Running)
            {
                parameters.Messages.Add("Game sector is not running");
                return false;
            }

            if (parameters.DataLayer.Enemies.Count + 1 > parameters.MaxEnemies)
            {
                parameters.Messages.Add($"Max enemies count reached: {parameters.MaxEnemies}");
                return false;
            }

            if (!parameters.Factories.EnemyFactory.ContainsKey(nameof(RandomEnemySpaceshipFactory)))
            {
                parameters.Messages.Add($"An enemy factory named {nameof(RandomEnemySpaceshipFactory)} is needed to create enemies");
                return false;
            }

            return true;
        }

        public CreateEnemyCommand(ICharacterNameGenerator<EnemySpaceship> enemyNameGenerator)
        {
            EnemyNameGenerator = enemyNameGenerator ?? throw new ArgumentNullException(nameof(enemyNameGenerator));
        }

        public void Execute(IGameSectorLayerService parameters)
        {
            if (!CanExecute(parameters))
                return;

            var enemy = parameters.Factories.EnemyFactory[nameof(RandomEnemySpaceshipFactory)].Create(BaseEnemyName);

            if (enemy == null)
            {
                parameters.Messages.Add("Enemy could not be created");
                return;
            }

            enemy.Name = EnemyNameGenerator.Generate(enemy);
            enemy.Tags.Add("GeneratedEnemies");
            parameters.DataLayer.Enemies.Add(enemy);
            parameters.Messages.Add($"enemy {enemy.Name} created");
        }

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            Execute(machine.SharedContext);
        }
    }
}