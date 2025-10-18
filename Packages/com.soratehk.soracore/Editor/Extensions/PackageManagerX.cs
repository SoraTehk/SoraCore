using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor.PackageManager.Requests;

namespace SoraTehk.Extensions {
    public static class PackageManagerX {
        public static async UniTask WaitForCompletion(this Request req, CancellationToken ct = default) {
            while (!req.IsCompleted && !ct.IsCancellationRequested) {
                await UniTask.Yield();
            }
            ct.ThrowIfCancellationRequested();
        }
    }
}