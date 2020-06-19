using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Bridge
{
    public class SingleCharacterEntityWithImageDataAdapter<TEntityWithImage> 
    : ISingleEntityWithImageMechanic<TEntityWithImage>
        where TEntityWithImage : IEntityWithImage
    {
        public ISingleFullPositionHolderMechanic<IFullPositionHolder> SingleFullPositionHolderMechanic { get; }

        public SingleCharacterEntityWithImageDataAdapter(ISingleFullPositionHolderMechanic<IFullPositionHolder> singleFullPositionHolderMechanic)
        {
            SingleFullPositionHolderMechanic = singleFullPositionHolderMechanic ?? throw new ArgumentNullException(nameof(singleFullPositionHolderMechanic));
        }

        public TEntityWithImage Update(TEntityWithImage singularEntity)
        {
            SingleFullPositionHolderMechanic.Update(singularEntity.CurrentImage);
            return singularEntity;
        }
    }
}