using System;
using Newtonsoft.Json;

namespace PrinterAgentServer.Util
{
    public class UrlSafeBase64Converter : JsonConverter
    {
        private byte[] ConvertFromBase64Url(string input)
        {
            string incoming = input.Replace('_', '/').Replace('-', '+');
            switch (input.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }
            byte[] bytes = Convert.FromBase64String(incoming);
            return bytes;

        }

        private string ConvertToBase64Url(byte[] bytes)
        {
            char[] padding = { '=' };
            string base64String= Convert.ToBase64String(bytes).TrimEnd(padding).Replace('+', '-').Replace('/', '_');
            return base64String;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value != null)
                return ConvertFromBase64Url((string) reader.Value);

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
