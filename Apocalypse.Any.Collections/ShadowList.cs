using System;
using System.Collections;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Collections
{
    public class ShadowList<T> : IList<T>
    {
        protected IList<T> Shadow { get; set; } = new List<T>();

        public T this[int index]
        {
            get
            {
                return Shadow[index];
            }

            set
            {
                Shadow[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return Shadow.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return Shadow.IsReadOnly;
            }
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}