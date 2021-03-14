using AutoMapper;

using DCIoT = IoTManagement.API.API.DataContracts.IoT;
using SIoT = IoTManagement.API.Services.Model.IoT;

namespace IoTManagement.API.IoC.Configuration.AutoMapper.Profiles
{
    public class APIMappingProfile : Profile
    {
        public APIMappingProfile()
        {
            CreateMap<SIoT.Device, DCIoT.Device>().ReverseMap();
            CreateMap<SIoT.DeviceIoTSettings, DCIoT.DeviceIoTSettings>().ReverseMap();
            CreateMap<SIoT.Location, DCIoT.Location>().ReverseMap();
            CreateMap<SIoT.Tags, DCIoT.Tags>().ReverseMap();
            CreateMap<SIoT.Twins, DCIoT.Twins>().ReverseMap();
            CreateMap<SIoT.Properties, DCIoT.Properties>().ReverseMap();
        }
    }
}
