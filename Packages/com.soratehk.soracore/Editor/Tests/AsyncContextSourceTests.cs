using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using SoraTehk.Threading.Context;

namespace SoraTehk.Tests {
    public class AsyncContextSourceTests {
        private const float TOLERATE_DELTA_FLOAT = 0.0001f;
        
        #region Helpers
        private static async Task DoWorkAsync(AsyncContext ctx, float[] percentSteps, Func<AsyncContext, UniTask>? finishAction = null) {
            foreach (var p in percentSteps) {
                ctx.Report(new ProgressData(p));
                await UniTask.Yield();
            }
            
            if (finishAction != null) await finishAction(ctx);
        }
        private static async Task DoNestedWorkAsync(AsyncContext ctx, float[][] percentLevels, Func<AsyncContext, UniTask>? lastDepthAction = null) {
            if (percentLevels.Length == 0) {
                ctx.Complete();
                return;
            }
            
            if (percentLevels.Length == 1) {
                // Last depth
                await DoWorkAsync(ctx, percentLevels[0], lastDepthAction);
            }
            else {
                // Current depth
                await DoWorkAsync(ctx, percentLevels[0]);
            }
            // Next depth
            await DoNestedWorkAsync(ctx.New(1), percentLevels[1..], lastDepthAction);
        }
        #endregion
        
        [Test]
        public async Task CancellationPropagation_Depth5Throw() {
            bool isCancelled = false;
            try {
                using var cts = new CancellationTokenSource();
                using var src = new AsyncContextSource(cts.Token);
                
                cts.CancelAfterSlim(500);
                
                await DoNestedWorkAsync(src.New(1), new[] {
                        new[] { 0.1f },
                        new[] { 0.2f },
                        new[] { 0.3f },
                        new[] { 0.4f },
                        new[] { 0.5f },
                    },
                    async ctx => {
                        await UniTask.WaitUntilCanceled(ctx.Token);
                        ctx.ThrowIfCancellationRequested();
                        ctx.Complete();
                    }
                );
            }
            catch (OperationCanceledException) {
                isCancelled = true;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
            Assert.That(isCancelled);
        }
        
        [Test]
        public async Task ProgressAggregation_Depth1_ArbitrarySteps() {
            var rng = new Random(nameof(ProgressAggregation_Depth1_ArbitrarySteps).GetHashCode());
            using var src = new AsyncContextSource();
            float finalPercent = -1f;
            src.OnReport += (p, _) => finalPercent = p;
            
            var percentArray = Enumerable
                .Range(0, 5)
                .Select(_ => (float)rng.NextDouble())
                .OrderBy(p => p)
                .ToArray();
            await DoWorkAsync(src.New(1), percentArray);
            
            Assert.That(finalPercent, Is.EqualTo(percentArray[^1]).Within(TOLERATE_DELTA_FLOAT), $"Final percent must be {percentArray[^1]}");
        }
        
        [Test]
        public async Task ProgressAggregation_Depth2_ArbitrarySteps() {
            var rng = new Random(nameof(ProgressAggregation_Depth2_ArbitrarySteps).GetHashCode());
            using var src = new AsyncContextSource();
            float finalPercent = -1f;
            src.OnReport += (p, _) => finalPercent = p;
            
            int rows = 2; // Depth
            
            float[][] percentLevels = Enumerable.Range(0, rows)
                .Select(_ => Enumerable.Range(0, rng.Next(1, 11))
                    .Select(_ => (float)rng.NextDouble())
                    .OrderBy(p => p)
                    .ToArray()
                )
                .ToArray();
            await DoNestedWorkAsync(src.New(1), percentLevels);
            
            float expectedFinalPercent = percentLevels
                .Select(level => level.Last())
                .Average();
            
            Assert.That(finalPercent, Is.EqualTo(expectedFinalPercent).Within(TOLERATE_DELTA_FLOAT), $"Final percent must be {expectedFinalPercent}");
        }
        
        [Test]
        public async Task ProgressAggregation_Depth9_ArbitrarySteps() {
            var rng = new Random(nameof(ProgressAggregation_Depth9_ArbitrarySteps).GetHashCode());
            using var src = new AsyncContextSource();
            float finalPercent = -1f;
            src.OnReport += (p, _) => finalPercent = p;
            
            int rows = 9; // Depth
            
            float[][] percentLevels = Enumerable.Range(0, rows)
                .Select(_ => Enumerable.Range(0, rng.Next(1, 11))
                    .Select(_ => (float)rng.NextDouble())
                    .OrderBy(p => p)
                    .ToArray()
                )
                .ToArray();
            await DoNestedWorkAsync(src.New(1), percentLevels);
            
            float expectedFinalPercent = percentLevels
                .Select(level => level.Last())
                .Average();
            
            Assert.That(finalPercent, Is.EqualTo(expectedFinalPercent).Within(TOLERATE_DELTA_FLOAT), $"Final percent must be {expectedFinalPercent}");
        }
    }
}