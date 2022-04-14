namespace SoraCore
{
    using System.Collections.Concurrent;
    using System.Text;

    /// <summary>
    /// Simple string builder pool with <see cref="ConcurrentBag{T}"/>
    /// </summary>
    public static class StringBuilderPool
    {

        const int INITIALIZE_SIZE = 1;
        const int CAPACITY = 256;
        static ConcurrentBag<StringBuilder> _poolBag;
        static StringBuilderPool() => Reset();

        /// <summary>
        /// Rebuild the pool from scratch
        /// </summary>
        public static void Reset()
        {
            _poolBag = new ConcurrentBag<StringBuilder>();
            for (int i = 0; i < INITIALIZE_SIZE; i++)
            {
                _poolBag.Add(new StringBuilder(CAPACITY));
            }
        }

        /// <summary>
        /// Get a <see cref="StringBuilder"/> from the pool
        /// </summary>
        public static StringBuilder Get()
        {
            if (!_poolBag.TryTake(out StringBuilder sb))
            {
                sb = new StringBuilder(CAPACITY);
            }

            return sb;
        }

        /// <summary>
        /// Return a <see cref="StringBuilder"/> to the pool
        /// </summary>
        public static void Return(StringBuilder sb)
        {
            sb.Clear();
            _poolBag.Add(sb);
        }
    }
}