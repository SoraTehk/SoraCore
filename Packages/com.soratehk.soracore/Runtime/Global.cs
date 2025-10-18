using Random = System.Random;

namespace SoraTehk {
    public static class Global {
        public static readonly Random GRandom = new();
    }
}

// Editor namespace to avoid using compile error
namespace UnityEditor {
    internal static class DummyNamespaceHolder { }
}

namespace UnityEditor.AddressableAssets {
    internal static class DummyNamespaceHolder { }
}

namespace UnityEditor.AddressableAssets.Settings {
    internal static class DummyNamespaceHolder { }
}

namespace SoraTehk.Prepare {
    internal static class DummyNamespaceHolder { }
}