using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PrinterAgent.Util
{
    public class HttpClientLoggingHandler : DelegatingHandler
    {
        public HttpClientLoggingHandler(HttpMessageHandler innerHandler)
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
            Logger.LogInfo(requestLog);

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);


            string responseLog = "Response:" + Environment.NewLine;
            responseLog += response.ToString();
            if (response.Content != null)
            {
                responseLog += (await response.Content.ReadAsStringAsync());
            }
            Logger.LogInfo(responseLog);

            return response;
        }


    }
}
