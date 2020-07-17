using Apocalypse.Any.Core.Behaviour;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model
{
    public class CircularLocation
    {
        public MovementBehaviour Position { get; set; }
        public float Radius { get; set; }
    }
}
