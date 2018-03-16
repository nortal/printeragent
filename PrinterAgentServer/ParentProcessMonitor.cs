using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using PrinterAgentServer.Util;

namespace PrinterAgentServer
{
    public class ParentProcessMonitor
    {
        private Timer _timer;
        private readonly int processId;

        public ParentProcessMonitor(int processId)
        {
            this.processId = processId;
            _timer = new Timer();
            _timer.Interval = 5000;
            _timer.Elapsed += Timer_Tick;
        }

        public void Start()
        {
            _timer.Start();
        }

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            Process process =null;
            try
            {
                process = Process.GetProcessById(processId);
            }
            catch (System.Exception ex)
            {
                Logger.LogInfo("Parent proccess has not been found:\n"+ ex);
            }
            if (process == null)
                Environment.Exit(0);
        }
    }
}
