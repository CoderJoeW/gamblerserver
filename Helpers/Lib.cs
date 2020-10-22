using MySqlX.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Better_Server.Helpers
{
    public static class Lib
    {
        public static string ToJSON(object model)
        {
            string json = JsonConvert.SerializeObject(model);

            return json;
        }

        public static object FromJSON(string json)
        {
            object model = JsonConvert.DeserializeObject(json);

            return model;
        }
    }
}
