using Apocalypse.Any.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class DynamicRelationFactoryArgument<T1,T2>
        where T1 : IIdentifiableModel
        where T2 : IIdentifiableModel
    {
        public string Entity1Id { get; }
        public string Entity2Id { get; }
        public DynamicRelationFactoryArgument(T1 entity1, T2 entity2)
        {
            if (entity1 == null)
                throw new ArgumentNullException(nameof(entity1));
            if (entity2 == null)
                throw new ArgumentNullException(nameof(entity2));
            Entity1Id = entity1.Id;
            Entity2Id = entity2.Id;
        }


    }
}

