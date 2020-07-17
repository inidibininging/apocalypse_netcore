using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.SectorMechanics;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy
{
    /// <summary>
    /// This behaviour prevents a game object from "escaping" the game screen
    /// </summary>
    public class NeverLeaveTheScreenProxyMechanic :
        ISingleFullPositionHolderMechanic<IFullPositionHolder>
    {
        public bool Active { get; set; } = true;
        private Func<IGameSectorBoundaries> GetSectorBoundaries { get; set; }
        private NeverLeaveTheSectorMechanic NeverLeaveTheScreenMechanics { get; set; }

        public NeverLeaveTheScreenProxyMechanic(
            NeverLeaveTheSectorMechanic neverLeaveTheScreenMechanic,
            Func<IGameSectorBoundaries> getSectorBoundaries
            )
        {
            if (neverLeaveTheScreenMechanic == null)
                throw new ArgumentNullException(nameof(neverLeaveTheScreenMechanic));
            NeverLeaveTheScreenMechanics = neverLeaveTheScreenMechanic;

            if (getSectorBoundaries == null)
                throw new ArgumentNullException(nameof(getSectorBoundaries));
            GetSectorBoundaries = getSectorBoundaries;
        }

        public IFullPositionHolder Update(IFullPositionHolder entity)
        {
            var sectorBoundaries = GetSectorBoundaries();
            //var screenConverted = new SectorBoundary() { MaxSectorX = screen.ScreenWidth, MaxSectorY = screen.ScreenHeight };
            NeverLeaveTheScreenMechanics.Update(entity, sectorBoundaries);
            return entity;
        }
    }
}