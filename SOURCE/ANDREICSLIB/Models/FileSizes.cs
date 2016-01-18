namespace ANDREICSLIB.Models
{
    public class FileSizes
    {
        public long Bytes;

        public FileSizes(long bytes)
        {
            Bytes = bytes;
        }

        public string ToStringBytes()
        {
            var str = $"{Bytes}B";
            return str;
        }
        public string ToStringKB()
        {
            var str = $"{Bytes / 1024}KB";
            return str;
        }
        public string ToStringMB()
        {
            var str = $"{Bytes / 1048576}MB";
            return str;
        }
    }
}