using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using VTBeat.Extensions;

namespace SoraTehk.Extensions {
    // ReSharper disable once InconsistentNaming
    public static partial class IDisposableX {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async UniTask DisposeIfNeededAsync(this object obj) {
            switch (obj) {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeIfNeeded(this object obj) {
            switch (obj) {
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
                case IAsyncDisposable asyncDisposable:
                    asyncDisposable.DisposeAsync().AsUniTask().ForgetEx(obj);
                    break;
            }
        }
    }
}