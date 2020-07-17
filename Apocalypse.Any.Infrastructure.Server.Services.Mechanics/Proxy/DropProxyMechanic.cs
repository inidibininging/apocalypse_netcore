using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy
{
    public class DropProxyMechanic<TCharacterEntity>
        : CheckWithReflectionFactoryBase<Item>,
        ISingleMechanic<TCharacterEntity, CharacterEntity>
        where TCharacterEntity : CharacterEntity
    {
        public bool Active { get; set; } = true;
        public int Offset { get; set; }
        public IDropMechanic Drop { get; private set; }

        public DropProxyMechanic(IDropMechanic dropMechanic)
        {
            Drop = dropMechanic ?? throw new ArgumentNullException(nameof(dropMechanic));
        }

        public override bool CanUse<TParam>(TParam instance)
        {
            return CanUseByTType<TParam, CharacterEntity>();
        }

        protected override Item UseConverter<TParam>(TParam parameter)
        {
            Console.WriteLine("trying to pass");
            if (!CanUse(parameter))
                return null;
            Console.WriteLine("passed");
            var character = parameter as CharacterEntity;

            if(Offset == 0)
                throw new InvalidOperationException("Offset not set");
            return Drop.Update(character, Offset, Offset);
        }

        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(CharacterEntity) };
        }
    }
}