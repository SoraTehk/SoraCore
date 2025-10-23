using System;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using SoraTehk.Threading.Context;

namespace SoraTehk.Extensions {
    public static class AsyncOperationHandleX {
        public struct AutoRelease : IDisposable {
            private AsyncOperationHandle m_Handle;
            public object Result => m_Handle.Result;
            
            public AutoRelease(AsyncOperationHandle handle) => m_Handle = handle;
            public async UniTask WithCancellation(AsyncContext asyncCtx = default) {
                while (!m_Handle.IsDone) {
                    asyncCtx.Report(m_Handle.PercentComplete);
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
        
        public struct AutoRelease<T> : IDisposable {
            private AsyncOperationHandle<T> m_Handle;
            public T Result => m_Handle.Result;
            
            public AutoRelease(AsyncOperationHandle<T> handle) => m_Handle = handle;
            public async UniTask WithCancellation(AsyncContext asyncCtx = default) {
                while (!m_Handle.IsDone) {
                    asyncCtx.Report(m_Handle.PercentComplete);
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
    }
}