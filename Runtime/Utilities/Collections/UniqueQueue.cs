namespace SoraCore.Collections {
    using System.Collections;
    using System.Collections.Generic;

    // UNDONE: UniqueQueue<T>
    public class UniqueQueue<T> : IEnumerable<T> {
        private HashSet<T> hashSet;
        private Queue<T> queue;

        public UniqueQueue() {
            hashSet = new HashSet<T>();
            queue = new Queue<T>();
        }

        public int Count => hashSet.Count;
        public void Clear() {
            hashSet.Clear();
            queue.Clear();
        }
        public bool Contains(T item) {
            return hashSet.Contains(item);
        }


        public void Enqueue(T item) {
            if (!hashSet.Add(item)) return;
            queue.Enqueue(item);
        }
        public T Dequeue() {
            T item = queue.Dequeue();
            hashSet.Remove(item);
            return item;
        }
        public T Peek() => queue.Peek();

        public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => queue.GetEnumerator();
    }
}
