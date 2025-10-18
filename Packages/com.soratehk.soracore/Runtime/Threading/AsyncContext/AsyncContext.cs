using System;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using static SoraTehk.Diagnostic.LibraryLogging;

namespace SoraTehk.Threading.Context {
    public readonly struct TemporaryTokenScope : IDisposable {
        private readonly AsyncContextSource m_Source;
        private readonly CancellationToken m_Ct;
        public TemporaryTokenScope(AsyncContextSource source, in CancellationToken ct) {
            m_Source = source;
            m_Ct = ct;
            m_Source.LinkToken(m_Ct);
        }
        public void Dispose() {
            m_Source?.UnlinkToken(m_Ct);
        }
    }
    
    // TODO: Convert this into a struct somehow
    public partial class AsyncContext {
        public AsyncContextSource Source { get; }
        
        internal AsyncContext(AsyncContextSource source) {
            Source = source;
        }
        
        // TODO: Subweight calculation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AsyncContext New(int weight) => Source.New(weight);
        // TODO: Cleanup to preserve memory
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete() => Report(new ProgressData(1f));
        
        #region CancellationToken
        public CancellationToken Token => Source.Token;
        public bool IsCancellationRequested => Source.Token.IsCancellationRequested;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ThrowIfCancellationRequested() => Source.Token.ThrowIfCancellationRequested();
        
        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TemporaryTokenScope LinkTemporaryToken(MonoBehaviour mono) => new(Source, mono.destroyCancellationToken);
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public void LinkToken(CancellationToken ct) => Source.LinkToken(ct);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnlinkToken(CancellationToken ct) => Source.UnlinkToken(ct);
        #endregion
    }
    
    public partial class AsyncContext : IProgress<ProgressData> {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private float m_PrevPercent;
#endif
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Report(ProgressData data) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (data.Percent < m_PrevPercent) {
                GLogger.ZLogWarning($"Invalid: data.Percent={data.Percent} < m_PrevPercent={m_PrevPercent}", this);
            }
            m_PrevPercent = data.Percent;
#endif
            Source.Report(this, data);
        }
    }
}