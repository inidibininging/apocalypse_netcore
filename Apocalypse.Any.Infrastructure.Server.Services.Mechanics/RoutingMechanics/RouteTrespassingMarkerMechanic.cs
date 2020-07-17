using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.RoutingMechanics
{
    /// <summary>
    /// Triggers
    /// </summary>
    public class RouteTrespassingMarkerMechanic
        : ISingleUpdeatableGameSectorRouteMechanic
    {
        public bool Active { get; set; } = true;
        public int Offset { get; private set; }

        public IEnumerable<EntityGameSectorRoute> EntityGameSectorRoutes { get; set; } = new List<EntityGameSectorRoute>();
        private RouteDualMediator RouteMediator { get; set; }

        public RouteTrespassingMarkerMechanic(
            RouteDualMediator routeMediator,
            int offset = 40)
        {
            RouteMediator = routeMediator ?? throw new ArgumentNullException(nameof(routeMediator));
            Offset = offset;
        }

        public void setRouteMediator(RouteDualMediator routeMediator)
        {
            RouteMediator = null;
            RouteMediator = routeMediator;
        }

        public IGameSectorsOwner Update(IGameSectorsOwner entity)
        {
            foreach (var sectorLayerService in entity.GameSectorLayerServices)
            {
                foreach (var player in sectorLayerService.Value.SharedContext.DataLayer.Players)
                {
                    var sectorRouteForPlayer = EntityGameSectorRoutes.FirstOrDefault(sectorRoute => sectorRoute.LoginToken == player.LoginToken);
                    //this is not my job
                    if (sectorRouteForPlayer == null)
                        sectorRouteForPlayer = new EntityGameSectorRoute()
                        {
                            LoginToken = player.LoginToken,
                            Trespassing = GameSectorTrespassingDirection.None,
                            GameSectorTag = sectorLayerService.Value.SharedContext.Tag
                        };
                    sectorRouteForPlayer.Position = new Vector2(player.CurrentImage.Position.X,
                                                                player.CurrentImage.Position.Y);

                    if (player.CurrentImage.Position.X >
                        sectorLayerService.Value.SharedContext.SectorBoundaries.MaxSectorX - Offset)
                    {
                        sectorRouteForPlayer.Trespassing |= GameSectorTrespassingDirection.Right;
                        sectorRouteForPlayer.Trespassing = sectorRouteForPlayer.Trespassing & (~GameSectorTrespassingDirection.Left);
                    }

                    if (player.CurrentImage.Position.X <
                        sectorLayerService.Value.SharedContext.SectorBoundaries.MinSectorX + Offset)
                    {
                        sectorRouteForPlayer.Trespassing |= GameSectorTrespassingDirection.Left;
                        sectorRouteForPlayer.Trespassing = sectorRouteForPlayer.Trespassing & (~GameSectorTrespassingDirection.Right);
                    }

                    if (player.CurrentImage.Position.Y >
                        sectorLayerService.Value.SharedContext.SectorBoundaries.MaxSectorY - Offset)
                    {
                        sectorRouteForPlayer.Trespassing |= GameSectorTrespassingDirection.Down;
                        sectorRouteForPlayer.Trespassing = sectorRouteForPlayer.Trespassing & (~GameSectorTrespassingDirection.Up);
                        
                    }

                    if (player.CurrentImage.Position.Y <
                        sectorLayerService.Value.SharedContext.SectorBoundaries.MinSectorY + Offset)
                    {
                        sectorRouteForPlayer.Trespassing |= GameSectorTrespassingDirection.Up;
                        sectorRouteForPlayer.Trespassing = sectorRouteForPlayer.Trespassing & (~GameSectorTrespassingDirection.Down);
                    }
                    if(sectorRouteForPlayer.Trespassing.HasFlag(GameSectorTrespassingDirection.Up) ||
                       sectorRouteForPlayer.Trespassing.HasFlag(GameSectorTrespassingDirection.Down) ||
                       sectorRouteForPlayer.Trespassing.HasFlag(GameSectorTrespassingDirection.Left) ||
                       sectorRouteForPlayer.Trespassing.HasFlag(GameSectorTrespassingDirection.Right))
                        RouteMediator.Notify(this, sectorRouteForPlayer);
                }
            }
            return entity;
        }
    }
}