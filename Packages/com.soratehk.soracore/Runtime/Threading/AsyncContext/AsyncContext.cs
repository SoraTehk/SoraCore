using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Freya;
using SoraTehk.Interfaces;

namespace SoraTehk.Threading.Context {
    public readonly struct LinkTokenScope : IDisposable {
        private readonly AsyncContextSource m_Source;
        private readonly CancellationToken m_Ct;
        public LinkTokenScope(AsyncContextSource source, in CancellationToken ct) {
            m_Source = source;
            m_Ct = ct;
            m_Source.LinkToken(m_Ct);
        }
        public void Dispose() {
            m_Source?.UnlinkToken(m_Ct);
        }
    }
    
    [StructLayout(LayoutKind.Auto)]
    public readonly partial struct AsyncContext {
        public AsyncContextSource Source { get; }
        
        internal AsyncContext(AsyncContextSource source) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            
            Id = m_IdCounter.Increment();
            Source = source;
        }
        
        // TODO: Accounting for subweight
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AsyncContext New(int weight) {
            // TODO: This will not work for 'default' instance
            return Source.New(weight);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete() {
            // TODO: Cleanup to preserve memory
        }
        
        #region CancellationToken
        public CancellationToken Token => Source?.Token ?? CancellationToken.None;
        public bool IsCancellationRequested => Source?.Token.IsCancellationRequested ?? false;
        public void ThrowIfCancellationRequested() => Source?.Token.ThrowIfCancellationRequested();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LinkTokenScope LinkToken(in CancellationToken ct) {
            // TODO: This will not work for 'default' instance
            return new LinkTokenScope(Source, ct);
        }
        public void UnlinkToken(in CancellationToken ct) {
            // TODO: This will not work for 'default' instance
            Source.UnlinkToken(ct);
        }
        #endregion
    }
    
    public partial struct AsyncContext : IProgress<float> {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Report(float percent) {
            Source.Report(this, Mathfs.Clamp01(percent));
        }
    }
    
    public partial struct AsyncContext : IHasIdentity {
        private static AtomicLong m_IdCounter;
        public long Id { get; }
    }
    
    public partial struct AsyncContext : IEquatable<AsyncContext> {
        public bool Equals(AsyncContext other) => Id == other.Id;
        public override bool Equals(object? obj) => obj is AsyncContext other && Equals(other);
        public override int GetHashCode() => Id.GetHashCode();
    }
}