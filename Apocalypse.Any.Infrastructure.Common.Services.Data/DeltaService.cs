using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public abstract class DeltaService<T>
    {
        protected List<T> DeltaRecords { get; set; } = new List<T>();
        public int DeltaRecordBufferSize { get; set; } = 10;
        protected bool RecordDeltas { get; set; }
        public T Value { get; private set; }
        public T Before { get; private set; }
        public T Delta { get; private set; }

        public DeltaService(bool recordDeltas = false)
        {
            RecordDeltas = recordDeltas;
        }

        public virtual DeltaService<T> Update(T value)
        {
            if (RecordDeltas && DeltaRecordBufferSize > DeltaRecords.Count)
            {
                DeltaRecords.Add(Delta);
            }
            else
            {
                DeltaRecords.Clear();
            }

            if (Value == null)
                Value = value;
            Before = Value;
            Value = value;
            Delta = GetDelta(Before,Value);
            
            return this;
        }
        protected abstract T GetDelta(T before, T after);
        public abstract T GetMeanOfRecords();
        
    }
}
