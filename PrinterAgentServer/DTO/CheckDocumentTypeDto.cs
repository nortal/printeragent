using Newtonsoft.Json;

namespace PrinterAgentServer.DTO
{
    public class CheckDocumentTypeDto
    {
        [JsonProperty("document-type")]
        public string DocumentType { get; set; }
    }
}
