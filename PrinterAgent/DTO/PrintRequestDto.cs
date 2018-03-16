using Newtonsoft.Json;
using PrinterAgent.Util;

namespace PrinterAgent.DTO
{
    [JsonObject]
    public class BacthedPrintRequestDto
    {
        [JsonProperty("document-type")]
        public string DocumentType { get; set; }
        [JsonProperty("document"), JsonRequired]
        public string Document { get; set; }
        [JsonProperty("signature")]
        [JsonConverter(typeof(UrlSafeBase64Converter))]
        public byte[] Signature { get; set; }
        [JsonProperty("signature-algorithm")]
        public string SignatureAlgorithm { get; set; }
        [JsonProperty("hash-algorithm")]
        public string HashAlgorithm { get; set; }

        [JsonProperty("batchNr"), JsonRequired]
        public int BatchNr { get; set; }

        [JsonProperty("batchesTotal"), JsonRequired]
        public int BatchesTotal { get; set; }

        [JsonProperty("printId"), JsonRequired]
        public string PrintId { get; set; }
    }
}
