using Newtonsoft.Json;

namespace PrinterAgent.PrintConfigurationSystem.DTO
{
    [JsonObject]
    public class SignatureVerificationRequestDto
    {
        [JsonProperty("document")]
        public string DocumentHash { get; set; }

        [JsonProperty("signature")]
        public string HashSignature { get; set; }

        [JsonProperty("signatureAlgorithm")]
        public string EncryptionAlgorithm { get; set; }
    }

    [JsonObject]
    public class SignatureVerificationResponseDto
    {
        [JsonProperty("verified")]
        public bool? Verified { get; set; }
    }
}
