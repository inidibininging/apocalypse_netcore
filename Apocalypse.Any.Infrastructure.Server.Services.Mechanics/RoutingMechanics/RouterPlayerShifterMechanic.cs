﻿using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.RoutingMechanics
{
    public class RouterPlayerShifterMechanic
        : ISingleUpdeatableGameSectorRouteMechanic
    {
        public bool Active { get; set; } = true;
        public IEnumerable<GameSectorRoutePair> GameSectorRoutes { get; private set; }
        public IEnumerable<EntityGameSectorRoute> EntityGameSectorRoutes { get; set; } = new List<EntityGameSectorRoute>();
        private RouteDualMediator RouteMediator { get; set; }

        public RouterPlayerShifterMechanic(
             RouteDualMediator routeMediator,
             IEnumerable<GameSectorRoutePair> gameSectorRoutes)
        {
            RouteMediator = routeMediator ?? throw new ArgumentNullException(nameof(routeMediator));
            GameSectorRoutes = gameSectorRoutes ?? throw new ArgumentNullException(nameof(gameSectorRoutes));
        }

        public void SetRouteMediator(RouteDualMediator routeMediator)
        {
            RouteMediator = null;
            RouteMediator = routeMediator;
        }

        public IGameSectorsOwner Update(IGameSectorsOwner gameSectorOwner)
        {
            foreach (var sectorPair in GameSectorRoutes)
            {
                foreach (var entitySectorRoute in EntityGameSectorRoutes)
                {
                    if (entitySectorRoute.GameSectorTag == sectorPair.GameSectorTag)
                    {
                        var gameSectorOfPlayer = gameSectorOwner.GameSectorLayerServices.FirstOrDefault(gs => gs.Value.SharedContext.Tag == entitySectorRoute.GameSectorTag);
                        if (gameSectorOfPlayer.Value == null)
                            continue;
                        var gameSectorDestination = gameSectorOwner.GameSectorLayerServices.FirstOrDefault(gs => gs.Value.SharedContext.Tag == sectorPair.GameSectorDestinationTag);
                        if (gameSectorDestination.Value == null)
                            continue;
                        var thePlayer =
                            gameSectorOfPlayer
                            .Value
                            .SharedContext
                            .DataLayer
                            .Players
                            .FirstOrDefault(player => player.LoginToken == entitySectorRoute.LoginToken);
                        if (!gameSectorOfPlayer
                            .Value
                            .SharedContext
                            .DataLayer
                            .Players
                            .TryTake(out thePlayer))
                        {
                            continue;
                        }

                        thePlayer.CurrentImage.Position.X = gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorX / 2;
                        thePlayer.CurrentImage.Position.Y = gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorY / 2;
                        gameSectorDestination
                            .Value
                            .SharedContext
                            .DataLayer
                            .Players
                            .Add(thePlayer);

                        entitySectorRoute.GameSectorTag = gameSectorDestination.Value.SharedContext.Tag;
                        RouteMediator.Notify(this, entitySectorRoute);
                    }
                }
            }

            return gameSectorOwner;
        }
    }
}