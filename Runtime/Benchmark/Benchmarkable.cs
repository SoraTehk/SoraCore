using UnityEngine;

namespace SoraCore {
    public abstract class Benchmarkable : MonoBehaviour, IBenchmarkable {
        public abstract void Action();
    }
}
