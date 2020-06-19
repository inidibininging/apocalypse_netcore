using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Bridge
{
    //SPOT THE PATTERN.
    public class SingleCharacterEntityFullPositionHolderAdapterMechanic<TCharacterEntity> : ISingleCharacterEntityMechanic<TCharacterEntity>
        where TCharacterEntity : CharacterEntity, new()

    {
        public ISingleFullPositionHolderMechanic<IFullPositionHolder> SingleFullPositionHolderMechanic { get; private set; }

        public SingleCharacterEntityFullPositionHolderAdapterMechanic(ISingleFullPositionHolderMechanic<IFullPositionHolder> singleFullPositionHolderMechanic)
        {
            SingleFullPositionHolderMechanic = singleFullPositionHolderMechanic ?? throw new ArgumentNullException(nameof(singleFullPositionHolderMechanic));
        }

        public TCharacterEntity Update(TCharacterEntity singularEntity)
        {
            SingleFullPositionHolderMechanic.Update(singularEntity.CurrentImage);
            return singularEntity;
        }
    }
}