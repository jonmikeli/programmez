using System.Collections.Generic;

namespace IoT.AzureFunctions.SchemaCompareFunction.Responses
{
    internal class SchemaValidationResponse
    {
        public string Message { get; set; }
        public bool IsMessageValid { get; set; }
        public IList<string> Errors { get; set; }

    }
}