using Apocalypse.Any.Domain.Common.Model.Network;
using System;

namespace Apocalypse.Any.Domain.Common.Model
{
    public class Projectile : IEntityWithImage, IDisplayableByName, IOwnable
    {
        public int Damage { get; set; }
        public string OwnerName { get; set; }
        public ImageData CurrentImage { get; set; }
        public bool Destroyed { get; set; }
        public string DisplayName { get; set; }
        public TimeSpan DecayTime { get; set; }
        public DateTime CreationTime { get; set; }
    }
}