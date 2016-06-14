using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StackExchange.Redis.Queue.Common
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Message
    {
        #region Properties
        public string SerializedContent { get; set; } 
        #endregion

        #region Methods
        #region C'tor
        public Message()
        {
        }

        public Message(object inputObject)
        {
            SerializedContent = SerializeObject(inputObject);
        }
        #endregion

        #region Public
        public static string SerializeObject(object inputObject)
        {
            var resultString = JsonConvert.SerializeObject(inputObject);

            return resultString;
        }

        public static TZ DeserializeObject<TZ>(string inputString)
        {
            var resultObject = JsonConvert.DeserializeObject<TZ>(inputString);

            return resultObject;
        }  
        #endregion
        #endregion
    }
}
