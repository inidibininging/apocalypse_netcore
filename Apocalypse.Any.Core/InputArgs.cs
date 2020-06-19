using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Core
{
    internal class InputArgs : IInputArgs
    {
        //public static readonly IInputArgs NoInput = new InputArgs(
        //    new List<Keys>(new KeyboardState(null).GetPressedKeys()),
        //    new MouseState(0,
        //    0,
        //    0,
        //    ButtonState.Released,
        //    ButtonState.Released,
        //    ButtonState.Released,
        //    ButtonState.Released,
        //    ButtonState.Released));

        public List<string> PressedKeys { get; }
        public MouseState MouseStates { get; }

        private DateTime recordedOn = DateTime.Now;
        public DateTime RecordedOn => recordedOn;

        /// <summary>
        /// Assumes that the input can be keyboard and mouse. This is ugly.. actually there should be 3 classes for each input (game pad)
        /// </summary>
        /// <param name="keyStates"></param>
        /// <param name="mouseStates"></param>
        internal InputArgs(
            List<string> pressedKeys,
            MouseState mouseStates)
        {
            PressedKeys = pressedKeys;
            MouseStates = mouseStates;
        }

        //public static implicit operator List<Keys>(InputArgs input)
        //{
        //    return input.PressedKeys;
        //}
    }
}