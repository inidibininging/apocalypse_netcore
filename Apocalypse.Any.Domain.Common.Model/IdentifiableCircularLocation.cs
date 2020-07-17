using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model
{
    public class IdentifiableCircularLocation : CircularLocation, IIdentifiableModel
    {
        public string Id { get ; set ; }
        public string Name { get; set; }
        
    }
}
