using Newtonsoft.Json.Linq;

namespace SoraTehk.Diagnostic.NewtonsoftJsonAddons {
    public interface IAsJToken {
        public JToken ToJToken();
    }
}