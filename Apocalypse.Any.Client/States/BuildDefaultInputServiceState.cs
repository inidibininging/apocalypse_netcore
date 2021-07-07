using System.Collections.Generic;
using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Core.Services;
using Microsoft.Xna.Framework.Input;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    public class BuildDefaultInputServiceState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(BuildDefaultInputServiceState));
            machine.SharedContext.InputService = new DefaultBusInputRecordService
            (
                new WASDKeyboardToKeysTranslator(),
                new GenericMouseDeviceToKeysTranslator(),
                new List<IInputTranslator<GamePadState, IEnumerable<string>>>
                {
                    new GenericGamepadDeviceToKeysTranslator(0),
                    new GenericGamepadDeviceToKeysTranslator(1),
                    new GenericGamepadDeviceToKeysTranslator(2),
                    new GenericGamepadDeviceToKeysTranslator(3),
                    new GenericGamepadDeviceToKeysTranslator(4)
                },
                new List<IInputTranslator<JoystickState, IEnumerable<string>>>
                {
                    new JoystickDeviceToKeysTranslator(),
                    new JoystickDeviceToKeysTranslator(1),
                    new JoystickDeviceToKeysTranslator(2),
                    new JoystickDeviceToKeysTranslator(3),
                    new JoystickDeviceToKeysTranslator(4)
                } //new ScreenVector2ToKeysTranslator(getPlayerFullPos)
            );
            machine.SharedContext.Messages.Add($"added default {nameof(DefaultBusInputRecordService)}");
        }
    }
}