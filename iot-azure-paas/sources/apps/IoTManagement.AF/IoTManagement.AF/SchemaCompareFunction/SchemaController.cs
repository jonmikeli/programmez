
using System;
using System.Collections.Generic;
using System.IO;

using Azure.Storage.Blobs;

using IoT.AzureFunctions.SchemaCompareFunction.Responses;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace IoT.AzureFunctions.SchemaCompareFunction
{
    public static class SchemaController
    {
        private static ILogger _logger;

        [FunctionName("SchemaValidator")]
        public static void Run(
            [EventHubTrigger("%afBindingInputEventHubName%", Connection = "AzureSettings:EventHub:Telemetry:ConnectionString")] JObject iotHubMessage,
            [EventHub("%afBindingOutputEventHubName%", Connection = "AzureSettings:EventHub:ValidatedTelemetry:ConnectionString")] out string outputEventHubMessage,
            [Blob("%afBindingValidatedTelemetryStoragePath%/{sys.utcnow}", Connection = "AzureSettings:Storage:Blob:ValidatedTemeletry:ConnectionString")] out string outputBlob,
            ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;

            string messageContent = iotHubMessage.ToString();
            _logger.LogDebug($"SCHEMA CONTROL for message: {messageContent}");

            outputEventHubMessage = null;
            outputBlob = null;

            try
            {
                if (iotHubMessage.TryGetValue("$schema", out JToken schemaPath))
                {
                    //Message schema control
                    string schemaContent = GetSchemaContent(schemaPath.Value<string>());

                    var result = CheckSchemaMessage(messageContent, schemaContent);

                    if (result != null && result.IsMessageValid)
                    {
                        _logger.LogDebug($"   >>>SCHEMA OK");
                        outputEventHubMessage = messageContent;
                        outputBlob = messageContent;
                    }
                    else
                    {
                        _logger.LogDebug($"   >>>SCHEMA KO");
                    }
                }
                else
                {
                    _logger.LogDebug($"   >>>NO SCHEMA PROVIDED.THE MESSAGE CANNOT BE ACCEPTED.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        #region Private methods
        /// <summary>
        /// Gets a blob from Azure Blob storage. Those blobs should contain JSON schemas.
        /// </summary>
        /// <param name="schemaContentPath"></param>
        /// <returns></returns>
        private static string GetSchemaContent(string schemaContentPath)
        {
            if (string.IsNullOrEmpty(schemaContentPath))
                throw new ArgumentNullException(nameof(schemaContentPath));

            string result = string.Empty;

            BlobClient blobClient = new BlobClient(new Uri(schemaContentPath));
            var schemaBlobDownloadResponse = blobClient.Download();

            if (schemaBlobDownloadResponse != null && schemaBlobDownloadResponse.Value != null)
            {
                using (var downLoadInfo = schemaBlobDownloadResponse.Value)
                using (var streamedContent = new StreamReader(downLoadInfo.Content))
                {
                    result = streamedContent.ReadToEnd();
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if a JSON message respects a given JSON Schema.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static SchemaValidationResponse CheckSchemaMessage(string message, string schema)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrEmpty(schema))
                throw new ArgumentNullException(nameof(schema));

            SchemaValidationResponse result = new SchemaValidationResponse { Message = message, IsMessageValid = false };

            try
            {

                JSchemaUrlResolver resolver = new JSchemaUrlResolver();
                JSchema jSchema = JSchema.Parse(schema, resolver);
                JObject jsonMessage = JObject.Parse(message);

                IList<string> errors = null;
                if (!jsonMessage.IsValid(jSchema, out errors))
                {
                    if (errors != null && errors.Count > 0)
                    {
                        result.Errors = errors;
                    }
                }
                else
                {
                    result.IsMessageValid = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"CheckSchema - Error {ex.Message}");
            }


            return result;
        }
        #endregion
    }
}
