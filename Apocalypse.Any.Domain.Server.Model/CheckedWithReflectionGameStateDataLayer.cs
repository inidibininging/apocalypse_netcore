﻿using Apocalypse.Any.Domain.Server.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Domain.Server.Model
{
    public abstract class CheckedWithReflectionGameStateDataLayer : IGenericTypeDataLayer
    {
        
        public abstract bool CanUse<T>(T instance);
        public abstract List<Type> GetValidTypes();

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

        public IEnumerable<T> DataAsEnumerable<T>()
        {
            if (!GetValidTypes().Any(t => CheckTypeOfToProofAgainstWith<T>(t)))
                return Array.Empty<T>();
            return AsEnumerableSafe<T>();
        }

        

        public void Add<T>(T item)
        {
            if (!CanUse(item))
                return;
            AddSafe(item);
        }

        protected abstract void AddSafe<T>(T item);
        protected abstract IEnumerable<T> AsEnumerableSafe<T>();
    }
}
