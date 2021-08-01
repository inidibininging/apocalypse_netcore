using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class DynamicRelationFactory<T1,T2> : CheckWithReflectionFactoryBase<DynamicRelation>
        where T1 : IIdentifiableModel
        where T2 : IIdentifiableModel
    {
        public override bool CanUse<TParam>(TParam instance) => CanUseByTType<TParam, DynamicRelationFactoryArgument<T1, T2>>();

        public override List<Type> GetValidParameterTypes() => new List<Type>() { typeof(DynamicRelationFactoryArgument<T1, T2>) };

        protected override DynamicRelation UseConverter<TParam>(TParam parameter)
        {
            var relation = parameter as DynamicRelationFactoryArgument<T1, T2> ?? throw new ArgumentNullException(nameof(parameter));
            return new DynamicRelation()
            {
                Id = Guid.NewGuid().ToString(),
                Entity1Id = relation.Entity1Id,
                Entity1 = typeof(T1),
                Entity2Id = relation.Entity2Id,
                Entity2 = typeof(T2)
            };
        }
    }
}
