namespace SoraCore.Collections
{
    using System.Collections;
    using System.Collections.Generic;

    // UNDONE: UniqueStack<T>
    public class UniqueStack<T> : IEnumerable<T>
    {
        private readonly HashSet<T> _hashSet;
        private readonly Stack<T> _stack;

        public UniqueStack()
        {
            _hashSet = new HashSet<T>();
            _stack = new Stack<T>();
        }

        public int Count => _hashSet.Count;
        public void Clear()
        {
            _hashSet.Clear();
            _stack.Clear();
        }
        public bool Contains(T item)
        {
            return _hashSet.Contains(item);
        }


        public void Push(T item)
        {
            if (!_hashSet.Add(item)) return;
            _stack.Push(item);
        }
        public T Pop()
        {
            T item = _stack.Pop();
            _hashSet.Remove(item);
            return item;
        }
        public T Peek() => _stack.Peek();

        public IEnumerator<T> GetEnumerator() => _stack.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _stack.GetEnumerator();
    }
}
