using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    public class ExamplePlayerDialogService : IPlayerDialogService
    {
        private Dictionary<string, string> OpenedPlayerDialogs { get; set; } = new Dictionary<string, string>();
        public IDialogService DialogService { get; }

        public ExamplePlayerDialogService(IDialogService dialogService)
        {
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public DialogNode GetDialogNodeByLoginToken(string loginToken)
        {
            if (!OpenedPlayerDialogs.ContainsKey(loginToken))
                return null;
            var currentDialogId = OpenedPlayerDialogs[loginToken];
            return DialogService.GetDialogNode(currentDialogId);
        }

        public void SwitchDialogNodeByLoginToken(string loginToken, string id) => OpenedPlayerDialogs[loginToken] = id;



    }
}
