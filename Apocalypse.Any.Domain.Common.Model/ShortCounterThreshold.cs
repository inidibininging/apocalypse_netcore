using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model
{
    /// <summary>
    /// A model for representing a counter
    /// </summary>
    public class ShortCounterThreshold
    {
        public byte Counter { get; set; }
        public byte Max { get; set; }
        public TimeSpan CooldownDeadline { get; set; }
        public bool MaxReached 
        { 
            get 
            {                
                return Counter >= Max;
            } 
        }
    }
}
