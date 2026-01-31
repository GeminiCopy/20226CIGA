using Newtonsoft.Json;

public static class JsonExtension
{
    public static string ToJson<T>(this T o) => JsonConvert.SerializeObject(o);
    public static T FromJson<T>(this string json) => JsonConvert.DeserializeObject<T>(json);
}