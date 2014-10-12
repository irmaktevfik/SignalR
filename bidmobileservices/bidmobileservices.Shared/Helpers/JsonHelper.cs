using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZUMOAPPNAME.Helpers
{
    public class JsonHelper
    {
        public static T Deserialize<T>(string request)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(request);
        }

        public static string Serialize<T>(T request)
        {
            return JsonConvert.SerializeObject(request);
        }
    }
}
