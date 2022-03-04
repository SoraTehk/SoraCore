namespace SoraCore.Collections {
    using System.Collections;
    using System.Collections.Generic;

    // UNDONE: UniqueQueue<T>
    public class UniqueStack<T> : IEnumerable<T> {
        private HashSet<T> hashSet;
        private Stack<T> stack;

        public UniqueStack() {
            hashSet = new HashSet<T>();
            stack = new Stack<T>();
        }

        public int Count => hashSet.Count;
        public void Clear() {
            hashSet.Clear();
            stack.Clear();
        }
        public bool Contains(T item) {
            return hashSet.Contains(item);
        }


        public void Push(T item) {
            if (!hashSet.Add(item)) return;
            stack.Push(item);
        }
        public T Pop() {
            T item = stack.Pop();
            hashSet.Remove(item);
            return item;
        }
        public T Peek() => stack.Peek();

        public IEnumerator<T> GetEnumerator() => stack.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => stack.GetEnumerator();
    }
}
