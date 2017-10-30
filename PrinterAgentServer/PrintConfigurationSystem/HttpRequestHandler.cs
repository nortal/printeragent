using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PrinterAgentServer.Util;

namespace PrinterAgentServer.PrintConfigurationSystem
{
    public class HttpRequestHandler : DelegatingHandler
    {
        private static readonly bool LogPrintConfRequests = bool.Parse(ConfigurationManager.AppSettings["LogPrintConfRequests"]);

        public HttpRequestHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string requestLog = "Request:"+Environment.NewLine;
            requestLog += request.ToString();
            if (request.Content != null)
            {
                requestLog += (await request.Content.ReadAsStringAsync());
            }

            LogRequest(requestLog);

            
            HttpResponseMessage response;
            try
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (System.Exception e)
            {
                Logger.LogError("Got exception while requesting PCS:" + Environment.NewLine + requestLog + Environment.NewLine + e.ToString());
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            
            string responseLog = "Response:" + Environment.NewLine;
            responseLog += response.ToString();
            if (response.Content != null)
            {
                responseLog += (await response.Content.ReadAsStringAsync());
            }

            LogRequest(responseLog);

            if((int)response.StatusCode >=400 && (int)response.StatusCode < 500)
                Logger.LogErrorToPrintConf("Got unsuccessful response from PCS:"+ Environment.NewLine + requestLog+ Environment.NewLine+ responseLog);

            return response;
        }


        private void LogRequest(string message)
        {
            if (LogPrintConfRequests)
                Logger.LogInfo(message);
        }

    }
}
