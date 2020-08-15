using Apocalypse.Any.Domain.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    /// <summary>
    /// This interface controls and hides a layer of instances of any type.
    /// The instances that can be manipulated (CRUD) can be used with all items of GetValidTypes
    /// </summary>
    public interface IGenericTypeDataLayer
    {
        /// <summary>
        /// This can be seen as an alias for the class
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Returns which type can be used for manipulation (CRUD)
        /// </summary>
        /// <returns></returns>
        List<Type> GetValidTypes();

        /// <summary>
        /// Returns all the items found of the type provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> DataAsEnumerable<T>();

        /// <summary>
        /// Checks if the type and instance provided can be used by the interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CanUse<T>(T item);

        /// <summary>
        /// Adds an item of type T if possible
        /// </summary>
        /// <typeparam name="T">The type of the item to insert, if possible</typeparam>
        /// <param name="item">The item to insert</param>
        void Add<T>(T item);

        /// <summary>
        /// Removes any instance of type T that matches the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Remove<T>(Func<T, bool> predicate);
    }
}
