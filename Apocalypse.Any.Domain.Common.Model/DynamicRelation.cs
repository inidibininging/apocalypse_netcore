using Apocalypse.Any.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model
{
    /// <summary>
    /// Describes a relationship between two entities with reflection
    /// </summary>
    public class DynamicRelation : IIdentifiableModel
    {
        public string Id { get; set; }
        public Type Entity1 { get; set; }
        public string Entity1Id { get; set; }

        public Type Entity2 { get; set; }
        public string Entity2Id { get; set; }
    }

    ///// <summary>
    ///// Describes a relationship between two entities with generics
    ///// </summary>
    ///// <typeparam name="T1"></typeparam>
    ///// <typeparam name="T2"></typeparam>
    //public class DynamicRelation<T1, T2> : IIdentifiableModel
    //    where T1 : IIdentifiableModel
    //    where T2 : IIdentifiableModel
    //{
    //    public string Id { get; set; }
    //    public T1 Entity1 { get; set; }
    //    public T2 Entity2 { get; set; }
    //}
}
