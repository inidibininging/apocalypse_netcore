using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    /// <summary>
    /// For now, a hard-coded variant of the enemy space ship factory.
    /// I made this because of the language feature (create, delete)
    /// </summary>
    public class UnconfiguredEnemyFactory : RandomEnemySpaceshipFactory
    {
        public override bool CanUse<TParam>(TParam instance) => CanUseByTType<TParam, string>();
        public override List<Type> GetValidParameterTypes() => new List<Type>() { typeof(string) };
        protected override EnemySpaceship UseConverter<TParam>(TParam parameter)
        {
            var enemy = base.UseConverter(Guid.NewGuid().ToString());
            enemy.Tags.Add(parameter as string);
            return enemy;
        }
    }
}
