using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PrinterAgentServer.DTO;
using PrinterAgentServer.Exception;
using PrinterAgentServer.Service;
using PrinterAgentServer.Util;

namespace PrinterAgentServer.SocketServer
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
            try
            {
                switch (request.method + " " + request.query)
                {
                    case "GET /api/print-jobs/dummy.png":
                        return Print();
                    case "GET /api/print-jobs/ping.png":
                        return Ping();
                    case "GET /api/print-jobs/check-doctype.png":
                        return CheckDocumentType();
                }

            }
            catch (AgentFatalException e)
            {
                Environment.Exit(e.GetCode());
            }
            catch (UnathorizedException e)
            {
                Logger.LogError(e.ToString());
                return UnauthorizedResponse(e);
            }
            catch (ForbiddenException e)
            {
                Logger.LogError(e.ToString());
                return ForbiddenResponse(e);
            }
            catch (System.Exception e)
            {
                Logger.LogErrorToPrintConf(e.ToString());
                return ForbiddenResponse(e);
            }
            return new TcpMessageHttpResponse() { status = "404 Not Found" };
        }


        private TcpMessageHttpResponse Ping()
        {
            string json = JsonConvert.SerializeObject(request.parameters, Formatting.Indented);
            var pingRequest = JsonConvert.DeserializeObject<PingRequestDto>(json);

            new PrinterAgentService().Ping(pingRequest);
            return PingImageResponse();
        }

        private TcpMessageHttpResponse CheckDocumentType()
        {
            
            string json = JsonConvert.SerializeObject(request.parameters, Formatting.Indented);
            var pingRequest = JsonConvert.DeserializeObject<CheckDocumentTypeDto>(json);

            new PrinterAgentService().CheckDocumentType(pingRequest);            
            return PingImageResponse();
        }

        private TcpMessageHttpResponse Print()
        {
            string json = JsonConvert.SerializeObject(request.parameters, Formatting.Indented);
            var printRequest = JsonConvert.DeserializeObject<PrintRequestDto>(json);
            var printerName = new PrinterAgentService().Print(printRequest);
            
            return PrinterNameImageResponse(printerName);
            
        }

        private TcpMessageHttpResponse PrinterNameImageResponse(string printerName)
        {
            var content = ImageDrawer.DrawImage(printerName);
            return ImageResponse(content);
        }

        private TcpMessageHttpResponse PingImageResponse()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dummy.png");
            var content = File.ReadAllBytes(path);
            return ImageResponse(content);
        }

        private TcpMessageHttpResponse ImageResponse(byte[] imgContent)
        {
            Logger.LogInfo("Print agent is sending a response to TCP");
            return new TcpMessageHttpResponse()
            {
                body = imgContent,
                headers = new Dictionary<string, string>()
                {
                    {"Content-Type","image/png"}
                },
                status = "200 OK"
            };
        }

        

        private TcpMessageHttpResponse ForbiddenResponse(System.Exception e)
        {
            return new TcpMessageHttpResponse() { status = "403 Forbidden" };
        }

        private TcpMessageHttpResponse UnauthorizedResponse(System.Exception e)
        {
            return new TcpMessageHttpResponse() { status = "401 Unauthorized" };
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
