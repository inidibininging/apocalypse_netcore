using Apocalypse.Any.Domain.Server.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Server.Model
{
    public abstract class CheckWithReflectionFactoryBase<T> : IGenericTypeFactory<T>
    //where T : class
    {        

        public abstract bool CanUse<TParam>(TParam instance);

        protected bool CanUseByTType<TParamSubject, TParamToProofAgainst>()
        {
            var tparam = typeof(TParamSubject);
            var tparamInstance = typeof(TParamToProofAgainst);
            if (tparam == tparamInstance)
                return true;
            if (tparam.IsAssignableFrom(tparamInstance))
                return true;
            if (tparamInstance.IsAssignableFrom(tparam))
                return true;
            return false;
        }

        public T Create<TParam>(TParam parameter)
        {
            if (!CanUse(parameter))
                return default(T);
            return UseConverter(parameter);
        }

        protected abstract T UseConverter<TParam>(TParam parameter);

        public abstract List<Type> GetValidParameterTypes();
        
    }
}