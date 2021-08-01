using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model.Change.ByteIdentified
{
    public class Vector2Change : IEntityChange<Vector2, byte>
    {
        public byte Identifier { get; }

        public Vector2 Data { get; }

        public Vector2Change(byte identifier, Vector2 data)
        {
            Identifier = identifier;
            Data = data;
        }
    }
}
