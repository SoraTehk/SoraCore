using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Freya;
using SoraTehk.Extensions;
using static SoraTehk.Diagnostic.LibraryLogging;

namespace SoraTehk.Threading.Context {
    public partial class AsyncContextSource {
        public CancellationToken Token {
            get {
                lock (m_MasterCtsLock) {
                    return m_MasterCts.Token;
                }
            }
        }
        public event Action<float, string?>? OnReport;
        
        private CancellationTokenSource m_MasterCts;
        private readonly object m_MasterCtsLock = new();
        private readonly ConcurrentDictionary<CancellationToken, int> m_CancellationTokenToExistCount = new();
        
        private AtomicInt m_AsyncContextTotalWeight;
        private AtomicFloat m_TotalPercent;
        private readonly ConcurrentDictionary<AsyncContext, (int weight, float percent)> m_ContextToMetrics = new();
        
        public AsyncContextSource(in CancellationToken ct = default) {
            m_MasterCts = new CancellationTokenSource();
            LinkToken(ct);
        }
        
        #region New/Remove
        public AsyncContext New(int weight) {
            var asyncCtx = new AsyncContext(this);
            if (!m_ContextToMetrics.TryAdd(asyncCtx, (weight, 0f))) {
                GLogger.ZLogWarning($"Failed: Can't add asyncCtx.GetHashCode()={asyncCtx.GetHashCode()} to {nameof(m_ContextToMetrics)}", this);
            }
            else {
                m_AsyncContextTotalWeight.Add(weight);
                RebuildTotalPercent();
            }
            return asyncCtx;
        }
        internal void Remove(in AsyncContext asyncCtx) {
            if (!m_ContextToMetrics.TryRemove(asyncCtx, out var tup)) {
                GLogger.ZLogWarning($"Failed: Can't remove asyncCtx.GetHashCode()={asyncCtx.GetHashCode()} from {nameof(m_ContextToMetrics)}", this);
            }
            else {
                m_AsyncContextTotalWeight.Add(-tup.weight);
                RebuildTotalPercent();
            }
        }
        #endregion
        
        #region Progress
        private void RebuildTotalPercent() {
            // TODO: Result could be staled (dictionary change while iterating)
            float newTotalPercent = 0;
            foreach (var kv in m_ContextToMetrics) {
                newTotalPercent += kv.Value.percent * kv.Value.weight;
            }
            m_TotalPercent.Exchange(newTotalPercent);
        }
        internal void Report(in AsyncContext asyncCtx, in ProgressData data) {
            if (!m_ContextToMetrics.TryGetValue(asyncCtx, out var oldTup)) {
                GLogger.ZLogWarning($"404: asyncCtx.GetHashCode()={asyncCtx.GetHashCode()} doesn't contained in {nameof(m_ContextToMetrics)}", this);
                return;
            }
            // Args
            (int oldWeight, float oldPercent) = oldTup;
            float percent = Mathfs.Clamp(data.Percent, 0f, 1f);
            if (!m_ContextToMetrics.TryUpdate(asyncCtx, (oldWeight, percent), oldTup)) {
                GLogger.ZLogWarning($"Failed: Can't update asyncCtx.GetHashCode()={asyncCtx.GetHashCode()} in {nameof(m_ContextToMetrics)}", this);
                return;
            }
            // Calculate actual percent
            float deltaWeightedPercent = (percent - oldPercent) * oldWeight;
            m_TotalPercent.Add(deltaWeightedPercent);
            
            // PREF: This > 0f check could be overcome
            float actualPercent = m_AsyncContextTotalWeight > 0f
                ? m_TotalPercent / m_AsyncContextTotalWeight
                : 0f;
            
            OnReport?.Invoke(actualPercent, data.Message);
        }
        #endregion
        
        #region CancellationToken
        public void LinkToken(CancellationToken ct) {
            m_CancellationTokenToExistCount.AddOrUpdate(ct, 1, (_, count) => count + 1);
            RebuildMasterCancellationTokenSource();
        }
        public void UnlinkToken(CancellationToken ct) {
            m_CancellationTokenToExistCount.UpdateOrRemove(ct, (_, count) => count - 1, (_, count) => count <= 0);
            RebuildMasterCancellationTokenSource();
        }
        private void RebuildMasterCancellationTokenSource() {
            lock (m_MasterCtsLock) {
                m_MasterCts.Dispose();
                m_MasterCts = m_CancellationTokenToExistCount.Count > 0
                    ? CancellationTokenSource.CreateLinkedTokenSource(m_CancellationTokenToExistCount.Keys.ToArray())
                    : new CancellationTokenSource();
            }
        }
        #endregion
    }
    
    public partial class AsyncContextSource : IDisposable {
        public void Dispose() {
            lock (m_MasterCtsLock) {
                m_MasterCts.Dispose();
            }
        }
    }
}