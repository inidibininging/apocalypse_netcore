using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model
{
    public class DialogNode : IIdentifiableModel
    {
        public string Id { get; set; }
        public string Content { get; set; }
        //public int FontSize { get; set; }
        public List<Tuple<string,string>> DialogIdContent { get; set; }
        public CharacterSheet Requirement { get; set; }
        public ImageData Portrait { get; set; }
        public string DynamicRelationId { get; set; }
    }
}
