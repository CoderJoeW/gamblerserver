using MySqlX.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GamblerServerCrossPlatform.Models;

namespace GamblerServerCrossPlatform.Helpers
{
    public static class Lib
    {
        public static string ToJSON(object model)
        {
            string json = JsonConvert.SerializeObject(model);

            return json;
        }

        public static T FromJSON<T>(string json)
        {
            T model = JsonConvert.DeserializeObject<T>(json);

            return (T)Convert.ChangeType(model, typeof(T));
        }
    }
}
