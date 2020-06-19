using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Server.Sector.Model
{
    public class CharacterPositionTimeTrigger : IIdentifiableModel
    {
        public int MilisecondsToTrigger { get; set; } = 10500;
        public TimeSpan MilisecondsCounter { get; set; } = TimeSpan.Zero;
        public MovementBehaviour Position { get; set; }
        public bool Trigger { get; set; }
        public string Id { get; set; }
    }
}
