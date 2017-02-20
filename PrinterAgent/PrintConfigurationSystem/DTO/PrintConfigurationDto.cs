using System.Collections.Generic;
using Newtonsoft.Json;

namespace PrinterAgent.PrintConfigurationSystem.DTO
{

    [JsonObject]
    public class PrintConfigurationRequestDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("agentVersion")]
        public string AgentVersion { get; set; }

        [JsonProperty("printers")]
        public List<string> Printers { get; set; }
        
    }

    [JsonObject]
    public class PrintConfigurationResponseDto
    {
        [JsonProperty("secret")]
        public string Secret { get; set; }

        [JsonProperty("agentListeningPort")]
        public int? AgentListeningPort { get; set; }

        [JsonProperty("documentTypePrinterMappings")]
        public Dictionary<string, string> DocTypeMappings { get; set; }
        
    }
    
}
