using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model
{
    public interface IEntityChange<TDataType, TIdentifierType>
    {
        TIdentifierType Identifier { get; }
        TDataType Data { get; }
    }
}
