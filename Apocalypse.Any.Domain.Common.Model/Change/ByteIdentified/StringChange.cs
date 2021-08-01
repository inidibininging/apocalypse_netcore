using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model.Change.ByteIdentified
{
    public class StringChange : IEntityChange<string, byte>
    {
        public byte Identifier { get; }

        public string Data { get; }

        public StringChange(byte identifier, string value)
        {
            Identifier = identifier;
            Data = value;
        }
    }
}
