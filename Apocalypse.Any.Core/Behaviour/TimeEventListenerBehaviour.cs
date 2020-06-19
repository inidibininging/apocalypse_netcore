using Apocalypse.Any.Core.Events;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Core.Behaviour
{
    /// <summary>
    /// Timer class for doing loops and one time operations that should be executed later on
    /// </summary>
	public class TimeEventListenerBehaviour
        : Behaviour<ITimeEventHandler>
    {
        public TimeSpan Length { get; set; }
        public TimeSpan CurrentTime { get; private set; }
        public bool Loop { get; set; }
        private bool TimeReached { get; set; }
        public string Name { get; private set; }

        public TimeEventListenerBehaviour(ITimeEventHandler target, string timeEventName, TimeSpan length) : base(target)
        {
            if (string.IsNullOrWhiteSpace(timeEventName))
                throw new ArgumentException("The given name for this time event is empty");
            if (Math.Abs(length.TotalMilliseconds) < 0.0)
                throw new ArgumentException("The given time event length is invalid");
            Name = timeEventName;
            Length = length;
            CurrentTime = Length;
        }

        public override void Update(GameTime gameTime)
        {
            if (TimeReached && !Loop)
                return;
            else
                TimeReached = false;

            CurrentTime = CurrentTime.Subtract(gameTime.ElapsedGameTime);
            if (CurrentTime.TotalMilliseconds <= 0)
            {
                NotifyTick();
                TimeReached = true;
                if (Loop)
                    CurrentTime = Length;
            }

            base.Update(gameTime);
        }

        private void NotifyTick()
        {
            Target?.Tick(this);
        }
    }
}