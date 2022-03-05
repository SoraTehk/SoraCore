namespace SoraCore.Collections {
    using System.Collections;
    using System.Collections.Generic;

    // UNDONE: UniqueLinkedList<T>
    public class UniqueLinkedList<T> : IEnumerable<T> {
        private readonly HashSet<T> _hashSet = new();
        private readonly LinkedList<T> _linkedList = new();

        public int Count => _hashSet.Count;
        public LinkedListNode<T> First => _linkedList.First;
        public LinkedListNode<T> Last => _linkedList.Last;

        public LinkedListNode<T> AddFirst(T item) => _hashSet.Add(item) ? _linkedList.AddFirst(item) : null;
        public LinkedListNode<T> AddLast(T item) => _hashSet.Add(item) ? _linkedList.AddLast(item) : null;
        public void RemoveFirst() { if (_hashSet.Remove(_linkedList.First.Value)) _linkedList.RemoveFirst(); }
        public void RemoveLast() { if (_hashSet.Remove(_linkedList.Last.Value)) _linkedList.RemoveLast(); }
        public T ConsumeFirst() { 
            T item = _linkedList.First.Value;

            _hashSet.Remove(item);
            _linkedList.RemoveFirst();

            return item;
        }

        public T ConsumeLast() {
            T item = _linkedList.Last.Value;

            _hashSet.Remove(item);
            _linkedList.RemoveLast();

            return item;
        }


        public void Clear() {
            _hashSet.Clear();
            _linkedList.Clear();
        }
        public bool Contains(T item) {
            return _hashSet.Contains(item);
        }

        public IEnumerator<T> GetEnumerator() => _linkedList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _linkedList.GetEnumerator();
    }
}
