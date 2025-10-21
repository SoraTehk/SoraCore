namespace SoraTehk.Extensions {
    public static partial class UObjectX {
        public static T? NullIfDestroyed<T>(this T uObj) where T : UObject {
            return uObj == null ? null : uObj;
        }
    }
}