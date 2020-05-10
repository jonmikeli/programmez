using IoT.Simulator.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace IoT.Simulator.Tools
{
    public class IoTTools
    {
        #region Tooling
        internal static void CheckDeviceConnectionStringData(string connectionString, ILogger logger)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "No IoT Hub connection string has been found");

            if (logger == null)
                throw new ArgumentNullException(nameof(logger), "No logger has been provided");

            string logPrefix = "system".BuildLogPrefix();

            //Hostname
            string hostname = connectionString.ExtractValue("HostName");
            if (string.IsNullOrEmpty(hostname))
                throw new ArgumentNullException("hostname", "No hostname has been found within the connection string");

            logger.LogDebug($"{logPrefix}::IoT Hub hostname: {hostname}");

            //DeviceId
            string deviceId = connectionString.ExtractValue("DeviceId");
            if (string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException("DeviceId", "No deviceId has been found within the connection string");

            logger.LogDebug($"{logPrefix}::DeviceId:{deviceId}::CheckDeviceConnectionStringData");
        }


        internal static string RandomizeData(string jsonMessage)
        {
            if (string.IsNullOrEmpty(jsonMessage))
                throw new ArgumentNullException(nameof(jsonMessage));

            JObject jobject = JObject.Parse(jsonMessage);

            if (jobject != null)
            {
                JToken jData;

                if (jobject.TryGetValue("data", out jData) && jData.Type == JTokenType.Array)
                {
                    Random r = new Random(DateTime.Now.Second);
                    foreach (var item in (JArray)jData)
                    {
                        item["timestamp"] = JValue.FromObject(DateTime.Now.TimeStamp());
                        item["propertyValue"] = JValue.FromObject(r.Next(150, 300));
                    }
                }

                return JsonConvert.SerializeObject(jobject, Formatting.Indented);
            }
            else return null;
        }
        #endregion
    }
}
