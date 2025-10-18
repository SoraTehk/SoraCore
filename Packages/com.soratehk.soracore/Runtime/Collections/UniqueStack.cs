using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SoraTehk.Collections {
    // TODO: Nullable item, thread-safe
    public partial class UniqueStack<T> where T : notnull {
        private readonly List<T> m_List;
        private readonly HashSet<T> m_Set;
        
        public UniqueStack() : this(new List<T>()) { }
        public UniqueStack(IEnumerable<T> collection) {
            m_List = new List<T>();
            m_Set = new HashSet<T>();
            
            foreach (var item in collection) {
                Add(item);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T item) {
            Remove(item);
            Add(item);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop() {
            if (!TryPeek(out var item)) throw new InvalidOperationException("Stack is empty.");
            
            RemoveAt(0);
            return item;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPeek([NotNullWhen(true)] out T? item) {
            if (m_List.Count == 0) {
                item = default!;
                return false;
            }
            
            item = m_List[^1];
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPeek(int stackIndex, [NotNullWhen(true)] out T? item) {
            if (m_List.Count == 0) {
                item = default!;
                return false;
            }
            
            item = m_List[^(stackIndex - 1)];
            return true;
        }
        
        public int IndexOf(T item) {
            int listIndex = m_List.LastIndexOf(item);
            return listIndex < 0
                ? -1
                : m_List.Count - 1 - listIndex;
        }
        
        public void Insert(int stackIndex, T item) {
            if (stackIndex < 0 || stackIndex > Count) throw new ArgumentOutOfRangeException(nameof(stackIndex));
            Remove(item);
            int listIndex = m_List.Count - stackIndex;
            
            m_List.Insert(listIndex, item);
            m_Set.Add(item);
        }
        
        public void RemoveAt(int stackIndex) {
            if (stackIndex < 0 || stackIndex >= Count) throw new ArgumentOutOfRangeException(nameof(stackIndex));
            
            int listIndex = m_List.Count - 1 - stackIndex;
            T item = m_List[listIndex];
            m_List.RemoveAt(listIndex);
            m_Set.Remove(item);
        }
    }
    
    public partial class UniqueStack<T> : ICollection<T> {
        public int Count => m_List.Count;
        public bool IsReadOnly => false;
        
        public void Add(T item) {
            if (Contains(item)) return;
            
            m_List.Add(item);
            m_Set.Add(item);
        }
        
        public void Clear() {
            m_List.Clear();
            m_Set.Clear();
        }
        
        public bool Contains(T item) => m_Set.Contains(item);
        
        public void CopyTo(T[] array, int arrayIndex) {
            int j = 0;
            for (int i = m_List.Count - 1; i >= 0; i--) {
                array[arrayIndex + j] = m_List[i];
                j++;
            }
        }
        
        public bool Remove(T item) {
            if (!Contains(item)) {
                return false;
            }
            
            int listIndex = m_List.LastIndexOf(item);
            m_List.RemoveAt(listIndex);
            m_Set.Remove(item);
            return true;
        }
        
        public IEnumerator<T> GetEnumerator() {
            for (int i = m_List.Count - 1; i >= 0; i--) {
                yield return m_List[i];
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public partial class UniqueStack<T> : IReadOnlyCollection<T> { } // Shared with ICollection<T>
}