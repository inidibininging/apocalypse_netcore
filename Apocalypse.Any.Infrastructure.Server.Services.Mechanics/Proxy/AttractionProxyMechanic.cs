using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy
{
    public class AttractionProxyMechanic<TFullPositionHolder>
        : ISingleFullPositionHolderMechanic<TFullPositionHolder>
        where TFullPositionHolder : IFullPositionHolder
    {
        public bool Active { get; set; } = true;
        protected IAttractionMechanic AttractionMechanics { get; private set; }
        protected Func<TFullPositionHolder,IFullPositionHolder> GetTarget { get; private set; }

        protected Func<TFullPositionHolder,float> GetForce { get; private set; }
        
        public AttractionProxyMechanic(
            IAttractionMechanic attractionMechanics, 
            Func<TFullPositionHolder,IFullPositionHolder> getTarget,
            Func<TFullPositionHolder,float> getForce)
        {
            AttractionMechanics = attractionMechanics ?? throw new ArgumentNullException(nameof(attractionMechanics));
            GetTarget = getTarget ?? throw new ArgumentNullException(nameof(getTarget));
            GetForce = getForce ?? throw new ArgumentNullException(nameof(getForce));
        }

        public virtual TFullPositionHolder Update(TFullPositionHolder entity)
        {
            var target =  GetTarget(entity);
            var force = GetForce(entity);
            if(target is null)
                return entity;
            AttractionMechanics.Update(entity,target,force);
            return entity;
        }
    }
}