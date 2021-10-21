using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sora.Extension;

using Debug = UnityEngine.Debug;

namespace Sora {
    [CreateAssetMenu(fileName = "Benchmark", menuName = "Sora/Benchmark")]
    public class BenchmarkSO : ScriptableObject
    {
        public Object object1;
        public Object object2;
        [Range(1, 100000)]
        public int iterate = 1;
        [Range(1, 100)]
        public int executingPerClick = 1;

        Animator spriteAnimator;

        [Space]
        [Header("◀GENERATING FIELDS▶")]
        public bool isEqual;
        public List<float> executedTimeA;
        public float averageA;

        public List<float> executedTimeB;
        public float averageB;


        public async void Execute() {
            Stopwatch stopwatch = new Stopwatch();

            //Generate a random float list
            List<float> inputList = new List<float>();
            int capacity = 20;
            float range = 3f;
            for(int i = 0; i < capacity; i++) {
                inputList.Add(((i+1)/capacity*i)*range);
            }
            inputList.Shuffle();

            //Decreasing all index one by one
            for (int i = 0; i < executingPerClick; i++) {
                stopwatch.Restart();

                //Clone list
                var testAList = new List<float>(inputList);
                float fakeTime = 0;
                while(testAList.Count > 0) {
                    for(int j = testAList.Count - 1; j >= 0; j--) {
                        if(fakeTime > testAList[j]) {
                            testAList.Remove(testAList[j]);
                        }
                    }

                    fakeTime += 0.1f;
                    await Task.Yield();
                }

                stopwatch.Stop();
                executedTimeA.Add(stopwatch.ElapsedMilliseconds);

                await Task.Yield();
            }
            averageA = executedTimeA.Average();

            //Sort & only process the close one
            for (int i = 0; i < executingPerClick; i++) {
                stopwatch.Restart();



                stopwatch.Stop();
                executedTimeB.Add(stopwatch.ElapsedMilliseconds);
                await Task.Yield();
            }
            averageB = executedTimeB.Average();
        }

        public void Clear() {
            isEqual = false;
            executedTimeA.Clear();
            averageA = 0;
        }

        void Reset() {
            object1 = null;
            object2 = null;
            iterate = 0;
            executingPerClick = 0;

            isEqual = false;
            executedTimeA.Clear();
            averageA = 0;
        }
    }
}

