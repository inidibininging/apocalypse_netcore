using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.RoutingMechanics
{
    public class RouteDualMediator
        : DualMediator<
            RouteTrespassingMarkerMechanic,
            RouterPlayerShifterMechanic,
            ISingleUpdeatableGameSectorRouteMechanic,
            EntityGameSectorRoute
            >
    {
        public RouteDualMediator(
            RouteTrespassingMarkerMechanic t1,
            RouterPlayerShifterMechanic t2) 
            : base(
                  t1,
                  t2,
                  (s, e) => 
            {
                if (s == t1)
                {
                    if (!t2.EntityGameSectorRoutes.Any(sectorRoute => sectorRoute.LoginToken == e.LoginToken))
                    {
                        t2.EntityGameSectorRoutes = t2.EntityGameSectorRoutes.Append(e).ToList();
                        t1.EntityGameSectorRoutes = t1.EntityGameSectorRoutes.Except(t1.EntityGameSectorRoutes.Where(esr => esr.LoginToken == e.LoginToken)).ToList();
                        return;
                    }
                }
                if (s == t2)
                {
                    if (!t1.EntityGameSectorRoutes.Any(es => es.LoginToken == e.LoginToken))
                    {
                        t2.EntityGameSectorRoutes = t2.EntityGameSectorRoutes.Except(t2.EntityGameSectorRoutes.Where(esr => esr.LoginToken == e.LoginToken)).ToList();
                        return;
                    }
                }
            })
        {

        }
    }
}
