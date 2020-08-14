using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public abstract class DeltaService<T>
    {
        public T Value { get; private set; }
        public T Before { get; private set; }
        public T Delta { get; private set; }
        public virtual DeltaService<T> Update(T value)
        {
            Before = Value;
            Value = value;
            Delta = GetDelta(Before,Value);
            return this;
        }
        protected abstract T GetDelta(T before, T after);

    }
}
