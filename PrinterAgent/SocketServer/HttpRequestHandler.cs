using System;
using System.Collections.Generic;
using System.IO;
using PrinterAgent.DTO;
using PrinterAgent.Service;
using PrinterAgent.Util;

namespace PrinterAgent.SocketServer
{
    public class HttpRequestHandler
    {
        private TcpMessageHttpRequest request;

        public HttpRequestHandler(TcpMessageHttpRequest request)
        {
            this.request = request;
        }

        public TcpMessageHttpResponse HandleRequest()
        {
            switch (request.method+" "+request.query)
            {
                case "GET /api/print-jobs/dummy.gif":
                    return Print();
                
            }
            return new TcpMessageHttpResponse() { status = "404 Not Found" };
        }

        private TcpMessageHttpResponse Print()
        {
            string printerName;
            try
            {
                var printRequest = new PrintRequestDto()
                {
                    Document =
                    request.parameters.ContainsKey("document")
                        ? UrlSafeBase64Converter.ConvertFromBase64Url(request.parameters["document"])
                        : null,
                    DocumentType = request.parameters.ContainsKey("document-type") ? request.parameters["document-type"] : null,
                    HashSignature = request.parameters.ContainsKey("signature") ? request.parameters["signature"] : null,
                    SignatureAlgorithm = request.parameters.ContainsKey("signature-algorithm") ? request.parameters["signature-algorithm"] : null,
                    HashAlgorithm = request.parameters.ContainsKey("hash-algorithm") ? request.parameters["hash-algorithm"] : null,
                };

                printerName=new PrinterAgentService().Print(printRequest);
            }
            catch (UnathorizedException e)
            {
                Logger.LogError(e.ToString());
                return new TcpMessageHttpResponse() { status = "401 Unauthorized" };
            }
            catch (ForbiddenException e)
            {
                Logger.LogError(e.ToString());
                return new TcpMessageHttpResponse() { status = "403 Forbidden"};
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                return new TcpMessageHttpResponse() { status = "403 Forbidden"};
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dummy.gif");
            var content = File.ReadAllBytes(path);
            Logger.LogInfo("Print agent is sending a response to TCP");
            return new TcpMessageHttpResponse()
            {
                body = content,
                headers = new Dictionary<string,string>()
                {
                    {"Content-Type","image/gif"},
                    {"Selected-Printer", printerName}
                },
                status = "200 OK"
            };
        }

        public class TcpMessageHttpRequest
        {
            public string method { get; set; }
            public string query { get; set; }
            public Dictionary<string, string> parameters = new Dictionary<string, string>();

        }

        public class TcpMessageHttpResponse
        {
            public string status { get; set; }
            public byte[] body { get; set; }

            public Dictionary<string, string> headers = new Dictionary<string, string>();


        }
    }
}
