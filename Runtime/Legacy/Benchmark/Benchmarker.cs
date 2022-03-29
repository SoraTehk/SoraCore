using MyBox;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace SoraCore {
    public class Benchmarker : MonoBehaviour {
        public Benchmarkable benchmarkableA;
        public Benchmarkable benchmarkableB;
        [Range(1, 100000)]
        public int iterate = 1;
        [Range(1, 100)]
        public int executingPerClick = 1;

        [Space]
        [Separator("Generated Data")]
        public bool isEqual;
        public List<float> executedTimeA;
        public float averageA;

        public List<float> executedTimeB;
        public float averageB;

        [ButtonMethod]
        public void Execute() {
            Stopwatch stopwatch = new();

            // Benchmarkable A
            if (benchmarkableA) {
                for (int i = 0; i < executingPerClick; i++) {
                    stopwatch.Restart();
                    /// Code here

                    for (int j = 0; j < iterate; j++) benchmarkableA.Action();

                    /// Code here
                    stopwatch.Stop();
                    executedTimeA.Add(stopwatch.ElapsedMilliseconds);
                }
                averageA = executedTimeA.Average();
            }

            // Benchmarkable B
            if (benchmarkableB) {
                for (int i = 0; i < executingPerClick; i++) {
                    stopwatch.Restart();
                    /// Code here

                    for (int j = 0; j < iterate; j++) benchmarkableB.Action();

                    /// Code here
                    stopwatch.Stop();
                    executedTimeB.Add(stopwatch.ElapsedMilliseconds);
                }
                averageB = executedTimeB.Average();
            }
        }

        [ButtonMethod]
        public void Clear() {
            isEqual = false;
            executedTimeA.Clear();
            averageA = 0;
            executedTimeB.Clear();
            averageB = 0;
        }

        void Reset() {
            iterate = 1;
            executingPerClick = 1;

            Clear();
        }
    }

}