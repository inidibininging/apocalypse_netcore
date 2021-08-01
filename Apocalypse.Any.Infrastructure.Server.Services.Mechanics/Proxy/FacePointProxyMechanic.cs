using System;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Mechanics;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy
{
    public class FacePointProxyMechanic :
        ISingleFullPositionHolderMechanic<IFullPositionHolder>
    {
        public bool Active { get; set; } = true;
        protected FacePointMechanic FacePointMechanics { get; private set; }
        protected Func<IFullPositionHolder,IFullPositionHolder> GetTarget { get; private set; }

        public FacePointProxyMechanic(FacePointMechanic facePointMechanic, Func<IFullPositionHolder,IFullPositionHolder> getTarget)
        {
            FacePointMechanics = facePointMechanic ?? throw new ArgumentNullException(nameof(facePointMechanic));
            GetTarget = getTarget ?? throw new ArgumentNullException(nameof(getTarget));
        }

        public virtual IFullPositionHolder Update(IFullPositionHolder entity)
        {
            var target = GetTarget(entity);
            if(target == null)
                return entity;

            FacePointMechanics.Update(entity, target);
            return entity;
        }
    }
}