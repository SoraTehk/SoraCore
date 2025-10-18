using System.Runtime.CompilerServices;
using System.Threading;

namespace SoraTehk.Threading {
    public struct AtomicLong {
        private long m_Value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AtomicLong(long initialValue = 0) {
            m_Value = initialValue;
        }
        
        public long Value {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Volatile.Read(ref m_Value);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interlocked.Exchange(ref m_Value, value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Add(long delta) => Interlocked.Add(ref m_Value, delta);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Increment() => Interlocked.Increment(ref m_Value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Decrement() => Interlocked.Decrement(ref m_Value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Exchange(long newValue) => Interlocked.Exchange(ref m_Value, newValue);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CompareExchange(long newValue, long expected) => Interlocked.CompareExchange(ref m_Value, newValue, expected) == expected;
        
        public override string ToString() => Value.ToString();
        
        public static implicit operator long(AtomicLong value) => value.Value;
    }
}