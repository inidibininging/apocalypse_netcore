using Apocalypse.Any.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model
{
    public class IntBank : ICurrencyHolder<int> , ITagableEntity, IIdentifiableModel,IOwnable
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public List<string> Tags { get; set; }
        public string OwnerName { get; set; }
    }
}
