using System;

namespace VTBeat.Attributes {
    [Flags]
    public enum PreloadStaticModes {
        NONE = 0,
        EDIT_MODE = 1 << 0,
        PLAY_MODE = 1 << 1,
        BOTH = EDIT_MODE | PLAY_MODE,
    }
    
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class PreloadStaticAttribute : Attribute {
        public Type? After { get; set; } = null;
        public Type? Before { get; set; } = null;
        public PreloadStaticModes Modes { get; set; } = PreloadStaticModes.BOTH;
    }
}