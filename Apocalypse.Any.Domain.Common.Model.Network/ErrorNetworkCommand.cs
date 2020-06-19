using System;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class ErrorNetworkCommand : NetworkCommand
    {
        public Exception WhatsTheProblem { get; set; }

        public ErrorNetworkCommand()
        {
            this.CommandName = "Error";
            this.CommandArgument = typeof(Exception).FullName;
        }
    }
}