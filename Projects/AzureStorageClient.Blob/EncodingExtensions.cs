namespace AzureStorageClient
{
    using System.Text;

    internal static class EncodingExtensions
    {
        public static byte[] Encode(this string stringToEncode, string srcEncoding = null)
        {
            // ToDo: parametrize culture/encoding
            if (string.IsNullOrWhiteSpace(srcEncoding))
            {
                return Encoding.UTF8.GetBytes(stringToEncode);
            }

            var encoding = Encoding.GetEncoding(srcEncoding);
            var bytes = encoding.GetBytes(stringToEncode);
            return Encoding.Convert(encoding, Encoding.UTF8, bytes);
        }

        public static string Decode(this byte[] bytesToDecode, string dstEncoding = null)
        {
            if (string.IsNullOrWhiteSpace(dstEncoding))
            {
                return Encoding.UTF8.GetString(bytesToDecode);
            }

            // ToDo: parametrize culture/encoding
            var encoding = Encoding.GetEncoding(dstEncoding);
            var bytes = Encoding.Convert(Encoding.UTF8, encoding, bytesToDecode);
            return encoding.GetString(bytes);
        }
    }
}
