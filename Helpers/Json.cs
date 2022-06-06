using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PC_Client.Helpers
{
    public static class Json
    {
        public static object? ToObject(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public static string Stringify(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
