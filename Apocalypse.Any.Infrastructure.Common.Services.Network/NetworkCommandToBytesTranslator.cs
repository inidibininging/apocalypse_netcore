using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network
{
    public class NetworkCommandToBytesTranslator : IInputTranslator<NetworkCommand, byte[]>
    {
        private IInputTranslator<string, byte> CommandArgumentTranslator { get; }
        private IInputTranslator<byte[], byte[]> DataTranslator { get;  }
        public NetworkCommandToBytesTranslator(
            IInputTranslator<string, byte> commandArgumentTranslator,
            IInputTranslator<byte[], byte[]> dataTranslator)
        {
            CommandArgumentTranslator = commandArgumentTranslator ?? throw new ArgumentNullException(nameof(commandArgumentTranslator));
            DataTranslator = dataTranslator ?? throw new ArgumentNullException(nameof(dataTranslator));
        }
        public byte[] Translate(NetworkCommand input)
        {
            var output = new byte[] { input.CommandName };
            
            if(!string.IsNullOrWhiteSpace(input.CommandArgument))
                output[1] = CommandArgumentTranslator.Translate(input.CommandArgument);

            if (input.Data != null)
                DataTranslator.Translate(input.Data).CopyTo(output, 2);
            
            return output;
        }
    }
}
