using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class LaserProjectileFactory : CheckWithReflectionFactoryBase<Projectile>
    {
        public override bool CanUse<TParam>(TParam instance)
        {
            return GetValidParameterTypes().Any(t => CheckTypeOfSubjectWithType<TParam>(t));
        }

        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(Projectile), typeof(ICharacterEntity), typeof(PlayerSpaceship) };
        }


        public string IdPrefix { get; set; } = "projectile";
        private IThrustMechanic ThrustMechanics { get; set; }

        public LaserProjectileFactory(IThrustMechanic thrustMechanics)
        {
            if (thrustMechanics == null)
                throw new ArgumentNullException(nameof(thrustMechanics));
            ThrustMechanics = thrustMechanics;
        }

        protected override Projectile UseConverter<TParam>(TParam parameter)
        {
            var previousProjectile = parameter as Projectile;
            var owner = parameter as ICharacterEntity;

            if (owner != null) 
            {
                var projectile = new Projectile()
                {
                    Damage = (owner.Stats.Attack + owner.Stats.Strength),
                    OwnerName = owner.DisplayName,
                    CurrentImage = new ImageData()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Alpha = new AlphaBehaviour() { Alpha = 1 },
                        Path = "Image/gamesheetExtended",
                        SelectedFrame = $"{IdPrefix}_4_7",
                        Height = 32,
                        Width = 32,
                        Scale = new Vector2(1.5f, 1.5f),
                        Color = (owner as PlayerSpaceship != null) ? Color.Red : Color.GreenYellow,
                        Position = new MovementBehaviour()
                        {
                            X = owner.CurrentImage.Position.X + 1,
                            Y = owner.CurrentImage.Position.Y
                        },
                        //Rotation = owner.CurrentImage.Rotation,
                        Rotation = new RotationBehaviour()
                        {
                            Rotation = owner.CurrentImage.Rotation.Rotation
                        },
                        LayerDepth = DrawingPlainOrder.Entities
                    },
                    CreationTime = DateTime.Now,
                    DecayTime = 0.5.Seconds()
                };
                ThrustMechanics.Update(projectile.CurrentImage, 3);
                return projectile;
            }
            if(previousProjectile != null)
            {                
                var projectile = new Projectile()
                {                    
                    Damage = previousProjectile.Damage,
                    OwnerName = previousProjectile.OwnerName,
                    CurrentImage = new ImageData()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Alpha = new AlphaBehaviour() { Alpha = 1 },
                        Path = "Image/gamesheetExtended",
                        SelectedFrame = $"{IdPrefix}_5_10",
                        Height = 32,
                        Width = 32,
                        Scale = new Vector2(previousProjectile.CurrentImage.Scale.X, previousProjectile.CurrentImage.Scale.Y + 1),
                        Color = previousProjectile.CurrentImage.Color,
                        Position = new MovementBehaviour()
                        {
                            X = previousProjectile.CurrentImage.Position.X,
                            Y = previousProjectile.CurrentImage.Position.Y
                        },
                        Rotation = previousProjectile.CurrentImage.Rotation,
                        //Rotation = new RotationBehaviour()
                        //{
                        //    Rotation = previousProjectile.CurrentImage.Rotation.Rotation
                        //},
                        LayerDepth = DrawingPlainOrder.Entities
                    },
                    CreationTime = DateTime.Now,
                    DecayTime = 0.5.Seconds()
                };
                ThrustMechanics.Update(projectile.CurrentImage, 16 * (previousProjectile.CurrentImage.Scale.Y + 1));
                previousProjectile.CurrentImage.SelectedFrame = $"{IdPrefix}_3_10";
                
                return projectile;                
            }
            return null;
        }
    }
}
