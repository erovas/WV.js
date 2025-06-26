namespace WV.Win.Classes
{
    internal sealed class BytesCache
    {
        public byte[]? Bytes { get; set; } = null;

        public DateTime LastModified { get; set; } = DateTime.MinValue;

        public string ETag { get; set; } = string.Empty;
    }
}