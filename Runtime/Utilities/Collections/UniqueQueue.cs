namespace SoraCore.Collections
{
    using System.Collections;
    using System.Collections.Generic;

    // UNDONE: UniqueQueue<T>
    public class UniqueQueue<T> : IEnumerable<T>
    {
        private readonly HashSet<T> _hashSet;
        private readonly Queue<T> _queue;

        public UniqueQueue()
        {
            _hashSet = new HashSet<T>();
            _queue = new Queue<T>();
        }

        public int Count => _hashSet.Count;
        public void Clear()
        {
            _hashSet.Clear();
            _queue.Clear();
        }
        public bool Contains(T item)
        {
            return _hashSet.Contains(item);
        }


        public void Enqueue(T item)
        {
            if (!_hashSet.Add(item)) return;
            _queue.Enqueue(item);
        }
        public T Dequeue()
        {
            T item = _queue.Dequeue();
            _hashSet.Remove(item);
            return item;
        }
        public T Peek() => _queue.Peek();

        public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
    }
}
