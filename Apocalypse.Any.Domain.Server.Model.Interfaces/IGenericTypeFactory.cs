using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    /// <summary>
    /// This interface represents an instance that can create items of a type provided, if possible
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericTypeFactory<T> 
    {
        /// <summary>
        /// Gives all the types that can be used as parameters
        /// </summary>
        /// <returns>A list of valid types</returns>
        List<Type> GetValidParameterTypes();

        /// <summary>
        /// Checks if the parameter and the parameter type provided can be used for creating an instance of type T in the method "Create"
        /// </summary>
        /// <typeparam name="TParam">Parameter type</typeparam>
        /// <param name="parameter">parameter instace</param>
        /// <returns></returns>
        bool CanUse<TParam>(TParam parameter);

        /// <summary>
        /// Creates an item of type T, if possible
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        T Create<TParam>(TParam parameter);
    }
}