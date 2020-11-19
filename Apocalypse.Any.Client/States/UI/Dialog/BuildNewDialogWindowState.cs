using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Domain.Common.Drawing.UI;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States.UI.Dialog
{
    public class BuildNewDialogWindowState : IState<string, INetworkGameScreen>
    {
        public const string NewDialogWindow = nameof(NewDialogWindow);
        
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            var newDialogWindow = new ApocalypseWindow();
            var choicesListBox = new ApocalypseListBox<int>(machine.SharedContext.GameSheet.Frames);
            
            newDialogWindow.Add(choicesListBox);
            machine.SharedContext.Add(NewDialogWindow, newDialogWindow);
            
        }
    }
}