using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrinterAgent.Service
{
    public class TempFilesRemover
    {

        private static readonly int RemoveInterval = Int32.Parse(ConfigurationManager.AppSettings["TempFilesRemoveIntervalMinutes"]);
        private static readonly string TempFilePrefix = ConfigurationManager.AppSettings["TempFilePrefix"];

        private bool isActive;
        public void Start()
        {
            isActive = true;
            while (isActive)
            {
                var tempPath = Path.GetTempPath();
                DirectoryInfo tempDir = new DirectoryInfo(tempPath);
                FileInfo[] tempFiles = tempDir.GetFiles(TempFilePrefix + "*.pdf");

                var expirationTime = DateTime.Now.AddMinutes(-1*RemoveInterval);
                foreach (FileInfo f in tempFiles)
                {
                    if (f.CreationTime < expirationTime)
                        RemoveFile(f.FullName);
                    
                }
                Thread.Sleep(RemoveInterval * 60 *1000);
            }
        }

        private void RemoveFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch
            {
                
            }
        }

        public void Stop()
        {
            isActive = false;
        }
    }
}
