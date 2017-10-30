using System;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using PrinterAgentServer.DTO;
using PrinterAgentServer.Exception;
using PrinterAgentServer.PrintConfigurationSystem;
using PrinterAgentServer.PrintConfigurationSystem.DTO;
using PrinterAgentServer.Util;

namespace PrinterAgentServer.Service
{
    public class PrinterConfigurationService
    {
        private PrinterConfigurationClient pcsClient = new PrinterConfigurationClient();

        static readonly object _getPrinterAgentIdlock = new object();

        static readonly object _sendLogLock = new object();

        public int? GetNetworkingPort()
        {
            try
            {
                return GetConfiguration()?.AgentListeningPort;
            }
            catch (AgentDisabledException)
            {
                return null;
            }

        }

        public PrintConfigurationResponseDto GetConfiguration()
        {
            var secret = GetPrinterAgentId();
            if (string.IsNullOrEmpty(secret))
                return null;

            var conf = pcsClient.GetConfiguration(secret);
            if (conf!=null && conf.AgentListeningPort==null)
                throw new AgentDisabledException();
            return conf;
        }

        private string GetPrinterAgentId()
        {
            lock (_getPrinterAgentIdlock)
            {
                var localSecret = SecretResolver.GetPrinterAgentId();

                if (string.IsNullOrEmpty(localSecret))
                {
                    localSecret = RegisterComputer();
                }

                if (string.IsNullOrEmpty(localSecret))
                {
                    Logger.LogError("Secret was not found. Registration of computer failed.");
                }
                return localSecret;
            }

        }

        private string RegisterComputer()
        {
            Logger.LogInfo("Agent id does not exist in registry. Registering computer on PCS.");

            RegistryDataResolver.CheckWriteAccess();

            var secret = CreateConfiguration()?.Secret;
            if (secret == null)
                return null;

            RegistryDataResolver.StorePrinterAgentId(secret);
            return secret;
       }
        
        

        public SignatureVerificationResponseDto CheckSignature(PrintRequestDto request)
        {
            var response = pcsClient.CheckSignature(new SignatureVerificationRequestDto()
            {
                DocumentHash = CreateHash(request.Document, request.HashAlgorithm),
                HashSignature = Convert.ToBase64String(request.Signature),
                EncryptionAlgorithm = request.SignatureAlgorithm
            });
            return response;

        }


        private string CreateHash(byte[] requestDocument, string requestHashAlgorithm)
        {
            using (var hashAlgorithm = HashAlgorithm.Create(requestHashAlgorithm))
            {
                if (hashAlgorithm == null)
                    throw new System.Exception(requestHashAlgorithm + " is not recognized as a valid hash algorithm");
                var hash = hashAlgorithm.ComputeHash(requestDocument);
                return Convert.ToBase64String(hash);

            }
        }

        public PrintConfigurationResponseDto CreateConfiguration()
        {
            var request = CreateConfigurationRequest();
            
            var response = pcsClient.CreateConfiguration(request);
            if (response != null)
            {
                CachedPrintConfiguration.LastSentPrinters = request.Printers;
            }
            return response;
            
        }

        private PrintConfigurationRequestDto CreateConfigurationRequest()
        {
            var request = new PrintConfigurationRequestDto();
            request.Name = GetCurrentComputerName();
            request.AgentVersion = Application.ProductVersion;
            request.Printers = PrinterManager.GetAvailablePrinters();
            return request;

        }

        private string GetCurrentComputerName()
        {
            string computerName = "";
            try
            {
                computerName += Domain.GetComputerDomain() + "\\";
            }
            catch { }
            computerName += Environment.MachineName;
            return computerName;
        }


        public void UpdateConfiguration()
        {
            var secret = GetPrinterAgentId();
            var request = CreateConfigurationRequest();
            
            var response = pcsClient.UpdateConfiguration(secret, request);
            if (response != null)
            {
                CachedPrintConfiguration.LastSentPrinters = request.Printers;
            }
            
        }

        public void SendLog(string log)
        {
            // lock monitor to avoid infinite recursive SendLog calls
            if (Monitor.TryEnter(_sendLogLock, 1500))
            {
                try
                {
                    var secret = GetPrinterAgentId();
                    if (!string.IsNullOrEmpty(secret))
                        pcsClient.SendLog(secret, log);
                }
                finally
                {
                    Monitor.Exit(_sendLogLock);
                }
            }

            
        }
    }
}
