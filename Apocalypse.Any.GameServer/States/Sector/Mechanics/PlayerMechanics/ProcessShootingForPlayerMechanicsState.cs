using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.DataLayer;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    public class ProcessShootingForPlayerMechanicsState : IState<string, IGameSectorLayerService>
    {
        public Dictionary<string, List<IdentifiableShortCounterThreshold>> PlayerCommandCache { get; set; } = new Dictionary<string, List<IdentifiableShortCounterThreshold>>();
        public byte MaxConsecutiveShootCommandsCounter { get; set; } = 2;
        public byte CooldownCounter { get; private set; }
        

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine
               .SharedContext
               .DataLayer
               .Players
               .Where(player => !string.IsNullOrWhiteSpace(player.LoginToken))
               .ToList()
               .ForEach(player =>
               {
                   var playerGameState = machine.SharedContext.IODataLayer.GetGameStateByLoginToken(player.LoginToken);
                   
                   ApplyCommandThresholdToPlayer(player, playerGameState, machine.SharedContext.CurrentGameTime);

                   playerGameState.Commands.ForEach(cmd =>
                   {
                       if (cmd == DefaultKeys.Shoot)
                       {
                           //Shooting part 


                           //Projectile lastProjectile = null;
                           var projectileCount = machine.SharedContext.DataLayer.Projectiles.Count(proj => proj.OwnerName == player.DisplayName);
                           if(projectileCount > 1)
                           {

                               var lastProjectile = machine.SharedContext.DataLayer.Projectiles.Where(p => p.OwnerName == player.DisplayName).OrderByDescending(p => p.CreationTime).LastOrDefault();
                               if (lastProjectile == null)
                                   return;
                               machine.SharedContext.DataLayer.Projectiles.Add(machine.SharedContext.Factories.ProjectileFactory[nameof(ProjectileFactory)].Create(lastProjectile));
                           }
                           else
                           {
                               var projectile = machine.SharedContext.Factories.ProjectileFactory[nameof(ProjectileFactory)].Create(player);
                               machine.SharedContext.DataLayer.Projectiles.Add(projectile);
                           }
                           //for (var projectileCounter = 0; projectileCounter < projectileCount; projectileCounter++)
                           //{
                           //    //laser
                           //    Projectile currentProjectile = null;
                           //    if (lastProjectile == null)
                           //        currentProjectile = machine.SharedContext.Factories.ProjectileFactory[nameof(ProjectileFactory)].Create(player);
                           //    else
                           //        currentProjectile = machine.SharedContext.Factories.ProjectileFactory[nameof(ProjectileFactory)].Create(lastProjectile);
                           //    lastProjectile = currentProjectile;

                           //    //currentProjectile.CurrentImage.Rotation.Rotation += (projectileCounter * 2) * (Randomness.Instance.TrueOrFalse() ? 1 : -1);
                           //    machine.SharedContext.DataLayer.Projectiles.Add(currentProjectile);
                           //}

                           
                       }
                       if (cmd == DefaultKeys.AltShoot &&
                           player.Stats.Strength > machine.SharedContext.DataLayer.Projectiles.Count(proj => proj.OwnerName == player.DisplayName))
                       {
                           var divisor = player.Stats.Aura == 0 ? 1 : player.Stats.Aura;
                           var projectileCount = player.Stats.Experience / divisor;
                           var maxProjectileCount = 8;
                           if (projectileCount > 1)
                           {
                               if (projectileCount > maxProjectileCount)
                                   projectileCount = maxProjectileCount;
                               
                               for (var projectileCounter = 0; projectileCounter < projectileCount; projectileCounter++)
                               {
                                   var currentProjectile = machine.SharedContext.Factories.ProjectileFactory[nameof(ProjectileFactory)].Create(player);
                                   currentProjectile.CurrentImage.Rotation.Rotation += (projectileCounter * 2) * (Randomness.Instance.TrueOrFalse() ? 1 : -1);
                                   machine.SharedContext.DataLayer.Projectiles.Add(currentProjectile);
                               }
                           }
                           else
                           {
                               var projectile = machine.SharedContext.Factories.ProjectileFactory[nameof(ProjectileFactory)].Create(player);
                               machine.SharedContext.DataLayer.Projectiles.Add(projectile);
                           }
                       }
                   });
               });
        }

        private void ApplyCommandThresholdToPlayer(Player player, GameStateData playerGameState, GameTime gameTime)
        {
            
            if (!PlayerCommandCache.ContainsKey(player.LoginToken))
            {
                //make a copy of the players command if not present
                PlayerCommandCache.Add(player.LoginToken, new List<IdentifiableShortCounterThreshold>());                                      
                return;
            }

            var lastPlayerCommandCache = PlayerCommandCache[player.LoginToken];
            var commandThresholdsMissing = playerGameState
                                           .Commands
                                           .Where(cmd => !lastPlayerCommandCache.Any(commandThreshold => commandThreshold.Id == cmd))
                                           .Select(cmd => new IdentifiableShortCounterThreshold()
                                           {
                                               Id = cmd,
                                               Max = MaxConsecutiveShootCommandsCounter,
                                               Counter = 0,
                                               CooldownDeadline = TimeSpan.Zero
                                           });
            PlayerCommandCache[player.LoginToken].AddRange(commandThresholdsMissing);

            var commandThresholdsUsed = lastPlayerCommandCache.Where(threshold => playerGameState.Commands.Contains(threshold.Id));
            //var commandThresholdsNotUsed = lastPlayerCommandCache.Except(commandThresholdsUsed);
            foreach(var commandThresholdUsed in commandThresholdsUsed)
            {

                //every player command has its own counter. if the player sends more than X times a command and reaches the counter (Max),
                //the command should be stopped, till the game time surpassses the cooldown time
                //the command is stopped by removing the players command
                var currentThreshold = commandThresholdUsed;
                if (currentThreshold.MaxReached)
                {
                    //Console.WriteLine($"Counter: {currentThreshold.Counter}");
                    if (currentThreshold.CooldownDeadline == TimeSpan.Zero)
                    {
                        //Console.WriteLine($"CooldownDeadline...");
                        currentThreshold.CooldownDeadline = GetCalculatedCooldownBasedOnPlayerStatsForShooting(player, gameTime);
                    }
                    else
                    {
                        if (gameTime.ElapsedGameTime.TotalMilliseconds > currentThreshold.CooldownDeadline.TotalMilliseconds)
                        {
                            //Console.WriteLine($"{currentThreshold.Id}, reset ({gameTime.ElapsedGameTime.TotalMilliseconds} / {currentThreshold.CooldownDeadline.TotalMilliseconds}) ");
                            
                            currentThreshold.CooldownDeadline = TimeSpan.Zero;
                            currentThreshold.Counter = 0;
                        }
                        else
                        {
                            
                            var currentCommandIndexInPlayersGameState = playerGameState.Commands.IndexOf(currentThreshold.Id);
                            if (currentCommandIndexInPlayersGameState == -1)
                            {
                                //Console.WriteLine($"CooldownDeadline continue");
                                continue;
                            }                                
                            else
                            {
                                //Console.WriteLine($"Command removed: {currentThreshold.Id} at {currentCommandIndexInPlayersGameState}");                                
                                playerGameState.Commands.RemoveAt(currentCommandIndexInPlayersGameState);
                            }
                        }
                    }
                }
                else
                {                    
                    currentThreshold.Counter += 1;
                }
            }
            
        }

        private TimeSpan GetCalculatedCooldownBasedOnPlayerStatsForShooting(Player player, GameTime gameTime)
        {
            //TODO: this should be used in combination with an API that makes all the player's RPG calculations.
            var playerStats = player.Stats.Attack * (player.Stats.Experience == 0 ? 1 : player.Stats.Experience);
            if (playerStats == 0)
                playerStats = player.Stats.Attack;

            return gameTime.ElapsedGameTime.Add(TimeSpan.FromMilliseconds(200)); //player.Stats.Attack / playerStats));
        }

    }
}