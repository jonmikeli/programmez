using System;
using System.IO;
using System.Text;

using IoT.Simulator.Exceptions;

namespace IoT.Simulator.Tools
{
    public static class ConfigurationHelpers
    {
        public static void CheckEnvironmentConfigurationFiles(string environment)
        {
            if (string.IsNullOrEmpty(environment))
                throw new ArgumentNullException(nameof(environment));

            StringBuilder sb = new StringBuilder();

            var appsettings = $"appsettings.{environment}.json";
            if (!File.Exists(appsettings))
                sb.AppendLine($"{appsettings} not found.");

            var devicesettings = $"devicesettings.{environment}.json";
            if (!File.Exists(devicesettings))
                sb.AppendLine($"{devicesettings} not found.");

            if (sb.Length > 0)
                throw new MissingEnvironmentConfigurationFileException(sb.ToString());

        }
    }
}
