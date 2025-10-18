using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using SoraTehk.Threading.Context;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SoraTehk.Extensions {
    public static class AsyncOperationHandleX {
        public struct ReleaseScope : IDisposable {
            private AsyncOperationHandle m_Handle;
            public object Result => m_Handle.Result;
            
            public ReleaseScope(AsyncOperationHandle handle) => m_Handle = handle;
            public async UniTask WithAsyncContext(AsyncContext asyncCtx) {
                while (!m_Handle.IsDone && !asyncCtx.IsCancellationRequested) {
                    asyncCtx.Report(new ProgressData(m_Handle.PercentComplete));
                    await UniTask.Yield();
                }
                if (asyncCtx.IsCancellationRequested) {
                    m_Handle.Release();
                    asyncCtx.ThrowIfCancellationRequested();
                }
                asyncCtx.Complete();
            }
            public void Dispose() {
                if (!m_Handle.IsValid()) return;
                
                m_Handle.Release();
            }
        }
        
        public struct ReleaseScope<T> : IDisposable {
            private AsyncOperationHandle<T> m_Handle;
            public T Result => m_Handle.Result;
            
            public ReleaseScope(AsyncOperationHandle<T> handle) => m_Handle = handle;
            public async UniTask WithAsyncContext(AsyncContext asyncCtx) {
                while (!m_Handle.IsDone && !asyncCtx.IsCancellationRequested) {
                    asyncCtx.Report(new ProgressData(m_Handle.PercentComplete));
                    await UniTask.Yield();
                }
                if (asyncCtx.IsCancellationRequested) {
                    m_Handle.Release();
                    asyncCtx.ThrowIfCancellationRequested();
                }
                asyncCtx.Complete();
            }
            public void Dispose() {
                if (!m_Handle.IsValid()) return;
                
                m_Handle.Release();
            }
        }
        
        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReleaseScope AsReleaseScope(this AsyncOperationHandle hdl) => new(hdl);
        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReleaseScope<T> AsReleaseScope<T>(this AsyncOperationHandle<T> hdl) => new(hdl);
    }
}