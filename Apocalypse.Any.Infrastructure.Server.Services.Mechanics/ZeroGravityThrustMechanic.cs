using System;
using System.Collections.Generic;
using System.ComponentModel;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class ZeroGravityThrustMechanic
        : ISingleCharacterEntityMechanic<PlayerSpaceship>
        
    {
        public bool Active { get; set; } = true;
        public float BasicAcceleration { get; set; } = 1;
        public float RotationalChange { get; set; } = 0.125f;
        private Dictionary<string, ThrustFullPositionHolder> PositionHolders { get; set; } = new Dictionary<string, ThrustFullPositionHolder>();
        
        public PlayerSpaceship Update(PlayerSpaceship target)
        {
            if (!PositionHolders.ContainsKey(target.Id))
            {
                PositionHolders.Add(target.Id,
                    new ThrustFullPositionHolder()
                        {
                            Position = new MovementBehaviour()
                            {
                                X = target.CurrentImage.Position.X,
                                Y = target.CurrentImage.Position.Y
                            },
                            Rotation = new RotationBehaviour()
                            {
                                Rotation = target.CurrentImage.Rotation.Rotation
                            },
                            SpeedFactor = BasicAcceleration
                        });
            }
            
            PositionHolders[target.Id].SpeedFactor = BasicAcceleration;
            PositionHolders[target.Id].Position.X = target.CurrentImage.Position.X;
            PositionHolders[target.Id].Position.Y = target.CurrentImage.Position.Y;

            if (target.Tags.Contains(DefaultKeys.Boost))
            {
                PositionHolders[target.Id].Rotation.Rotation = MathHelper.Lerp(PositionHolders[target.Id].Rotation.Rotation, target.CurrentImage.Rotation.Rotation, RotationalChange);
            }
            else
            {
                GetNextX(PositionHolders[target.Id], PositionHolders[target.Id].SpeedFactor);
                GetNextY(PositionHolders[target.Id], PositionHolders[target.Id].SpeedFactor);
                target.CurrentImage.Position.X = PositionHolders[target.Id].Position.X; 
                target.CurrentImage.Position.Y = PositionHolders[target.Id].Position.Y;             
            }

            return target;
        }
        private void GetNextX(IFullPositionHolder target, float accelerationDelta)
        {
            var x = (float)(Math.Sin(target.Rotation));
            target.Position.X += x * BasicAcceleration * accelerationDelta;
            
            //target.Position.X = MathHelper.Lerp(target.Position.X, target.Position.X + (x * BasicAcceleration * accelerationDelta), 0.01f);
        }

        private void GetNextY(IFullPositionHolder target, float accelerationDelta)
        {
            var y = (float)(Math.Cos(target.Rotation)) * -1;
            target.Position.Y += y * BasicAcceleration * accelerationDelta;
            
            //target.Position.Y = MathHelper.Lerp(target.Position.Y, target.Position.Y + (y * BasicAcceleration * accelerationDelta), 0.01f);
        }
    }
}