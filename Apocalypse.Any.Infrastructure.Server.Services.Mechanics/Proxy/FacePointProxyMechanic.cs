using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Proxy
{
    public class FacePointProxyMechanic :
        ISingleFullPositionHolderMechanic<IFullPositionHolder>
    {
        public bool Active { get; set; } = true;
        private FacePointMechanic FacePointMechanics { get; set; }
        private Func<IFullPositionHolder,IFullPositionHolder> GetTarget { get; set; }

        public FacePointProxyMechanic(FacePointMechanic facePointMechanic, Func<IFullPositionHolder,IFullPositionHolder> getTarget)
        {
            if (facePointMechanic == null)
                throw new ArgumentNullException(nameof(facePointMechanic));
            FacePointMechanics = facePointMechanic;

            if (getTarget == null)
                throw new ArgumentNullException(nameof(getTarget));
            GetTarget = getTarget;
        }

        public IFullPositionHolder Update(IFullPositionHolder entity)
        {
            var target = GetTarget(entity);
            if(target == null)
                return entity;

            FacePointMechanics.Update(entity, target);
            return entity;
        }
    }
}