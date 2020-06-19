using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics
{
    public class ProcessCollisionMechanicState : IState<string, IGameSectorLayerService>
    {
        private IRectangleCollisionMechanic CollisionMechanic { get; }
        private ImageToRectangleTransformationService ImageToRectangleService { get; }
        public ProcessCollisionMechanicState(
            IRectangleCollisionMechanic rectangleCollisionMechanic,
            ImageToRectangleTransformationService imageToRectangleService)
        {
            CollisionMechanic = rectangleCollisionMechanic ?? throw new ArgumentNullException(nameof(rectangleCollisionMechanic));
            ImageToRectangleService = imageToRectangleService ?? throw new ArgumentNullException(nameof(imageToRectangleService));
        }
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            CollisionMechanic.Update(
                machine
                .SharedContext
                .DataLayer
                .Projectiles
                .Select(projectile => new ImageDataCollisionWrapper<IEntityWithImage>(projectile, (target) =>
                                {
                                    if (target.GetType() == typeof(Projectile)){
                                        return;
                                    }
                                    else
                                    {
                                        if (target.GetType() == typeof(PlayerSpaceship))
                                        {
                                            if (machine.SharedContext.DataLayer.Players.Any(p => p.Name == projectile.OwnerName))
                                            {
                                                return;
                                            }
                                            else
                                            {
                                                (target as PlayerSpaceship).Stats.Health -= projectile.Damage;
                                            }
                                        }

                                        if (target.GetType() == typeof(EnemySpaceship))
                                        {
                                            if (machine.SharedContext.DataLayer.Enemies.Any(enemy => enemy.Name == projectile.OwnerName))
                                            {
                                                return;
                                            }
                                            else
                                            {
                                                var currentEnemy = (target as EnemySpaceship);
                                                currentEnemy.Stats.Health -= projectile.Damage;
                                                if( currentEnemy.Stats.Health <= currentEnemy.Stats.GetMinAttributeValue())
                                                {
                                                    var belongingToPlayer = machine.SharedContext.DataLayer.Players.FirstOrDefault(player => player.Name == projectile.OwnerName );
                                                    if(belongingToPlayer != null)
                                                    {
                                                        belongingToPlayer.Stats.Kills++;
                                                    }
                                                    else
                                                    {
                                                        var belongingToAnEnemy = machine.SharedContext.DataLayer.Enemies.FirstOrDefault(enemy => enemy.Name == projectile.OwnerName );
                                                        if(belongingToAnEnemy != null)
                                                        {
                                                            belongingToAnEnemy.Stats.Kills++;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        projectile.Destroyed = true;
                                        projectile.CurrentImage.Color = Color.Yellow;
                                        projectile.CurrentImage.SelectedFrame = "projectile_5_7";
                                    }
                                }, ImageToRectangleService))
                .Concat
                (
                    machine.SharedContext.DataLayer.Enemies.Select(enemy => new ImageDataCollisionWrapper<IEntityWithImage>(enemy, (target) =>
                    {
                        if (target.GetType() == typeof(Projectile))
                        {
                            if (machine.SharedContext.DataLayer.Enemies.Any(possibleOwner => possibleOwner.Name == (target as Projectile).OwnerName))
                                return;

                            //enemy.Stats.Health -= (target as Projectile).Damage;
                        }

                        //else
                        //enemy.Stats.Health -= 1;
                    }, ImageToRectangleService))
                )
                .Concat
                (
                    machine.SharedContext.DataLayer.Players.Select(player => new ImageDataCollisionWrapper<IEntityWithImage>(player, (target) =>
                    {
                        var t = target.GetType();

                        if (t != typeof(Item))
                            return;

                        //consumption
                        var item = (target as Item);

                        if (item.Taken)
                            return;
                        

                        //pass stats to taker
                        player.Stats.Attack += item.Stats.Attack;
                        player.Stats.Health += item.Stats.Health;
                        player.Stats.Strength += item.Stats.Strength;
                        //player.Stats.Speed += item.Stats.Speed;
                        player.Stats.Experience += item.Stats.Experience;
                        item.OwnerName = player.Name;
                        machine.SharedContext.Messages.Add($"item {item.Name} passed to {player.Name}");
                        item.Taken = true;
                    }, ImageToRectangleService))
                )
                .Concat
                (
                    machine.SharedContext.DataLayer.Items.Select(item => new ImageDataCollisionWrapper<IEntityWithImage>(item, (target) =>
                    {
                    }, ImageToRectangleService))
                )
            );
        }
    }
}
