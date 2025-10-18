namespace SoraTehk.Interfaces {
    public enum Status {
        DISPOSED = -2,
        DISPOSING = -1,
        NONE = 0,
        UNINITIALIZED,
        INITIALIZING,
        INITIALIZED,
        RUNNING,
        PAUSED,
    }
    
    public interface IHasStatus {
        public Status Status { get; }
    }
}