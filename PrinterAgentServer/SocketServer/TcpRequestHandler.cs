using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PrinterAgentServer.SocketServer
{
    
    public class TcpRequestHandler
    {
        private static readonly int RequestTimeout = Int32.Parse(ConfigurationManager.AppSettings["RequestTimeoutMinutes"]);

        private TcpClient clientConnection;
        private string protocol;

        public TcpRequestHandler(TcpClient clientConnection)
        {
            this.clientConnection = clientConnection;
        }

        public void HandleClientConnection()
        {
            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    using (clientConnection)
                    using (NetworkStream stream = clientConnection.GetStream())
                    {
                        var tcpMessage = GetTcpMessage(clientConnection, stream);
                        var request = ParseTcpMessageToRequest(tcpMessage);

                        var response = new HttpRequestHandler(request).HandleRequest();

                        var tcpResponse = CreateResponse(response.status, response.body, response.headers);
                        var responseBytes = Encoding.ASCII.GetBytes(tcpResponse);

                        stream.Write(responseBytes, 0, responseBytes.Length);
                        if (response.body != null)
                            stream.Write(response.body, 0, response.body.Length);
                    }
                }
                catch
                {
                    
                }
            });
            task.Wait(RequestTimeout * 60 * 1000);
        }

        private HttpRequestHandler.TcpMessageHttpRequest ParseTcpMessageToRequest(byte[] tcpMessage)
        {

            var messageChars = Encoding.ASCII.GetChars(tcpMessage.ToArray(), 0, (int)tcpMessage.Length);
            var requestLineEnd = Array.FindIndex(messageChars, x => x == '\n' || x == '\r');
            if (requestLineEnd < 0)
                return null;

            var httpRequestLine = new string(messageChars, 0, requestLineEnd);

            Regex regex = new Regex(@"^(?<method>[A-Z]*) (?<query>.*?)(\?(?<params>.*))? (?<protocol>HTTP\/1\.1)$");
            var match = regex.Match(httpRequestLine);
            if (!match.Success)
                return null;

            var groups = match.Groups;

            this.protocol = groups["protocol"].Value;

            var request = new HttpRequestHandler.TcpMessageHttpRequest()
            {
                query = groups["query"].Success ? groups["query"].Value : null,
                method = groups["method"].Success ? groups["method"].Value : null,
            };

            if (groups["params"].Success)
            {
                var paramsString = groups["params"].Value;
                var paramsKeyValues = paramsString.Split('&');
                foreach (var param in paramsKeyValues)
                {
                    string value = null;
                    var i = param.IndexOf('=');
                    if (i < 0)
                        continue;

                    var key = param.Substring(0, i);
                    if (param.Length > i + 1)
                        value = param.Substring(i + 1);
                    if (!string.IsNullOrEmpty(key))
                        request.parameters[key] = value;
                }
            }

            return request;
        }



        public byte[] GetTcpMessage(TcpClient clientConnection, NetworkStream stream)
        {

            byte[] data = new byte[clientConnection.ReceiveBufferSize];
            stream.ReadTimeout = 250;
            using (MemoryStream ms = new MemoryStream())
            {

                int numBytesRead;
                try
                {
                    while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0)
                    {
                        ms.Write(data, 0, numBytesRead);
                    }
                }
                catch (IOException ex)
                {
                    var socketExept = ex.InnerException as SocketException;
                    if (socketExept == null || socketExept.ErrorCode != 10060)
                        // if it's not the "expected" exception, let's not hide the error
                        throw ex;
                }

                return ms.ToArray();
            }


        }
        

        private string CreateResponse(string status, byte[] body, Dictionary<string, string> headers)
        {
            var headerString = "";

            foreach (var header in headers)
            {
                headerString += header.Key+": "+ header.Value+"\n";
            }

            return
                this.protocol + " " + status + "\n" +
                "Date: " + DateTime.Now + "\n" +
                "Server: Apache/2.2.3\n" +
                "Last-Modified: " + DateTime.Now + "\n" +
                "Content-Length: " + (body!=null ? body.Length : 0)+ "\n" +
                headerString +
                "Accept-Ranges: bytes\n" +
                "Connection: close\n" +
                "\n";
        }
    }
}
