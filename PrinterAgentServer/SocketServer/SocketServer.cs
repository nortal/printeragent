using System.Net;
using System.Net.Sockets;
using System.Threading;
using PrinterAgentServer.Util;

namespace PrinterAgentServer.SocketServer
{
    public class SocketServer 
    {
        private TcpListener listener;
        
        private int port;

        public SocketServer(int port)
        {
            IPAddress ipAddr = Dns.GetHostEntry("localhost").AddressList[0];
            listener = new TcpListener(ipAddr, port);
            this.port = port;
        }
        

        public void Start()
        {
            Logger.LogInfo("Starting socket server on: "+port);
            listener.Start();

            while (true)
            {
                try
                {
                    TcpClient clientSocket = listener.AcceptTcpClient();
                    new Thread(new TcpRequestHandler(clientSocket).HandleClientConnection).Start();
                }
                catch
                {
                    
                }
                
            }

        }

        public void Stop()
        {
            //if (listener.Stop())
        }
        
    }
}
