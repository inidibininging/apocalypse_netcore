using System;
using Echse.Net.Domain;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class ErrorNetworkCommand : NetworkCommand
    {
        public Exception WhatsTheProblem { get; set; }

        public ErrorNetworkCommand()
        {
            this.CommandName = 6;
            this.CommandArgument = typeof(Exception).FullName;
        }
    }
}