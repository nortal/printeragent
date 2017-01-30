using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using PrinterAgent.DTO;
using PrinterAgent.Util;

namespace PrinterAgent.Service
{
    public class PrinterConfigurationService
    {
        private static readonly string HostUrl = ConfigurationManager.AppSettings["PrinterConfigurationHostUrl"];

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
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            return client;
        }

        public PrintConfigurationDto CreateConfiguration(PrintConfigurationDto conf)
        {
            using (var client = CreateClient())
            {
                var response = client.PutAsJsonAsync("/printer-conf/api/computers/agent", conf).Result;

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
                    client.PostAsJsonAsync<PrintConfigurationDto>("/printer-conf/api/computers/agent/" + printerAgentId, conf).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<PrintConfigurationDto>().Result;
                }
                Logger.LogError("UpdateConfiguration got unsuccessful response: " + response);
                return null;
            }
        }

        public PrintConfigurationDto GetConfiguration(string printerAgentId)
        {
            using (var client = CreateClient())
            {
                var response = client.GetAsync("/printer-conf/api/computers/agent/" + printerAgentId).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<PrintConfigurationDto>().Result;
                }

                Logger.LogError("GetConfiguration got unsuccessful response: " + response);
                return null;
            }
        }

        public SignatureVerificationResponseDto CheckSignature(SignatureVerificationRequestDto request)
        {
            using (var client = CreateClient())
            {
                var response = client.PostAsJsonAsync("/printer-conf/api/signature/verify", request).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<SignatureVerificationResponseDto>().Result;
                }
                Logger.LogError("CheckSignature got unsuccessful response: " + response);
                return null;
            }
        }
    }
}
