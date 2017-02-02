using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using PrinterAgent.DTO;
using PrinterAgent.Util;

namespace PrinterAgent.Service
{
    public class PrinterConfigurationService
    {
        private static readonly string HostUrl = ConfigurationManager.AppSettings["PrinterConfigurationBaseUrl"];

        private static readonly bool LogPrintConfRequests = bool.Parse(ConfigurationManager.AppSettings["LogPrintConfRequests"]);
        private HttpClient CreateClient()
        {
            HttpClient client = null;

            if (LogPrintConfRequests)
                client = new HttpClient(new HttpClientLoggingHandler(new HttpClientHandler()));
            else
                client = new HttpClient();

            client.BaseAddress = new Uri(HostUrl);
            
            client.DefaultRequestHeaders.Accept.Clear();
            
            
            return client;
        }

        public PrintConfigurationDto CreateConfiguration(PrintConfigurationDto conf)
        {
            using (var client = CreateClient())
            {
                var response = client.PutAsJsonAsync("api/computers/agent", conf).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<PrintConfigurationDto>().Result;
                }
                Logger.LogError("CreateConfiguration got unsuccessful response: " + response);
                return null;
            }
        }

        public PrintConfigurationDto UpdateConfiguration(string printerAgentId, PrintConfigurationDto conf)
        {
            using (var client = CreateClient())
            {
                var response =
                    client.PostAsJsonAsync<PrintConfigurationDto>("api/computers/agent/" + printerAgentId, conf).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<PrintConfigurationDto>().Result;
                }
                Logger.LogErrorToPrintConf("UpdateConfiguration got unsuccessful response: " + response);
                return null;
            }
        }

        public PrintConfigurationDto GetConfiguration(string printerAgentId)
        {
            using (var client = CreateClient())
            {
                var response = client.GetAsync("api/computers/agent/" + printerAgentId).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<PrintConfigurationDto>().Result;
                }

                Logger.LogErrorToPrintConf("GetConfiguration got unsuccessful response: " + response);
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
                Logger.LogErrorToPrintConf("CheckSignature got unsuccessful response: " + response);
                return null;
            }
        }

        public void SendLog(string printerAgentId, string log)
        {
            using (var client = CreateClient())
            {
                var requestUrl = string.Format("api/computers/agent/{0}/logs", printerAgentId);
                
                var response = client.PutAsync(requestUrl, new StringContent(log, Encoding.UTF8)).Result;

                if (response.IsSuccessStatusCode)
                {
                    return;
                }
                Logger.LogError("SendLog got unsuccessful response: " + response);
            }
        }
    }
}
