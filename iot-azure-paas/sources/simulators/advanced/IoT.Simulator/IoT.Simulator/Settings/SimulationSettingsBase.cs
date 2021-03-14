using Newtonsoft.Json;

namespace IoT.Simulator.Settings
{
    public class SimulationSettingsBase
    {
        [JsonProperty("enableTelemetryMessages")]
        public bool EnableTelemetryMessages { get; set; }
        [JsonProperty("TelemetryTimeInterval")]
        public int TelemetryTimeInterval { get; set; }

        [JsonProperty("enableC2DDirectMethods")]
        public bool EnableC2DDirectMethods { get; set; }

        [JsonProperty("enableC2DMessages")]
        public bool EnableC2DMessages { get; set; }

        [JsonProperty("enableTwinPropertiesDesiredChangesNotifications")]
        public bool EnableTwinPropertiesDesiredChangesNotifications { get; set; }
    }
}
