using UnityEngine;

namespace SoraCore {
    public abstract class BenchmarkableMono : MonoBehaviour, IBenchmarkable {
        public abstract void Action();
    }
}
