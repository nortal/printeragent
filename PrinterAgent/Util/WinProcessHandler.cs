using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PrinterAgent.Util
{
    public static class WinProcessHandler
    {
        public static bool IsCurrentUserRunningSingleInstance()
        {
            var currentProcess = Process.GetCurrentProcess();
            var otherProcessesWithTheSameName =
                Process.GetProcessesByName(currentProcess.ProcessName).Where(p => p.Id != currentProcess.Id).ToList();
            string currentUser = Environment.UserName;
            foreach (var p in otherProcessesWithTheSameName)
            {
                try
                {
                    var query = new ObjectQuery(string.Format("Select * from Win32_Process Where ProcessID ={0}", p.Id));
                    var result = new ManagementObjectSearcher(query);
                    foreach (ManagementObject mo in result.Get())
                    {
                        var ownerDetails = new String[2];
                        mo.InvokeMethod("GetOwner", ownerDetails);
                        var user = ownerDetails[0];
                        if (currentUser.Equals(user, StringComparison.OrdinalIgnoreCase))
                            return false;
                    }
                }
                catch
                {

                }
            }
            return true;
        }
    }

}
