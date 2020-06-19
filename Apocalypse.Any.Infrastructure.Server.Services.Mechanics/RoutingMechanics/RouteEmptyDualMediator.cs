using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.RoutingMechanics
{
    public sealed class RouteEmptyDualMediator
        : DualMediator<
            RouteTrespassingMarkerMechanic,
            RouterPlayerShifterMechanic,
            ISingleUpdeatableGameSectorRouteMechanic,
            EntityGameSectorRoute
            >
    {
        public RouteEmptyDualMediator()
            : base(null, null, (s, e) => { }) { }
    }
}