using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using Lidgren.Network;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network
{
    public class BytesToNetworkCommandTranslator : IInputTranslator<byte[], NetworkCommand>
    {
        public IInputTranslator<byte[], string> DataTranslator { get; }
        public IInputTranslator<byte, string> CommandArgumentTranslator { get; }

        public BytesToNetworkCommandTranslator(
            IInputTranslator<byte, string> commandArgumentTranslator,
            IInputTranslator<byte[], string> dataTranslator)
        {
            DataTranslator = dataTranslator ?? throw new ArgumentNullException(nameof(dataTranslator));
            CommandArgumentTranslator = commandArgumentTranslator ?? throw new ArgumentNullException(nameof(commandArgumentTranslator));
        }
        public NetworkCommand Translate(byte[] input)
        {
            var output = new NetworkCommand()
            {
                CommandName = input[0],
                CommandArgument = input.Length > 1 ? CommandArgumentTranslator.Translate(input[1]) : string.Empty,
                Data = input.Length > 2 ? DataTranslator.Translate(input.Skip(2).ToArray()) : string.Empty
            };
            return output;
        }
    }
}
