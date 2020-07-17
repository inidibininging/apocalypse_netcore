using Apocalypse.Any.Domain.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    public interface IGenericTypeDataLayer
    {
        List<Type> GetValidTypes();
        IEnumerable<T> DataAsEnumerable<T>();
        bool CanUse<T>(T item);
        void Add<T>(T item);
    }
}
