using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    /// <summary>
    /// Creates an ownable bank. The parameter for creating is the owner's name
    /// </summary>
    public class IntBankFactory : CheckWithReflectionFactoryBase<IntBank>
    {
        public override bool CanUse<TParam>(TParam instance) => CanUseByTType<TParam, string>() && !string.IsNullOrWhiteSpace((instance as string));        

        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(string) };
        }

        protected override IntBank UseConverter<TParam>(TParam parameter)
        {
            return new IntBank()
            {
                Id = Guid.NewGuid().ToString(),
                OwnerName = parameter as string,
                Tags = new List<string>()
            };
        }
    }
}
