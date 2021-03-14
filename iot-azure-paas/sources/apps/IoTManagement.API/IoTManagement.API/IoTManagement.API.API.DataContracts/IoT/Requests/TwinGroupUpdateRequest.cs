using System.ComponentModel.DataAnnotations;

namespace IoTManagement.API.API.DataContracts.IoT.Requests
{
    public class TwinGroupUpdateRequest
    {
        [Required]
        public string WhereConstraint { get; set; }

        public Properties TwinProperties { get; set; }

        public Tags TwinTags { get; set; }
    }
}
