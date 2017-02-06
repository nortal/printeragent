using System.Collections.Generic;
using Newtonsoft.Json;

namespace PrinterAgent.DTO
{
    [JsonObject]
    public class PrintConfigurationDto
    {
        [JsonProperty("secret", NullValueHandling = NullValueHandling.Ignore)]
        public string Secret { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("additionalInfo", NullValueHandling = NullValueHandling.Ignore)]
        public string AdditionalInfo { get; set; }

        [JsonProperty("agentListeningPort", NullValueHandling = NullValueHandling.Ignore)]
        public int? AgentListeningPort { get; set; }

        [JsonProperty("agentVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string AgentVersion { get; set; }

        [JsonProperty("printers")]
        public List<PrinterDto> Printers { get; set; }


    }

    [JsonObject]
    public class PrinterDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("additionalInfo", NullValueHandling = NullValueHandling.Ignore)]
        public string AdditionalInfo { get; set; }

        [JsonProperty("isDefault", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsDefault { get; set; }

        [JsonProperty("documentTypes", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> DocumentTypes { get; set; }
    }
}
