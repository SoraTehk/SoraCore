using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SoraTehk.Threading {
    public struct AtomicFloat {
        private int m_Value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AtomicFloat(float initialValue = 0f) {
            m_Value = BitConverter.SingleToInt32Bits(initialValue);
        }
        
        public float Value {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BitConverter.Int32BitsToSingle(Volatile.Read(ref m_Value));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interlocked.Exchange(ref m_Value, BitConverter.SingleToInt32Bits(value));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Add(float delta) {
            while (true) {
                int oldBits = Volatile.Read(ref m_Value);
                float oldValue = BitConverter.Int32BitsToSingle(oldBits);
                float newValue = oldValue + delta;
                int newBits = BitConverter.SingleToInt32Bits(newValue);
                if (Interlocked.CompareExchange(ref m_Value, newBits, oldBits) == oldBits) {
                    return newValue;
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Exchange(float newValue) {
            int newBits = BitConverter.SingleToInt32Bits(newValue);
            int oldBits = Interlocked.Exchange(ref m_Value, newBits);
            return BitConverter.Int32BitsToSingle(oldBits);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CompareExchange(float newValue, float expected) {
            int newBits = BitConverter.SingleToInt32Bits(newValue);
            int expectedBits = BitConverter.SingleToInt32Bits(expected);
            return Interlocked.CompareExchange(ref m_Value, newBits, expectedBits) == expectedBits;
        }
        
        public override string ToString() => Value.ToString(CultureInfo.CurrentCulture);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float(AtomicFloat value) => value.Value;
    }
}