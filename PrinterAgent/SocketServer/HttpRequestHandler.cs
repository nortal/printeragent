using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
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
                case "GET /api/print-jobs/print.png":
                    return Print();
                case "GET /api/print-jobs/ping.png":
                    return Ping();                
                case "GET /api/print-jobs/check-doctype.png":
                    return CheckDocumentType();
            }
            return new TcpMessageHttpResponse() { status = "404 Not Found" };
        }


        private TcpMessageHttpResponse Ping()
        {
            try
            {
                string json = JsonConvert.SerializeObject(request.parameters, Formatting.Indented);
                var pingRequest = JsonConvert.DeserializeObject<PingRequestDto>(json);

                new PrinterAgentService().Ping(pingRequest);
            }
            catch (Exception e)
            {
                Logger.LogErrorToPrintConf(e.ToString());
                return ForbiddenResponse(e);
            }
            return PingImageResponse();
        }

        private TcpMessageHttpResponse CheckDocumentType()
        {
            try
            {
                string json = JsonConvert.SerializeObject(request.parameters, Formatting.Indented);
                var pingRequest = JsonConvert.DeserializeObject<CheckDocumentTypeDto>(json);

                new PrinterAgentService().CheckDocumentType(pingRequest);
            }
            catch (Exception e)
            {
                Logger.LogErrorToPrintConf(e.ToString());
                return ForbiddenResponse(e);
            }
            return PingImageResponse();
        }

        private TcpMessageHttpResponse Print()
        {
            string printerName;
            try
            {
                string json = JsonConvert.SerializeObject(request.parameters, Formatting.Indented);
                var printRequest = JsonConvert.DeserializeObject<BacthedPrintRequestDto>(json);
                printerName=new PrinterAgentService().BatchedPrint(printRequest);
            }
            catch (UnathorizedException e)
            {
                Logger.LogError(e.ToString());
                return new TcpMessageHttpResponse() { status = "401 Unauthorized" };
            }
            catch (ForbiddenException e)
            {
                Logger.LogError(e.ToString());
                return ForbiddenResponse(e);
            }
            catch (Exception e)
            {
                Logger.LogErrorToPrintConf(e.ToString());
                return ForbiddenResponse(e);
            }
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

        

        private TcpMessageHttpResponse ForbiddenResponse(Exception e)
        {
            return new TcpMessageHttpResponse() { status = "403 Forbidden" };
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
