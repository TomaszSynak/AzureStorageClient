namespace AzureStorageClient
{
    using System.Text;

    internal static class EncodingExtensions
    {
        public static byte[] Encode(this string stringToEncode, string srcEncoding = "iso-8859-2")
        {
            // ToDo: parametrize culture/encoding
            var encoding = Encoding.GetEncoding(srcEncoding);
            var bytes = encoding.GetBytes(stringToEncode);
            return Encoding.Convert(encoding, Encoding.UTF8, bytes);
        }

        public static string Decode(this byte[] bytesToDecode, string dstEncoding = "iso-8859-2")
        {
            // ToDo: parametrize culture/encoding
            var encoding = Encoding.GetEncoding(dstEncoding);
            var bytes = Encoding.Convert(Encoding.UTF8, encoding, bytesToDecode);
            return encoding.GetString(bytes);
        }
    }
}
