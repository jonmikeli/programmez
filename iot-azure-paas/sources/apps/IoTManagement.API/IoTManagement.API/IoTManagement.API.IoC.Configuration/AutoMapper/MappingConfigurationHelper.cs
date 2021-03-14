using AutoMapper;

using IoTManagement.API.IoC.Configuration.AutoMapper.Profiles;

namespace IoTManagement.API.IoC.Configuration.AutoMapper
{
    public static class MappingConfigurationsHelper
    {
        public static void ConfigureMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(APIMappingProfile));
                cfg.AddProfile(typeof(ServicesMappingProfile));
            });
        }
    }
}
