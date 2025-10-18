using System.Runtime.CompilerServices;
using System.Threading;

namespace SoraTehk.Threading {
    public struct AtomicInt {
        private int m_Value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AtomicInt(int initialValue = 0) {
            m_Value = initialValue;
        }
        
        public int Value {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Volatile.Read(ref m_Value);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interlocked.Exchange(ref m_Value, value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Add(int delta) => Interlocked.Add(ref m_Value, delta);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Increment() => Interlocked.Increment(ref m_Value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Decrement() => Interlocked.Decrement(ref m_Value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Exchange(int newValue) => Interlocked.Exchange(ref m_Value, newValue);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CompareExchange(int newValue, int expected) => Interlocked.CompareExchange(ref m_Value, newValue, expected) == expected;
        
        public override string ToString() => Value.ToString();
        
        public static implicit operator int(AtomicInt value) => value.Value;
    }
}