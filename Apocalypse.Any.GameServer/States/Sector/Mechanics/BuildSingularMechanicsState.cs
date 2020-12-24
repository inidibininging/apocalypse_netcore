using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.GameServer.Mechanics.Proxy;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Bridge;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.EnemyMechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.PositionMechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.ProjectileMechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.SectorMechanics;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics
{
    public class BuildSingularMechanicsState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (machine.SharedContext.SingularMechanics == null)
            {
                machine.SharedContext.SingularMechanics = new InMemorySingularMechanics
                {
                    PlayerMechanics = new Dictionary<string, ISingleCharacterEntityMechanic<PlayerSpaceship>>(),
                    EnemyMechanics = new Dictionary<string, ISingleCharacterEntityMechanic<EnemySpaceship>>(),
                    ProjectileMechanics = new Dictionary<string, ISingleEntityWithImageMechanic<Projectile>>(),
                    ItemMechanics = new Dictionary<string, ISingleCharacterEntityMechanic<Item>>(),
                    ImageDataMechanics = new Dictionary<string, ISingleFullPositionHolderMechanic<ImageData>>()
                };
            }

            PropRotation(machine);

            AttractPlayerToTheBiggestObject(machine);

            AttractEnemyToPlayer(machine);

            EnemyFacePlayerInDistance(machine);

            ThrustPlayer(machine);

            ThrustEnemy(machine);

            MakeEnemyMoveRandomly(machine);

            EnemyWontLeaveScreen(machine);

            EnemyShootProjectiles(machine);

            ProjectileMoveSlow(machine);

            AttractProjectileToEnemy(machine);

            ProjectileDecay(machine);

            MovePropAround(machine);

            StretchFog(machine);
        }

        private static void MovePropAround(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.ImageDataMechanics.ContainsKey("move_props_around"))
            {
                machine.SharedContext.SingularMechanics.ImageDataMechanics.Add("move_props_around",
                    new ThrustProxyImageDataMechanic(
                        new ThrustMechanic())
                    {
                        SpeedFactor = (float) Randomness.Instance.From(1, 100) / 200f
                    });
            }
        }

        private static void ProjectileDecay(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.ProjectileMechanics.ContainsKey("projectiles_decay_after"))
            {
                machine.SharedContext.SingularMechanics.ProjectileMechanics.Add("projectiles_decay_after",
                    new DestroyProjectileWithDecayTimeProxyMechanic(
                        new MarkProjectileWithDecayTimeAsDestroyedMechanic())
                    {
                        DestroyTimeSpan = 2.Seconds()
                    }
                );
            }
        }

        private static void ThrustEnemy(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.EnemyMechanics.ContainsKey("thrust_enemies"))
                machine.SharedContext.SingularMechanics.EnemyMechanics.Add("thrust_enemies",
                    CreateThrustCommand<EnemySpaceship>(new ThrustMechanic()
                    {
                        BasicAcceleration = 1.0f
                    }));
        }

        private static void ThrustPlayer(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.PlayerMechanics.ContainsKey("thrust_players"))
            {
                machine.SharedContext.SingularMechanics.PlayerMechanics.Add("thrust_players",
                    CreateThrustCommand<PlayerSpaceship>(new ThrustMechanic()
                    {
                        BasicAcceleration = 1.25f
                    }));
            }

            if (!machine.SharedContext.SingularMechanics.PlayerMechanics.ContainsKey("thrust_players_zero"))
            {
                machine.SharedContext.SingularMechanics.PlayerMechanics.Add("thrust_players_zero",
                    new ZeroGravityThrustMechanic()
                    {
                        BasicAcceleration = 1.25f
                    });
            }
        }

        private static void EnemyFacePlayerInDistance(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.EnemyMechanics.ContainsKey("enemy_face_player_if_distance"))
            {
                //TODO!!!
                machine.SharedContext.SingularMechanics.EnemyMechanics.Add("enemy_face_player_if_distance",
                    new SingleCharacterEntityFullPositionHolderAdapterMechanic<EnemySpaceship>(
                        new FacePointTargetWithinDistanceRangeProxyMechanic(new FacePointMechanic(), (enemyImage) =>
                        {
                            var minDistance = 1024;
                            var nearestPlayer = machine.SharedContext.DataLayer.Players.FirstOrDefault(plyr =>
                                Vector2.Distance(
                                    plyr.CurrentImage.Position,
                                    enemyImage.Position) < minDistance);
                            return nearestPlayer?.CurrentImage;
                        })
                        {
                            MinDistanceRange = 512,
                            MaxDistanceRange = 2048
                        }
                    ));
            }
        }

        private static void AttractEnemyToPlayer(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.EnemyMechanics.ContainsKey("attract_enemy_to_player"))
            {
                machine.SharedContext.SingularMechanics.EnemyMechanics.Add("attract_enemy_to_player",
                    new SingleCharacterEntityFullPositionHolderAdapterMechanic<EnemySpaceship>(
                        new FollowTargetWithinDistanceRangeProxyMechanic<IFullPositionHolder>(
                            new AttractionMechanic(),
                            (subject) =>
                                machine.SharedContext.DataLayer.Players.OrderByDescending(player => player.Stats.Kills)
                                    .FirstOrDefault()?.CurrentImage,
                            (subject) => 0.005f)
                        {
                            MinDistanceRange = 2048,
                            MaxDistanceRange = 4096
                        }));
            }
        }

        private static void AttractPlayerToTheBiggestObject(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.PlayerMechanics.ContainsKey(
                "move_players_to_the_biggest_object"))
            {
                machine.SharedContext.SingularMechanics.PlayerMechanics.Add("move_players_to_the_biggest_object",
                    new SingleCharacterEntityFullPositionHolderAdapterMechanic<PlayerSpaceship>(
                        new AttractionProxyMechanic<IFullPositionHolder>(
                            new AttractionMechanic(),
                            (subject) => machine.SharedContext.DataLayer.ImageData
                                .OrderByDescending(img => img.Scale.X * img.Width).FirstOrDefault(img =>
                                    img.Path >= ImagePaths.planetsRandom0_edit &&
                                    img.Path <= ImagePaths.planetsRandom4_edit),
                            (subject) => 0.0001f)));
            }
        }

        private static void PropRotation(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.ImageDataMechanics.ContainsKey("prop_rotation"))
                machine.SharedContext.SingularMechanics.ImageDataMechanics.Add("prop_rotation",
                    new PropRotationMechanic());
        }
        

        private static SingleCharacterEntityFullPositionHolderAdapterMechanic<TCharacterEntity>
            CreateThrustCommand<TCharacterEntity>(ThrustMechanic thrustMechanic)
            where TCharacterEntity : CharacterEntity, new()
        {
            return new SingleCharacterEntityFullPositionHolderAdapterMechanic<TCharacterEntity>(
                new ThrustProxyMechanic(thrustMechanic)
                {
                    SpeedFactor = thrustMechanic.BasicAcceleration
                });
        }


        private static void StretchFog(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.ImageDataMechanics.ContainsKey("stretch_fog"))
            {
                var morph = new Func<ImageData, ImageData>((entity) =>
                {
                    if (entity.SelectedFrame.frame == ImagePaths.FogFrame)
                    {
                        if (entity.Scale.X < 4)
                        {
                            var colorVec = entity.Color.ToVector3() + new Vector3(0.01f, 0.01f, 0.02f);
                            entity.Color = new Color(colorVec);
                            entity.Scale = new Vector2(entity.Scale.X + 0.001f, entity.Scale.Y - 0.001f);
                        }

                        if (entity.Scale.X > 4)
                        {
                            var colorVec = entity.Color.ToVector3() - new Vector3(0.02f, 0.01f, 0.01f);
                            entity.Color = new Color(colorVec);
                            entity.Scale = new Vector2(entity.Scale.X - 0.001f, entity.Scale.Y + 0.001f);
                        }
                    }

                    return entity;
                });

                machine.SharedContext.SingularMechanics.ImageDataMechanics.Add("stretch_fog",
                    new DelegateFullPositionHolderMechanic<ImageData>(morph));
            }
        }

        private static void AttractProjectileToEnemy(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.ProjectileMechanics.ContainsKey("projectile_attract_to_some_enemy"))
            {
                machine.SharedContext.SingularMechanics.ProjectileMechanics.Add(
                    "projectile_attract_to_some_enemy",
                    new SingleCharacterEntityWithImageDataAdapter<Projectile>(
                        new AttractionProxyMechanic<IFullPositionHolder>(
                            new AttractionMechanic(),
                            (subject) =>
                            {
                                try
                                {
                                    var upcastSubject =
                                        machine.SharedContext.DataLayer.Projectiles.FirstOrDefault(projectile =>
                                            projectile.CurrentImage.Id == (subject as ImageData)?.Id);

                                    var playerOwner = machine.SharedContext.DataLayer.Players.FirstOrDefault(plyr =>
                                        plyr.DisplayName == upcastSubject.OwnerName);
                                    if (playerOwner == null)
                                        return null;
                                    return machine.SharedContext.DataLayer.Enemies.FirstOrDefault(enemy =>
                                        Vector2.Distance(enemy.CurrentImage.Position,
                                            playerOwner.CurrentImage.Position) < 512)?.CurrentImage;
                                }
                                catch
                                {
                                    return null;
                                }
                            },
                            (subject) => 0.01f)));
            }
        }

        private static void ProjectileMoveSlow(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.ProjectileMechanics.ContainsKey(nameof(ProjectileMoveSlow)))
            {               
                var thrustMechanic = new ThrustMechanic();
                machine.SharedContext.SingularMechanics.ProjectileMechanics.Add(nameof(ProjectileMoveSlow),
                    new DelegateSingleImageDataMechanic<Projectile>((projectile) =>
                            {
                                var enemy = machine.SharedContext.DataLayer.Enemies.FirstOrDefault(e => e.Id == projectile.OwnerName);
                                var player = machine.SharedContext.DataLayer.Players.FirstOrDefault(e => e.Id == projectile.OwnerName);
                                const int basicAcceleration = 3;

                                if (enemy != null)
                                    thrustMechanic.Update(projectile.CurrentImage, basicAcceleration + (enemy.Stats.Speed / enemy.Stats.GetMaxAttributeValue()));
                                if (player != null)
                                    thrustMechanic.Update(projectile.CurrentImage, basicAcceleration + (player.Stats.Speed / player.Stats.GetMaxAttributeValue()));

                                return projectile;                                
                            }));
            }
        }
        

        private static void MakeEnemyMoveRandomly(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.EnemyMechanics.ContainsKey("make_enemies_move_randomly"))
            {
                machine.SharedContext.SingularMechanics.EnemyMechanics.Add("make_enemies_move_randomly",
                    new MoveRandomlyMechanic(
                        new ThrustMechanic(),
                        new RandomRotationMechanic()));
            }
        }

        private static void EnemyWontLeaveScreen(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.EnemyMechanics.ContainsKey("enemies_wont_leave_the_screen"))
            {
                machine.SharedContext.SingularMechanics.EnemyMechanics.Add("enemies_wont_leave_the_screen",
                    new SingleCharacterEntityFullPositionHolderAdapterMechanic<EnemySpaceship>(
                        new NeverLeaveTheScreenProxyMechanic(
                            new NeverLeaveTheSectorMechanic(),
                            () => machine.SharedContext.SectorBoundaries)));
            }
        }

        private static void EnemyShootProjectiles(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (!machine.SharedContext.SingularMechanics.EnemyMechanics.ContainsKey("enemy_shoot_projectile"))
            {
                Func<EnemySpaceship, EnemySpaceship> shootMechanic = (enemy) =>
                {
                    var minDistance = 256;
                    var maxProjectiles = 5;
                    if (!machine.SharedContext.DataLayer.Players.Any(plyr =>
                        Vector2.Distance(enemy.CurrentImage.Position, plyr.CurrentImage.Position) < minDistance))
                    {
                        return enemy;
                    }

                    if (machine.SharedContext.DataLayer.Projectiles.Count(proj =>
                        proj.OwnerName == enemy.DisplayName) > maxProjectiles)
                    {
                        return enemy;
                    }

                    Task.Factory.StartNew(() =>
                    {
                        if (machine.SharedContext.Factories.ProjectileFactory.ContainsKey(nameof(ProjectileFactory)))
                        {
                            var enemyProjectile = machine.SharedContext.Factories.ProjectileFactory[nameof(ProjectileFactory)].Create(enemy);
                            if (enemyProjectile != null)
                            {
                                enemyProjectile.OwnerName = enemy.DisplayName;
                                machine.SharedContext.DataLayer.Projectiles.Add(enemyProjectile);
                            }
                        }
                    });
                    return enemy;
                };
                machine.SharedContext.SingularMechanics.EnemyMechanics.Add("enemy_shoot_projectile",
                    new DelegateSingleCharacterEntityMechanic<EnemySpaceship>(shootMechanic));
            }
        }
    }
}

        
    
