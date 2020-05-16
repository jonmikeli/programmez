using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace IoT.Simulator.Services
{
    //https://dejanstojanovic.net/aspnet/2018/december/registering-multiple-implementations-of-the-same-interface-in-aspnet-core/
    public class SimpleTelemetryMessageService : ITelemetryMessageService
    {
        private ILogger _logger;
        private string fileTemplatePath = @"./Messages/data.json";

        public SimpleTelemetryMessageService(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<SimpleTelemetryMessageService>();
        }

        public async Task<string> GetMessageAsync()
        {
            string messageString = File.ReadAllText(fileTemplatePath);

            if (string.IsNullOrEmpty(messageString))
                throw new ArgumentNullException(nameof(messageString), "DATA: The message to send is empty or not found.");

            return messageString;
        }
    }
}
