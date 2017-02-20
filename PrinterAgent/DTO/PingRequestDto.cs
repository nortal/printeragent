using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace PrinterAgent.DTO
{
    public class PingRequestDto
    {
        [JsonProperty("document-type")]
        public string DocumentType { get; set; }
    }
}
