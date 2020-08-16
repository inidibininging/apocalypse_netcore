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

        protected bool CheckTypeOfWith(Type subjectType, Type toProofAgainst)
        {
            if (subjectType == null || toProofAgainst == null)
                return false;
            var tparam = subjectType;
            var tparamInstance = toProofAgainst;
            if (tparam == tparamInstance)
                return true;
            if (tparam.IsAssignableFrom(tparamInstance))
                return true;
            if (tparamInstance.IsAssignableFrom(tparam))
                return true;
            return false;
        }

        protected bool CheckTypeOfToProofAgainstWith<TParamToProofAgainst>(Type subjectType)
        {
            if (subjectType == null)
                return false;
            var tparam = subjectType;
            var tparamInstance = typeof(TParamToProofAgainst);
            if (tparam == tparamInstance)
                return true;
            if (tparam.IsAssignableFrom(tparamInstance))
                return true;
            if (tparamInstance.IsAssignableFrom(tparam))
                return true;
            return false;
        }
        protected bool CheckTypeOfSubjectWithType<TParamSubject>(Type toProofAgainst)
        {
            if (toProofAgainst == null)
                return false;
            var tparam = typeof(TParamSubject);
            var tparamInstance = toProofAgainst;
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