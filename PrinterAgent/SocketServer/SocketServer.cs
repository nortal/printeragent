using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using PrinterAgent.Util;


namespace PrinterAgent.SocketServer
{
    public class SocketServer 
    {
        private bool isRunning;
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
            isRunning = true;

            Logger.LogInfo("Starting socket server on: "+port);
            listener.Start();

            while (isRunning)
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
            isRunning = false;
            listener.Stop();
        }
    }
}
