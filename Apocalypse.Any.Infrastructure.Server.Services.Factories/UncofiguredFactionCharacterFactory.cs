using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.Domain.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    /// <summary>
    /// Factory for creating unconfigured character entities
    /// </summary>
    /// <typeparam name="TCharacter"></typeparam>
    public class UncofiguredFactionCharacterFactory<TCharacter> : CheckWithReflectionFactoryBase<TCharacter>
        where TCharacter : CharacterEntity, new()
    {
        public override bool CanUse<TParam>(TParam instance) => CanUseByTType<TParam, string>();

        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(string) };
        }
        protected override TCharacter UseConverter<TParam>(TParam parameter)
            => new TCharacter()
            {
                Name = $"{typeof(TCharacter).Name}{ Guid.NewGuid()}",
                Tags = new List<string>() { parameter as string },
                CurrentImage = new ImageData(),
                IconImage = new ImageData(),
                Stats = new CharacterSheet()
            };
    }
}
