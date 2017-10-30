using Newtonsoft.Json;

namespace PrinterAgentServer.DTO
{
    public class PingRequestDto
    {
        [JsonProperty("document-type")]
        public string DocumentType { get; set; }
    }
}
