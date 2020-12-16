using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Core.Input.Translator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Core.Services
{
    /// <summary>
    /// Records all the supported input devices and puts them into place
    /// </summary>
    public class DefaultBusInputRecordService : GameObject, IInputService
    {
        private IInputTranslator<MouseState, IEnumerable<string>> MouseTranslator { get; set; }
        private IInputTranslator<Keys[], IEnumerable<string>> KeyboardTranslator { get; set; }
        private IList<IInputTranslator<GamePadState, IEnumerable<string>>> GamePadsTranslators { get; set; }
        private IList<IInputTranslator<JoystickState, IEnumerable<string>>> JoystickTranslators { get; set; }
        public IInputTranslator<ScreenService, IEnumerable<string>> ScreenServiceVector2Translator { get; set; }

        public DefaultBusInputRecordService(
            IInputTranslator<Keys[], IEnumerable<string>> keyboardTranslator = null,
            IInputTranslator<MouseState, IEnumerable<string>> mouseTranslator = null,
            IList<IInputTranslator<GamePadState, IEnumerable<string>>> gamepadTranslators = null,
            IList<IInputTranslator<JoystickState, IEnumerable<string>>> joystickTranslators = null,
            IInputTranslator<ScreenService, IEnumerable<string>> screenServiceVector2Translator = null)
        {
            MouseTranslator = mouseTranslator;//(mouseTranslator ?? new GenericMouseDeviceToKeysTranslator());
            KeyboardTranslator = keyboardTranslator;//(keyboardTranslator ?? new GenericKeyboardDeviceToKeysTranslator());

            GamePadsTranslators = gamepadTranslators;//(gamepadTranslators ?? GetGenericGamePadTranslators());
            JoystickTranslators = joystickTranslators;//((joystickTranslators ?? GetGenericJoystickTranslators()); //TODO!

            ScreenServiceVector2Translator = screenServiceVector2Translator;
        }

        private IList<IInputTranslator<GamePadState, IEnumerable<string>>> GetGenericGamePadTranslators()
        {
            var gamePadTranslators = new List<IInputTranslator<GamePadState, IEnumerable<string>>>();
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++)
            {
                try
                {
                    var gState = GamePad.GetState(i);
                    if (gState.IsConnected)
                        gamePadTranslators.Add(new GenericGamepadDeviceToKeysTranslator(i));
                }
                catch (TypeInitializationException tiexGamePads)
                {
                    //ignore. we are in a vm. thats the reason this is failing ... for now ... wooork around it
                    //TODO:how to prevent ex on var gState = GamePad.GetState(i);
                }
            }
            return gamePadTranslators;
        }

        private IList<IInputTranslator<JoystickState, IEnumerable<string>>> GetGenericJoystickTranslators()
        {
            var joystickTranslators = new List<IInputTranslator<JoystickState, IEnumerable<string>>>();
            
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++)
            {
                try
                {
                    var jState = Joystick.GetState(i);
                    if (jState.IsConnected)
                        JoystickTranslators.Add(new JoystickDeviceToKeysTranslator(i));
                }
                catch (NullReferenceException nrex)
                {
                }
                catch (TypeInitializationException tiex)
                {
                }
            }
            return joystickTranslators;
        }

        public IEnumerable<string> GetInput()
        {
            var allOutputs = new List<string>();

            if (MouseTranslator != null)
                allOutputs.AddRange(MouseTranslator.Translate(Mouse.GetState()));

            if (KeyboardTranslator != null)
                allOutputs.AddRange(KeyboardTranslator.Translate(Keyboard.GetState().GetPressedKeys()));

            if (GamePadsTranslators != null)
            {
                var index = GamePadsTranslators.Count;
                allOutputs
                    .AddRange
                    (
                        GamePadsTranslators.Select(gPadTranslator =>
                        {
                            return gPadTranslator.Translate(GamePad.GetState(index: index--));
                        }
                    )
                    .SelectMany(input => input));
            }

            if (JoystickTranslators != null)
            {
                
                var index = JoystickTranslators.Count;
                allOutputs
                    .AddRange
                    (
                        JoystickTranslators.Select(joystickTranslator =>
                        {
                            return joystickTranslator.Translate(Joystick.GetState(index--));
                        }
                    )
                    .SelectMany(input => input));
            }
            if (ScreenServiceVector2Translator != null)
                allOutputs.AddRange(ScreenServiceVector2Translator.Translate(ScreenService.Instance));

            return allOutputs;
        }

        public IEnumerable<string> InputBefore { get; private set; }

        private IEnumerable<string> _inputNow;

        public IEnumerable<string> InputNow
        {
            get
            {
                //TODO: this is an input bugfix. check if it works
                if (_inputNow == null || !_inputNow.Any())
                    _inputNow = new List<string>();
                else
                    _inputNow = _inputNow.ToList();
                return _inputNow;
            }
            private set
            {
                _inputNow = value;
            }
        }

        public override void Update(GameTime time)
        {
            InputBefore = !InputNow.Any() ? GetInput() : InputNow;
            InputNow = GetInput();
        }
    }
}