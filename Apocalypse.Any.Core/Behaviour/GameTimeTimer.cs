using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Core.Behaviour
{
    public sealed class GameTimeTimer : IUpdateableLite
    {
        private TimeSpan Length { get; set; }
        private TimeSpan TimeOut { get; set; }
        private Action FinalAction { get; set; }
        private bool Done { get; set; }
        public bool Loop { get; set; }

        public GameTimeTimer(TimeSpan timeOut)
        {
            if (Math.Abs(timeOut.TotalMilliseconds) < 0.0)
                throw new ArgumentException("The given time event length is invalid");

            TimeOut = timeOut;
            Length = timeOut;
        }

        public IUpdateableLite Do(Action action)
        {
            FinalAction = action;
            return this;
        }

        public void Update(GameTime time)
        {
            if (Done && !Loop)
                return;
            else
                Done = false;

            var newTimeOut = TimeOut - time.ElapsedGameTime;

            if (newTimeOut.TotalMilliseconds > 0)
                TimeOut = newTimeOut;
            else
            {
                if (FinalAction == null)
                    return;
                else
                {
                    FinalAction();
                    Done = true;
                    if (Loop)
                        TimeOut = Length;
                }
            }
        }
    }
}