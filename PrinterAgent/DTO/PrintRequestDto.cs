namespace PrinterAgent.DTO
{
    public class PrintRequestDto
    {
        public string DocumentType { get; set; }

        public byte[] Document { get; set; }
        public string HashSignature { get; set; }
        public string SignatureAlgorithm { get; set; }
        public string HashAlgorithm { get; set; }
    }
}
