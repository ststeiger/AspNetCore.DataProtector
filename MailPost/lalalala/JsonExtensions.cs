
namespace System
{


    public static class JsonExtensions
    {
        private static readonly System.Text.Json.JsonSerializerOptions _jsonOptions =
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true
            }
        ;
        

        public static string ToJson<T>(this T obj)
        {
            return System.Text.Json.JsonSerializer.Serialize<T>(obj, _jsonOptions);
        }


        public static T FromJson<T>(this string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }


        public static async System.Threading.Tasks.Task ToJsonAsync<T>(this T obj, System.IO.Stream stream)
        {
            await System.Text.Json.JsonSerializer.SerializeAsync(stream, obj, typeof(T), _jsonOptions);
        }


        public static async System.Threading.Tasks.Task<string> ToJsonAsync<T>(this T obj)
        {
            string ret = null;

            Microsoft.IO.RecyclableMemoryStreamManager streamManager = new Microsoft.IO.RecyclableMemoryStreamManager();


            using (System.IO.MemoryStream ms = streamManager.GetStream())
            {
                await System.Text.Json.JsonSerializer.SerializeAsync(ms, obj, typeof(T), _jsonOptions);
                ms.Position = 0;

                using (System.IO.TextReader sr = new System.IO.StreamReader(ms, System.Text.Encoding.UTF8))
                {
                    ret = await sr.ReadToEndAsync();
                }

            }

            return ret;
        }


        public static async System.Threading.Tasks.Task<T> FromJsonAsync<T>(this System.IO.Stream stream)
        {
            return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions);
        }
        
    }


}
