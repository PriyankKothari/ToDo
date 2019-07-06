using Newtonsoft.Json;

namespace ToDo.Domain.Helpers
{
    public static class JsonHelper
    {
        public static string ObjectToJson<T>(this T @object) where T : class
        {
            return JsonConvert.SerializeObject(@object);
        }

        public static T JsonToObject<T>(this string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}