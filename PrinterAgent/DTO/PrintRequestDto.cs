using Newtonsoft.Json;
using PrinterAgent.Util;

namespace PrinterAgent.DTO
{
    [JsonObject]
    public class PrintRequestDto
    {
        [JsonProperty("document-type")]
        public string DocumentType { get; set; }
        [JsonProperty("document"), JsonRequired]
        [JsonConverter(typeof(UrlSafeBase64Converter))]
        public byte[] Document { get; set; }
        [JsonProperty("signature"), JsonRequired]
        [JsonConverter(typeof(UrlSafeBase64Converter))]
        public byte[] Signature { get; set; }
        [JsonProperty("signature-algorithm"), JsonRequired]
        public string SignatureAlgorithm { get; set; }
        [JsonProperty("hash-algorithm"), JsonRequired]
        public string HashAlgorithm { get; set; }
    }
}
