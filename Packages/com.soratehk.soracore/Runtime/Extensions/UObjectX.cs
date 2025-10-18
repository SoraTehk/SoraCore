namespace SoraTehk.Extensions {
    public static partial class UObjectX {
        public static T DefaultIfDestroyed<T>(this T uObj) where T : UObject {
            return uObj == null ? null : uObj;
        }
    }
}