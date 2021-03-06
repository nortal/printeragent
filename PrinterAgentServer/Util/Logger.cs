﻿using System;
using System.Diagnostics;
using System.Security;
using PrinterAgentServer.Service;

namespace PrinterAgentServer.Util
{
    public class Logger
    {
        private static PrinterConfigurationService confService = new PrinterConfigurationService();
        
        public static void LogErrorToPrintConf(string message)
        {
            try
            {
                confService.SendLog(ComposeMessage("ERROR", message));
            }
            catch { }
            LogError(message);
        }

        public static void LogError(string message)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry(ComposeMessage("ERROR", message), EventLogEntryType.Error);
            }
            
        }

        public static void LogInfo(string message)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry(ComposeMessage("INFO",message), EventLogEntryType.Information);
            }
            
        }


        public static string ComposeMessage(string type, string message)
        {
            return string.Format("{0}: {1}\\{2}{3}{4}", type, Environment.UserDomainName, Environment.UserName, Environment.NewLine, message);
        }

        public static bool UserHasRights()
        {
            try
            {

                LogInfo("Print Agent - Verifying event log permissions");
            }
            catch (SecurityException)
            {
                return false;
            }
            return true;
        }
    }
}
