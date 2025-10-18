using System.Runtime.CompilerServices;
using System.Threading;

namespace SoraTehk.Extensions {
    public static partial class CancellationTokenSourceX {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CancellationToken RestartAndGetToken(ref CancellationTokenSource? toBeCanceledCts) {
            toBeCanceledCts?.Cancel();
            toBeCanceledCts?.Dispose();
            toBeCanceledCts = new CancellationTokenSource();
            return toBeCanceledCts.Token;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CancellationToken RestartAndGetLinkedToken(ref CancellationTokenSource? toBeCanceledCts, CancellationToken targetCt) {
            toBeCanceledCts?.Cancel();
            toBeCanceledCts?.Dispose();
            toBeCanceledCts = new CancellationTokenSource();
            return CancellationTokenSource.CreateLinkedTokenSource(toBeCanceledCts.Token, targetCt).Token;
        }
    }
}