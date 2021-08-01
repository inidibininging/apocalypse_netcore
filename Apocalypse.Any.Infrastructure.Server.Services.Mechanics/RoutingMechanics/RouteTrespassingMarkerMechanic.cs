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

        /// <summary>
        /// Stores the player coordinates/login token and space where the player is trespassing and the 
        /// </summary>
        private List<EntityGameSectorRoute> EntityGameSectorRoutes { get; set; } = new List<EntityGameSectorRoute>();

        private List<GameSectorRoutePair> GameSectorRoutePairs { get; set; } = new List<GameSectorRoutePair>();
        
        public RouteTrespassingMarkerMechanic(
            int offset = 40)
        {
            Offset = offset;
        }
        

        public void RegisterRoutePair(GameSectorRoutePair gameSectorRoutePair)
        {
            GameSectorRoutePairs.Add(gameSectorRoutePair);
        }
        
        public IGameSectorsOwner Update(IGameSectorsOwner entity)
        {
            foreach (var sectorLayerService in entity.GameSectorLayerServices)
            {
                foreach (var player in sectorLayerService.Value.SharedContext.DataLayer.Players)
                {
                    var sectorRouteForPlayer = EntityGameSectorRoutes.FirstOrDefault(sectorRoute => sectorRoute.LoginToken == player.LoginToken);

                    //this is not my job (create the entity game sector route... maybe a factory?
                    if (sectorRouteForPlayer == null)
                    {
                        sectorRouteForPlayer = new EntityGameSectorRoute()
                        {
                            LoginToken = player.LoginToken,
                            Trespassing = GameSectorTrespassingDirection.None,
                            GameSectorTag = sectorLayerService.Value.SharedContext.Tag
                        };
                        //add player sector route info if not existing
                        EntityGameSectorRoutes.Add(sectorRouteForPlayer);
                    }
                    sectorRouteForPlayer.Position = new Vector2(player.CurrentImage.Position.X,
                                                                player.CurrentImage.Position.Y);
                    
                    if (player.CurrentImage.Position.X >=
                        sectorLayerService.Value.SharedContext.SectorBoundaries.MaxSectorX - Offset)
                    {
                        Console.WriteLine($"SECTOR {sectorRouteForPlayer.GameSectorTag} DETECTS TRESPASSING RIGHT");
                        sectorRouteForPlayer.Trespassing |= GameSectorTrespassingDirection.Right;
                        sectorRouteForPlayer.Trespassing = sectorRouteForPlayer.Trespassing & (~GameSectorTrespassingDirection.Left);
                        ShiftPlayers(sectorRouteForPlayer, entity);
                        continue;
                    }

                    if (player.CurrentImage.Position.X <=
                        sectorLayerService.Value.SharedContext.SectorBoundaries.MinSectorX + Offset)
                    {
                        Console.WriteLine($"SECTOR {sectorRouteForPlayer.GameSectorTag} DETECTS TRESPASSING LEFT");
                        sectorRouteForPlayer.Trespassing |= GameSectorTrespassingDirection.Left;
                        sectorRouteForPlayer.Trespassing = sectorRouteForPlayer.Trespassing & (~GameSectorTrespassingDirection.Right);
                        ShiftPlayers(sectorRouteForPlayer, entity);
                        continue;
                    }

                    if (player.CurrentImage.Position.Y >=
                        sectorLayerService.Value.SharedContext.SectorBoundaries.MaxSectorY - Offset)
                    {
                        Console.WriteLine($"SECTOR {sectorRouteForPlayer.GameSectorTag} DETECTS TRESPASSING DOWN");
                        sectorRouteForPlayer.Trespassing |= GameSectorTrespassingDirection.Down;
                        sectorRouteForPlayer.Trespassing = sectorRouteForPlayer.Trespassing & (~GameSectorTrespassingDirection.Up);
                        ShiftPlayers(sectorRouteForPlayer, entity);
                        continue;
                    }

                    if (player.CurrentImage.Position.Y <=
                        sectorLayerService.Value.SharedContext.SectorBoundaries.MinSectorY + Offset)
                    {
                        Console.WriteLine($"SECTOR {sectorRouteForPlayer.GameSectorTag} DETECTS TRESPASSING UP");
                        sectorRouteForPlayer.Trespassing |= GameSectorTrespassingDirection.Up;
                        sectorRouteForPlayer.Trespassing = sectorRouteForPlayer.Trespassing & (~GameSectorTrespassingDirection.Down);
                        ShiftPlayers(sectorRouteForPlayer, entity);                            
                        continue;
                    }

                }
            }
            return entity;
        }

        private void ShiftPlayers(EntityGameSectorRoute entitySectorRoute, IGameSectorsOwner gameSectorOwner)
        {
            foreach (var sectorPair in GameSectorRoutePairs)
            {
                if (entitySectorRoute.GameSectorTag != sectorPair.GameSectorTag || 
                    entitySectorRoute.Trespassing != sectorPair.Trespassing || 
                    entitySectorRoute.Trespassing == GameSectorTrespassingDirection.None) 
                    continue;
                

                Console.WriteLine($"{entitySectorRoute.LoginToken} -> {entitySectorRoute.GameSectorTag} {sectorPair.GameSectorTag} to {sectorPair.GameSectorDestinationTag} {entitySectorRoute.Position.X},{entitySectorRoute.Position.Y} ");
                var gameSectorOfPlayer = gameSectorOwner.GameSectorLayerServices.FirstOrDefault(gs => gs.Value.SharedContext.Tag == entitySectorRoute.GameSectorTag);
                if (gameSectorOfPlayer.Value == null)
                    continue;
                
                
                var gameSectorDestination = gameSectorOwner.GameSectorLayerServices.FirstOrDefault(gs => gs.Value.SharedContext.Tag == sectorPair.GameSectorDestinationTag);
                if (gameSectorDestination.Value == null)
                {
                    //BUILD THE SECTOR HERE (IF LAZY LOADED AND NO OTHER PLAYER HAS TRIGGERED IT) 
                    continue;
                }
                    
                    
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

                const int offset = 128;
                switch (entitySectorRoute.Trespassing)
                {
                    case GameSectorTrespassingDirection.Up:
                        Console.WriteLine("UP");
                        Console.WriteLine($"FROM Y {thePlayer.CurrentImage.Position.Y} -> {gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorY - entitySectorRoute.Position.Y - offset}");
                        thePlayer.CurrentImage.Position.Y = gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorY - offset;
                        break;
                    case GameSectorTrespassingDirection.Left:
                        Console.WriteLine("LEFT");
                        Console.WriteLine($"FROM X {thePlayer.CurrentImage.Position.X} -> {gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorX - entitySectorRoute.Position.X + offset}");
                        thePlayer.CurrentImage.Position.X = gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorX - offset;
                        break;
                    case GameSectorTrespassingDirection.Right:
                        Console.WriteLine("RIGHT");
                        Console.WriteLine($"FROM X {thePlayer.CurrentImage.Position.X} -> {gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorX - entitySectorRoute.Position.X + offset}");
                        thePlayer.CurrentImage.Position.X = gameSectorDestination.Value.SharedContext.SectorBoundaries.MinSectorX + offset;
                        break;
                    case GameSectorTrespassingDirection.Down:
                        Console.WriteLine("DOWN");
                        Console.WriteLine($"FROM Y {thePlayer.CurrentImage.Position.Y} -> {gameSectorDestination.Value.SharedContext.SectorBoundaries.MaxSectorY - entitySectorRoute.Position.Y - offset}");
                        thePlayer.CurrentImage.Position.Y = gameSectorDestination.Value.SharedContext.SectorBoundaries.MinSectorY + offset;
                        break;
                }

                //RESET TRESPASSING
                entitySectorRoute.Trespassing = GameSectorTrespassingDirection.None;
                    
                gameSectorDestination
                    .Value
                    .SharedContext
                    .DataLayer
                    .Players
                    .Add(thePlayer);
                
                gameSectorDestination.Value.SharedContext.IODataLayer.RegisterGameStateData(thePlayer.LoginToken);
                gameSectorDestination.Value.SharedContext.IODataLayer.ForwardServerDataToGame(gameStateData);
                entitySectorRoute.GameSectorTag = gameSectorDestination.Value.SharedContext.Tag;

            }
        }
    }
}