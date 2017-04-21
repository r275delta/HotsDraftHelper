namespace HotsDraftLib
{
    public struct Statistic
    {
        public double Value { get; }
        public int SampleSize { get; }

        public Statistic(double value, int sampleSize)
        {
            Value = value;
            SampleSize = sampleSize;
        }
    }
}
