namespace SoraTehk.Threading.Context {
    public readonly struct ProgressData {
        public ProgressData(float percent, string? message = null) {
            Percent = percent;
            Message = message;
        }
        public float Percent { get; init; }
        public string? Message { get; init; }
    }
}