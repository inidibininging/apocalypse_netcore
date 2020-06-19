using System.Collections.Generic;
using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Web.Admin.Models
{
    public class CharacterEntityListViewModel<T>
    where T : CharacterEntity
    {
        public string Tag { get; set; }
        public string Name { get; set; } = typeof(T).FullName;
        public List<T> Entities { get; set; }
    }
}