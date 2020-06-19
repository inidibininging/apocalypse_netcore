using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Utilities;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Client.Mechanics
{
    public class EnemyThrustMechanicBehaviour<T> : Behaviour<T>
        where T :
        IFullPositionHolder,
        IUpdateableLite
    {
        private float DefaultThrustAcceleration = 0.6f;

        private TimeSpan _maxStopTime = 4.Seconds();

        public float Delta
        {
            get; protected set;
        }

        protected Microsoft.Xna.Framework.Input.Keys Key;

        public EnemyThrustMechanicBehaviour(T target) : base(target)
        {
            DefaultThrustAcceleration = Randomness.Instance.From(50, 220) / 100;
        }

        public TimeSpan StopTime { get; private set; } = 4.Seconds();
        private bool _triggered = false;

        //SimpleLivingObject

        public bool Triggered
        {
            get
            {
                return _triggered;
            }
            set
            {
                _triggered = value;
            }
        }

        private MovementBehaviour Movement => Target.Position;
        private RotationBehaviour Rotation => Target.Rotation;

        public override void Update(GameTime gameTime)
        {
            //Activate Thrust Trigger
            var triggerFired = Convert.ToSingle(Randomness.Instance.From(0, 100)) / 100f > 0.5;
            //var delta = Math.Round(Delta * 100, MidpointRounding.ToEven);
            if (triggerFired)
            {
                AddStopTime();
                FixFastThrust();
                Triggered = true;
            }
            else
            {
                if (Triggered)
                {
                    FadingThrust();
                    CheckResetStopTime(gameTime);
                }
                else
                {
                    Thrust(GetDefaultAcceleration());
                }
            }
            base.Update(gameTime);
        }

        #region Time Stuff

        private void CheckResetStopTime(GameTime gameTime)
        {
            if (StopTime <= TimeSpan.Zero && Delta == 0)
            {
                ResetStopTime();
            }
            else
            {
                StopTime -= gameTime.ElapsedGameTime;
            }
        }

        private void ResetStopTime()
        {
            Triggered = false;
            StopTime = _maxStopTime;
        }

        private void AddStopTime()
        {
            if (StopTime <= 4.Seconds())
            {
                StopTime.Add(1.Seconds());
            }
        }

        #endregion Time Stuff

        #region Thrust Functions

        protected virtual float GetDefaultAcceleration()
        {
            return DefaultThrustAcceleration;
        }

        private void Thrust(float accelerationDelta)
        {
            // Face Towards ( From Top Down View )
            if (Rotation == null) throw new ArgumentNullException(nameof(Rotation));

            GetNextX(accelerationDelta);
            GetNextY(accelerationDelta);
        }

        public void GetNextX(float accelerationDelta)
        {
            var x = (float)(Math.Sin(Rotation));
            Movement.X += x * 3f * accelerationDelta;
        }

        public void GetNextY(float accelerationDelta)
        {
            var y = (float)(Math.Cos(Rotation)) * -1;
            Movement.Y += y * 3f * accelerationDelta;
        }

        private void SlowThrust(GameTime gameTime)
        {
            if (StopTime >= 0.Seconds())
            {
                StopTime -= gameTime.ElapsedGameTime;
            }
        }

        private void FixFastThrust()
        {
            Thrust(DefaultThrustAcceleration * 2 + (float)8.Seconds().Milliseconds / 5500);
        }

        private void FadingThrust()
        {
            SlowDelta();
            Thrust(DefaultThrustAcceleration + Delta);
        }

        protected virtual void SlowDelta()
        {
            //I wasted MONTHS!!! because of this fucking function!
            //I put Milliseconds instead of TOTALMiliseconds... grrrr
            Delta = ((float)StopTime.TotalMilliseconds / 3000 - 1);
            if (!(Delta < 0)) return;
            Delta = 0;
            ResetStopTime();
        }

        #endregion Thrust Functions
    }
}