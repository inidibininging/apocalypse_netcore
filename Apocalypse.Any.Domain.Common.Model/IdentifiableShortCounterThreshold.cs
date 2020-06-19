using Apocalypse.Any.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model
{
    public class IdentifiableShortCounterThreshold : ShortCounterThreshold ,IIdentifiableModel
    {
        public string Id { get; set; }

    }
}
