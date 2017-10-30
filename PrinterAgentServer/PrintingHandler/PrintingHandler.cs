﻿using System;
using System.Configuration;
using System.IO;

namespace PrinterAgentServer.PrintingHandler
{
    public abstract class PrintingHandler
    {
        private static readonly string TempFilePrefix = ConfigurationManager.AppSettings["TempFilePrefix"];

        public void Print(string printerName, byte[] document)
        {
            var filePath = StoreFile(document);
            //new Thread(() =>{
                try
                {
                    Print(printerName, filePath);
                }
                finally
                {
                    RemoveFile(filePath);
                }
            //}).Start();
        }

        protected abstract void Print(string printerName, string filePath);
        
        private string StoreFile(byte[] document)
        {
            var tempPath = Path.GetTempPath();
            var filePath = tempPath + TempFilePrefix + Guid.NewGuid() + ".pdf";

            File.WriteAllBytes(filePath, document);
            return filePath;
        }

        private void RemoveFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch
            {
                // ignored
            }
        }



    }
}