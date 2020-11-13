namespace AzureStorageClient
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    internal static class Serializer
    {
        /// <summary>
        /// You should not override the Newtonsoft.Json ContractResolver with CamelCasePropertyNamesContractResolver for Json Serialization.
        /// Newtonsoft.Json by default respects the casing used in property/field names which is typically PascalCase.
        /// This can be overriden to serialize the names to camelCase and blob client will store the JSON in the storage as specified by the Newtonsoft.Json settings.
        /// https://martendb.io/documentation/documents/json/newtonsoft/
        /// </summary>
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Formatting = Formatting.Indented,
            DateFormatString = "o",
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string Serialize<T>(this T objectToSerialize)
            where T : class
        {
            return JsonConvert.SerializeObject(objectToSerialize, JsonSerializerSettings);
        }

        public static T Deserialize<T>(this string contentToDeserialize)
            where T : class
        {
            return JsonConvert.DeserializeObject<T>(contentToDeserialize, JsonSerializerSettings);
        }
    }
}
