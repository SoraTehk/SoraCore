namespace SoraTehk.Extensions {
    public static partial class MathX {
        public static double Clamp(double value, double min, double max) {
            return value < min ? min : value > max ? max : value;
        }
    }
}