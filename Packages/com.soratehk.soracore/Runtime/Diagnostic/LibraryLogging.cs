using Microsoft.Extensions.Logging;

namespace SoraTehk.Diagnostic {
    public static class LibraryLogging {
        public static readonly ILoggerFactory GFactory = LoggerFactory.Create(builder => {
            builder.AddZLoggerLogProcessor(new UnityAsyncLogProcessor());
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            builder.SetMinimumLevel(LogLevel.Trace);
#else
            builder.SetMinimumLevel(LogLevel.Error);
#endif
        });
        // TODO: Allow use to override with they own
        public static readonly ILogger GLogger = GFactory.CreateLogger($"SoraCore");
    }
}