using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PrinterAgent.DTO
{
    public class CheckDocumentTypeDto
    {
        [JsonProperty("document-type")]
        public string DocumentType { get; set; }
    }
}
