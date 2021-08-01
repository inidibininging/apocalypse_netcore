using System;
using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy
{
    public class FacePointTargetWithinDistanceRangeProxyMechanic : FacePointProxyMechanic
    {
        private int _minDistanceRange;
        public int MinDistanceRange
        {
            get => _minDistanceRange;
            set => _minDistanceRange = value >= MaxDistanceRange && MaxDistanceRange != 0?
                throw new ArgumentException($"{nameof(MinDistanceRange)} cannot exceed {nameof(MaxDistanceRange)}") :
                value;
        }
        
        private int _maxDistanceRange;

        public int MaxDistanceRange
        {
            get => _maxDistanceRange;
            set => _maxDistanceRange = value <= MinDistanceRange && MinDistanceRange != 0?
                throw new ArgumentException($"{nameof(MaxDistanceRange)} cannot exceed {nameof(MaxDistanceRange)}") :
                value;
        }
        public FacePointTargetWithinDistanceRangeProxyMechanic(FacePointMechanic facePointMechanic, Func<IFullPositionHolder, IFullPositionHolder> getTarget) : base(facePointMechanic, getTarget)
        {
        }

        public override IFullPositionHolder Update(IFullPositionHolder entity)
        {
            var target = GetTarget(entity);
            if (entity == null || target == null)
                return entity;
            var distanceBetweenSubjectAndTarget =
                Vector2.Distance(entity.Position.ToVector2(), target.Position.ToVector2());
            return distanceBetweenSubjectAndTarget > MinDistanceRange && 
                   distanceBetweenSubjectAndTarget < MaxDistanceRange ? base.Update(entity) : entity;
        }
    }
}