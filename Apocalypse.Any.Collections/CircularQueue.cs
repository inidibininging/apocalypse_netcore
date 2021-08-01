using System;
using System.Collections;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Collections
{
    public class CircularQueue<T> : IEnumerable<T>
    {
        private List<T> ShadowList { get; set; }
        private int CursorPosition { get; set; }

        public CircularQueue()
        {
            ShadowList = new List<T>();
        }

        public void Add(T item)
        {
            ShadowList.Add(item);
        }

        public void Remove(T item)
        {
            ShadowList.Remove(item);
        }

        public void RemoveAt(int index) => ShadowList.RemoveAt(index);

        public int Count => ShadowList.Count;

        public T MoveNext()
        {
            if (ShadowList.Count == 0)
                return default(T);
            MoveCursor();
            return ShadowList[CursorPosition];
        }

        private void MoveCursor()
        {
            var nextIndex = CursorPosition + 1; // TODO: Circular List
            SetCursor(nextIndex > ShadowList.Count - 1 ? 0 : nextIndex);
        }

        public void SetCursor(int position)
        {
            if (position >= ShadowList.Count)
                throw new IndexOutOfRangeException();
            CursorPosition = position;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ShadowList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ShadowList.GetEnumerator();
        }
    }
}