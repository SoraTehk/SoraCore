using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Text;
using Microsoft.Extensions.Logging;
using SoraTehk.Extensions;
using UnityEngine;
using ZLogger.Formatters;

namespace SoraTehk.Diagnostic {
    public class UnityAsyncLogProcessor : IAsyncLogProcessor {
        [ThreadStatic]
        private static ArrayBufferWriter<byte>? gBufferWriter;
        private readonly IZLoggerFormatter m_Formatter = new PlainTextZLoggerFormatter();
        
        [HideInCallstack]
        public void Post(IZLoggerEntry entry) {
            try {
                gBufferWriter ??= new ArrayBufferWriter<byte>();
                gBufferWriter.Clear();
                m_Formatter.FormatLogEntry(gBufferWriter, entry);
                // TODO: Directly integrate this into ZString.CreateUtf8StringBuilder
                var msg = Encoding.UTF8.GetString(gBufferWriter.WrittenSpan);
                
                // TODO: Frame value might not be correct (since this method being called async)
                int frame = Time.frameCount;
                Timestamp timeStamp = entry.LogInfo.Timestamp;
                string threadId = entry.LogInfo.ThreadInfo.ThreadId.ToString();
                
                object? ctx = entry.LogInfo.Context;
                UObject? uObjCtx = ctx as UObject;
                string ctxName = ctx switch {
                    Type t => $"S_{t.Name}",
                    null => "null",
                    _ => ctx.GetType().Name
                };
                string uObjId = uObjCtx?.GetInstanceID().ToString() ?? "";
                
                string memberName = entry.LogInfo.MemberName ?? "";
                
#if UNITY_EDITOR
                threadId = threadId.RTColorByHash();
                ctxName = ctxName.RTColorByHash();
                uObjId = uObjId.RTColorByHash();
#endif
                
                string formattedMsg;
                using (var sb = ZString.CreateUtf8StringBuilder(true)) {
                    sb.Append('[');
                    sb.Append(frame);
                    sb.Append('|');
                    sb.Append(timeStamp.Local.ToString("HH:mm:ss.fff"));
                    sb.Append('|');
                    sb.Append(threadId);
                    sb.Append(']');
                    if (!string.IsNullOrEmpty(ctxName)) {
                        sb.Append('[');
                        sb.Append(ctxName);
                        if (!string.IsNullOrEmpty(uObjId)) {
                            sb.Append('(');
                            sb.Append(uObjId);
                            sb.Append(')');
                        }
                        sb.Append(']');
                    }
                    if (!string.IsNullOrEmpty(memberName)) {
                        sb.Append('[');
                        sb.Append(memberName);
                        sb.Append(']');
                    }
                    sb.Append(msg);
                    formattedMsg = sb.ToString();
                }
                
                switch (entry.LogInfo.LogLevel) {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                    case LogLevel.Information:
                        Debug.Log(formattedMsg, uObjCtx);
                        break;
                    case LogLevel.Warning:
                        Debug.LogWarning(formattedMsg, uObjCtx);
                        break;
                    case LogLevel.Error:
                    case LogLevel.Critical:
                        Debug.LogError(formattedMsg, uObjCtx);
                        break;
                    case LogLevel.None:
                    default:
                        break;
                }
            }
            finally {
                entry.Return();
            }
        }
        
        public ValueTask DisposeAsync() => default;
    }
}