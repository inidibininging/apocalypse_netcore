using System;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class ScaleFixMechanic : ISingleFullPositionHolderMechanic<ImageData>
    {
        public bool Active { get; set; } = true;
        public ImageData Update(ImageData entity)
        {
	        if(entity == null)
		        return null;

	        //todo: do this once
	        entity.Width *= entity.Scale.X;
	        entity.Height *= entity.Scale.Y;
	        return entity;
        }
    }
}
