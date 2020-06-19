using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Services;
using States.Core.Infrastructure.Services;
using System;

namespace Apocalypse.Any.Client.States
{
    public class BuildInputServiceState : IState<string, INetworkGameScreen>
    {
        public IInputService InputService { get; set; }

        public BuildInputServiceState(IInputService inputService)
        {
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(BuildInputServiceState));
            machine.SharedContext.InputService = InputService;
            machine.SharedContext.Messages.Add($"added a {nameof(DefaultBusInputRecordService)}");
        }
    }
}