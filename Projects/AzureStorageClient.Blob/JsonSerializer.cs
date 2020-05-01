namespace AzureStorageClient
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    internal static class JsonSerializer
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Formatting = Formatting.Indented,
            DateFormatString = "o",
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string Serialize<T>(this T objectToSerialize)
            where T : class, new()
        {
            return JsonConvert.SerializeObject(objectToSerialize, JsonSerializerSettings);
        }

        public static T Deserialize<T>(this string contentToDeserialize)
            where T : class, new()
        {
            return JsonConvert.DeserializeObject<T>(contentToDeserialize, JsonSerializerSettings);
        }
    }
}
