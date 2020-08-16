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
            var next = parameter as Projectile;
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
                        Scale = new Vector2(0.75f, 1.5f),
                        Color = (owner as PlayerSpaceship != null) ? Color.Violet : Color.GreenYellow,
                        Position = new MovementBehaviour()
                        {
                            X = owner.CurrentImage.Position.X,
                            Y = owner.CurrentImage.Position.Y
                        },
                        Rotation = new RotationBehaviour()
                        {
                            Rotation = owner.CurrentImage.Rotation.Rotation
                        },
                        LayerDepth = DrawingPlainOrder.Entities
                    },
                    CreationTime = DateTime.Now,
                    DecayTime = 2.Seconds()
                };
                ThrustMechanics.Update(projectile.CurrentImage, 3);
                return projectile;
            }
            if(next != null)
            {
                var projectile = new Projectile()
                {
                    Damage = next.Damage,
                    OwnerName = next.OwnerName,
                    CurrentImage = new ImageData()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Alpha = new AlphaBehaviour() { Alpha = 1 },
                        Path = "Image/gamesheetExtended",
                        SelectedFrame = $"{IdPrefix}_{Randomness.Instance.From(1,3)}_10",
                        Height = 32,
                        Width = 32,
                        Scale = new Vector2(0.75f, 1.5f),
                        Color = Color.Violet,
                        Position = new MovementBehaviour()
                        {
                            X = next.CurrentImage.Position.X,
                            Y = next.CurrentImage.Position.Y
                        },
                        Rotation = new RotationBehaviour()
                        {
                            Rotation = next.CurrentImage.Rotation.Rotation
                        },
                        LayerDepth = DrawingPlainOrder.Entities
                    },
                    CreationTime = DateTime.Now,
                    DecayTime = 4.Seconds()
                };
                ThrustMechanics.Update(projectile.CurrentImage, 8);
                return projectile;                
            }
            return null;
        }
    }
}
