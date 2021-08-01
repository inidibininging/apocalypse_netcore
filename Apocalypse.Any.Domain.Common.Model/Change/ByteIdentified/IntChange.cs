using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model.Change.ByteIdentified
{
    public struct IntChange : IEntityChange<int, byte>
    {
        public byte Identifier { get; }

        public int Data { get; }

        public IntChange(byte identifier, int value)
        {
            Identifier = identifier;
            Data = value;
        }
    }
}
