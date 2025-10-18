using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using static SoraTehk.Diagnostic.LibraryLogging;

namespace VTBeat.Extensions {
    public static class UniTaskX {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForgetEx(this UniTask task, object ctx, [CallerMemberName] string? memberName = null) {
            task.Forget(ex => GLogger.ZLogError($"Exception: {ex}", ctx, memberName));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForgetEx<T>(this UniTask<T> task, object ctx, [CallerMemberName] string? memberName = null) {
            task.Forget(ex => GLogger.ZLogError($"Exception: {ex}", ctx, memberName));
        }
    }
}