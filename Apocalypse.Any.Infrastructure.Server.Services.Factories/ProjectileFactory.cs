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

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories
{
    public class ProjectileFactory : CheckWithReflectionFactoryBase<Projectile>
    {
        public string IdPrefix { get; set; } = "projectile";
        private IThrustMechanic ThrustMechanics { get; set; }

        public ProjectileFactory(IThrustMechanic thrustMechanics)
        {
            if (thrustMechanics == null)
                throw new ArgumentNullException(nameof(thrustMechanics));
            ThrustMechanics = thrustMechanics;
        }
        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(ICharacterEntity) };
        }

        public override bool CanUse<TParam>(TParam instance)
        {
            return CanUseByTType<TParam, ICharacterEntity>();
        }

        protected override Projectile UseConverter<TParam>(TParam parameter)
        {
            var owner = parameter as ICharacterEntity;
            var createdProjectile =
                new Projectile()
                {
                    Damage = (owner.Stats.Attack + owner.Stats.Strength),
                    OwnerName = owner.Name,
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

            ThrustMechanics.Update(createdProjectile.CurrentImage, 3);
            return createdProjectile;
        }
    }
}