using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    public interface IGenericTypeFactory<T> 
    {
        List<Type> GetValidParameterTypes();

        bool CanUse<TParam>(TParam parameter);

        T Create<TParam>(TParam parameter);
    }
}