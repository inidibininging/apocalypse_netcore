using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.RoutingMechanics
{
    public class RouterPlayerShifterMechanic
        : ISingleUpdeatableGameSectorRouteMechanic
    {
        public bool Active { get; set; } = true;
        private IEnumerable<GameSectorRoutePair> GameSectorRoutes { get; set; }
        public IEnumerable<EntityGameSectorRoute> EntityGameSectorRoutes { get; set; } = new List<EntityGameSectorRoute>();
        // private RouteDualMediator RouteMediator { get; set; }

        public RouterPlayerShifterMechanic(
             // RouteDualMediator routeMediator,
             IEnumerable<GameSectorRoutePair> gameSectorRoutes)
        {
            // RouteMediator = routeMediator ?? throw new ArgumentNullException(nameof(routeMediator));
            GameSectorRoutes = gameSectorRoutes ?? throw new ArgumentNullException(nameof(gameSectorRoutes));
        }

        // public void SetRouteMediator(RouteDualMediator routeMediator)
        // {
        //     RouteMediator = null;
        //     RouteMediator = routeMediator;
        // }

        public IGameSectorsOwner Update(IGameSectorsOwner gameSectorOwner)
        {
            foreach (var sectorPair in GameSectorRoutes)
            {
                foreach (var entitySectorRoute in EntityGameSectorRoutes)
                {
                    if (entitySectorRoute.GameSectorTag != sectorPair.GameSectorTag) continue;
                    Console.WriteLine($"{nameof(RouterPlayerShifterMechanic)} TAG {entitySectorRoute.GameSectorTag} {entitySectorRoute.Position.X},{entitySectorRoute.Position.Y} ");
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

                    var gameStateData = gameSectorOfPlayer.Value.SharedContext.IODataLayer.GetGameStateByLoginToken(thePlayer.LoginToken);

                    //remove player from sector
                    if (!gameSectorOfPlayer
                        .Value
                        .SharedContext
                        .DataLayer
                        .Players
                        .TryTake(out thePlayer))
                        continue;

                    //mark sector as running if sector is not running ( should be put in a game sector owner mechanic )
                    if (gameSectorDestination.Value.SharedContext.CurrentStatus != Domain.Server.Model.Interfaces.GameSectorStatus.Running)
                        gameSectorDestination.Value.SharedContext.CurrentStatus = Domain.Server.Model.Interfaces.GameSectorStatus.Running;

                    const int offset = 512;
                    switch (entitySectorRoute.Trespassing)
                    {
                        case GameSectorTrespassingDirection.Up:
                            Console.WriteLine("UP");
                            Console.WriteLine($"FROM {thePlayer.CurrentImage.Position.Y} {gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorY - entitySectorRoute.Position.Y - offset}");
                            thePlayer.CurrentImage.Position.Y = gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorY - entitySectorRoute.Position.Y - offset;
                            break;
                        case GameSectorTrespassingDirection.Left:
                            Console.WriteLine("LEFT");
                            Console.WriteLine($"FROM {thePlayer.CurrentImage.Position.X} {gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorX - entitySectorRoute.Position.X + offset}");
                            thePlayer.CurrentImage.Position.X = gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorX - entitySectorRoute.Position.X + offset;
                            break;
                        case GameSectorTrespassingDirection.Right:
                            Console.WriteLine("RIGHT");
                            Console.WriteLine($"FROM {thePlayer.CurrentImage.Position.X} {gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorX - entitySectorRoute.Position.X + offset}");
                            thePlayer.CurrentImage.Position.X = gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorX - entitySectorRoute.Position.X + offset;
                            break;
                        case GameSectorTrespassingDirection.Down:
                            Console.WriteLine("DOWN");
                            Console.WriteLine($"FROM {thePlayer.CurrentImage.Position.Y} {gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorY - entitySectorRoute.Position.Y - offset}");
                            thePlayer.CurrentImage.Position.Y = gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorY - entitySectorRoute.Position.Y - offset;
                            break;
                    }
                    if(entitySectorRoute.Trespassing != GameSectorTrespassingDirection.None)
                        Console.WriteLine("trespassing");
                        
                    // thePlayer.CurrentImage.Rotation.Rotation -= 180; 
                        
                        
                    //gameSectorOfPlayer
                    //    .Value
                    //    .SharedContext
                    //    .DataLayer
                    //    .Players = new ConcurrentBag<PlayerSpaceship>(gameSectorOfPlayer
                    //                                                    .Value
                    //                                                    .SharedContext
                    //                                                    .DataLayer
                    //                                                    .Players.Where(p => p.Id == thePlayer.Id));

                    gameSectorDestination
                        .Value
                        .SharedContext
                        .DataLayer
                        .Players
                        .Add(thePlayer);
                    gameSectorDestination.Value.SharedContext.IODataLayer.RegisterGameStateData(thePlayer.LoginToken);
                    gameSectorDestination.Value.SharedContext.IODataLayer.ForwardServerDataToGame(gameStateData);
                        

                    entitySectorRoute.GameSectorTag = gameSectorDestination.Value.SharedContext.Tag;
                    // RouteMediator.Notify(this, entitySectorRoute);
                }
            }

            return gameSectorOwner;
        }
    }
}