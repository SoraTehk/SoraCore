namespace SoraCore.Collections {
    using System.Collections;
    using System.Collections.Generic;

    // UNDONE: UniqueLinkedList<T>
    public class UniqueLinkedList<T> : IEnumerable<T> {
        private HashSet<T> hashSet;
        private LinkedList<T> linkedList;

        public int Count => hashSet.Count;
        public LinkedListNode<T> First => linkedList.First;
        public LinkedListNode<T> Last => linkedList.Last;

        public UniqueLinkedList() {
            hashSet = new HashSet<T>();
            linkedList = new LinkedList<T>();
        }

        public LinkedListNode<T> AddFirst(T item) => hashSet.Add(item) ? linkedList.AddFirst(item) : null;
        public LinkedListNode<T> AddLast(T item) => hashSet.Add(item) ? linkedList.AddLast(item) : null;
        public void RemoveFirst() { if (hashSet.Remove(linkedList.First.Value)) linkedList.RemoveFirst(); }
        public void RemoveLast() { if (hashSet.Remove(linkedList.Last.Value)) linkedList.RemoveLast(); }
        public T ConsumeFirst() { 
            T item = linkedList.First.Value;

            hashSet.Remove(item);
            linkedList.RemoveFirst();

            return item;
        }

        public T ConsumeLast() {
            T item = linkedList.Last.Value;

            hashSet.Remove(item);
            linkedList.RemoveLast();

            return item;
        }


        public void Clear() {
            hashSet.Clear();
            linkedList.Clear();
        }
        public bool Contains(T item) {
            return hashSet.Contains(item);
        }

        public IEnumerator<T> GetEnumerator() => linkedList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => linkedList.GetEnumerator();
    }
}
