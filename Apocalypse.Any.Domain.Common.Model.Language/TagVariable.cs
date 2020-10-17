using System;
using Apocalypse.Any.Core.Model;

namespace Apocalypse.Any.Domain.Common.Model.Language
{
    public class TagVariable : Variable
    {
        public string Value { get; set; }
        public LexiconSymbol DataTypeSymbol { get; set; }

    }
}