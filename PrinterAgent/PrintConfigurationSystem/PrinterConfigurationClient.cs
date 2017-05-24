using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using PrinterAgent.PrintConfigurationSystem.DTO;
using PrinterAgent.Util;

namespace PrinterAgent.PrintConfigurationSystem
{
    public class PrinterConfigurationClient
    {
        private static readonly string HostUrl = ConfigurationManager.AppSettings["PrinterConfigurationBaseUrl"];

        private HttpClient CreateClient()
        {
            HttpClient client = new HttpClient(new HttpRequestHandler(new HttpClientHandler()));

            client.BaseAddress = new Uri(HostUrl);
            
            client.DefaultRequestHeaders.Accept.Clear();
            
            
            return client;
        }

        public PrintConfigurationResponseDto CreateConfiguration(PrintConfigurationRequestDto conf)
        {
            using (var client = CreateClient())
            {
                var response = client.PutAsJsonAsync("api/computers/agent", conf).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<PrintConfigurationResponseDto>().Result;
                }
                
                return null;
            }
        }

        public PrintConfigurationResponseDto UpdateConfiguration(string printerAgentId, PrintConfigurationRequestDto conf)
        {
            using (var client = CreateClient())
            {
                var response = client.PostAsJsonAsync("api/computers/agent/" + printerAgentId, conf).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<PrintConfigurationResponseDto>().Result;
                }
                
                return null;
            }
        }

        public PrintConfigurationResponseDto GetConfiguration(string printerAgentId)
        {
            using (var client = CreateClient())
            {
                var response = client.GetAsync("api/computers/agent/" + printerAgentId).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<PrintConfigurationResponseDto>().Result;
                }
                
                return null;
            }
        }

        public SignatureVerificationResponseDto CheckSignature(SignatureVerificationRequestDto request)
        {
            using (var client = CreateClient())
            {
                var response = client.PostAsJsonAsync("api/signature/verify", request).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<SignatureVerificationResponseDto>().Result;
                }
                
                return null;
            }
        }

        public void SendLog(string printerAgentId, string log)
        {
            using (var client = CreateClient())
            {
                var requestUrl = string.Format("api/computers/agent/{0}/logs", printerAgentId);
                
                var response = client.PutAsync(requestUrl, new StringContent(log, Encoding.UTF8)).Result;
                
            }
        }
    }
}
