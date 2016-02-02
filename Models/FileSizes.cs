namespace ANDREICSLIB.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class FileSizes
    {
        public long Bytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSizes"/> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public FileSizes(long bytes)
        {
            Bytes = bytes;
        }

        /// <summary>
        /// To the string bytes.
        /// </summary>
        /// <returns></returns>
        public string ToStringBytes()
        {
            var str = $"{Bytes}B";
            return str;
        }

        /// <summary>
        /// To the string kb.
        /// </summary>
        /// <returns></returns>
        public string ToStringKB()
        {
            var str = $"{Bytes/1024}KB";
            return str;
        }

        /// <summary>
        /// To the string mb.
        /// </summary>
        /// <returns></returns>
        public string ToStringMB()
        {
            var str = $"{Bytes/1048576}MB";
            return str;
        }
    }
}