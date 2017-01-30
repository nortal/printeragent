using System;
using System.Diagnostics;

namespace PrinterAgent.Util
{
    public class Logger
    {
        public static void LogError(string message, Guid? instanceId = null)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry(ComposeMessage(message, instanceId), EventLogEntryType.Error);
            }
            //Trace.TraceError(ComposeMessage(message, instanceId));

        }

        public static void LogInfo(string message, Guid? instanceId=null)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry(ComposeMessage(message, instanceId), EventLogEntryType.Information);
            }
            //Trace.TraceInformation(ComposeMessage(message, instanceId));
        }


        public static string ComposeMessage(string message, Guid? instanceId = null)
        {
            return string.Format("{0} - {1}", instanceId, message);
            //return string.Format("{0: yyyy/MM/dd HH:mm:ss} - {1} - {2}", DateTime.Now, instanceId, message);
        }
    }
}
