using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model.Change.ByteIdentified
{
    public class ColorChange : IEntityChange<Color, byte>
    {
        public byte Identifier { get; }

        public Color Data { get; }

        public ColorChange(byte identifier, Color value)
        {
            Identifier = identifier;
            Data = value;
        }

    }
}
