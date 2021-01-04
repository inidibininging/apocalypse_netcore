using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model.Change.ByteIdentified
{
    public class FloatChange : IEntityChange<float, byte>
    {
        public byte Identifier { get; }

        public float Data { get; }

        public FloatChange(byte identifier, float value)
        {
            Identifier = identifier;
            Data = value;
        }
    }
}
