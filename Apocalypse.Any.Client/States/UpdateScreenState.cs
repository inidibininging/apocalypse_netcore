using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Services;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Updates the screen data with the data from the server
    /// </summary>
    public class UpdateScreenState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(UpdateScreenState));

            if (machine.SharedContext.CurrentGameStateData == null)
                return;
            if (machine.SharedContext.CurrentGameStateData.Screen == null)
                return;

            machine.SharedContext.CurrentGameStateData.Screen.ScreenWidth = int.Parse(ScreenService.Instance.Resolution.X.ToString());
            machine.SharedContext.CurrentGameStateData.Screen.ScreenHeight = int.Parse(ScreenService.Instance.Resolution.Y.ToString());
        }
    }
}