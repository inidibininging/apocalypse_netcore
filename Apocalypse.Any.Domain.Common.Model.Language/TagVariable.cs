using System;
using Apocalypse.Any.Core.Model;

namespace Apocalypse.Any.Domain.Common.Model.Language
{
    /// <summary>
    /// Represents a variable of any type (not only Tags).
    /// </summary>
    public class TagVariable : Variable
    {
        public string Value { get; set; }
        public LexiconSymbol DataTypeSymbol { get; set; }

    }
}