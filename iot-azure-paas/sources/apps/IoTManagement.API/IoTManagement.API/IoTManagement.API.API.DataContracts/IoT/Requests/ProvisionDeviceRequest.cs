using System.ComponentModel.DataAnnotations;

namespace IoTManagement.API.API.DataContracts.IoT.Requests
{
    public class ProvisionDeviceRequest
    {
        [Required]
        public string DeviceId { get; set; }

        public DeviceIoTSettings DeviceIoTSettings { get; set; }
    }
}
