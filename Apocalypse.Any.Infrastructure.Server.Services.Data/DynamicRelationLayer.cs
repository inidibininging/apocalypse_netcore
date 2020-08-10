using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    /// <summary>
    /// Layer for accessing instances of DynamicRelation 
    /// The relation types of type 1 or type 2 can be represented either as 
    /// T1, T2 
    /// or 
    /// T2, T1
    /// the order of the type is not relevant. 
    /// This is important when checking relation types because the elements are unsorted
    /// </summary>
    /// <typeparam name="T1">entity type of relation</typeparam>
    /// <typeparam name="T2">entity type of relation</typeparam>
    public class DynamicRelationLayer<T1,T2> : CheckedWithReflectionGameStateDataLayer
        where T1 : class, IIdentifiableModel
        where T2 : class, IIdentifiableModel
    {
        private ConcurrentBag<DynamicRelation> InMemoryData { get; set; } = new ConcurrentBag<DynamicRelation>();

        public DynamicRelationLayer(string name) : base(name)
        {

        }
        public override bool CanUse<T>(T instance)
        {

            if (!CanUseByTType<T, DynamicRelation>() ||
                ((instance as DynamicRelation) == null))
                return false;
            var typeOfEntity1 = (instance as DynamicRelation).Entity1;
            var typeOfEntity2 = (instance as DynamicRelation).Entity2;
            if (typeOfEntity1 == null || typeOfEntity2 == null)
                return false;

            //check 1 against circular location and 2 against dialog node
            //check 1 against dialog node and 2 against circular
            return (CheckTypeOfSubjectWithType<T1>(typeOfEntity1) &&
                   CheckTypeOfSubjectWithType<T2>(typeOfEntity2)) ||
                   (CheckTypeOfSubjectWithType<T2>(typeOfEntity1) &&
                   CheckTypeOfSubjectWithType<T1>(typeOfEntity2));
        }

        public override List<Type> GetValidTypes()
        {            
            return new List<Type>() { typeof(DynamicRelation) };
        }

        protected override void AddSafe<T>(T item)
        {
            InMemoryData.Add(item as DynamicRelation);
        }

        protected override IEnumerable<T> AsEnumerableSafe<T>()
        {
            //return a cloned list because the types can be changed.
            //this is not safe enough but at least something. definitely a TODO
            return InMemoryData.Cast<T>().Where(relation => CanUse(relation));
        }

        protected override bool UpdateEnumerable<T>(IEnumerable<T> items)
        {
            var itemsAsList = items.OfType<DynamicRelation>().DefaultIfEmpty().ToList();
            //if (itemsAsList.Any())
            //    return false;
            InMemoryData = new ConcurrentBag<DynamicRelation>(itemsAsList);
            return true;
        }
    }

 
}
