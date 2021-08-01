using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model.Change.ByteIdentified
{
    public class ByteChange : IEntityChange<byte, byte>
    {
        public byte Identifier { get; }

        public byte Data { get; }

        public ByteChange(byte identifier, byte data)
        {
            Identifier = identifier;
            Data = data;
        }
    }
}
