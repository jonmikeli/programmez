using Newtonsoft.Json;

namespace IoT.Simulator.Settings
{
    public class SimulationSettingsDevice : SimulationSettingsBase
    {
        [JsonProperty("enableDevice")]
        public bool EnableDevice { get; set; }
    }
}
