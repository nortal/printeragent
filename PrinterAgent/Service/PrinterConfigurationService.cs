﻿using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing.Printing;
using System.Security.Cryptography;
using System.Windows.Forms;
using PrinterAgent.DTO;
using PrinterAgent.PrintConfigurationSystem;
using PrinterAgent.PrintConfigurationSystem.DTO;
using PrinterAgent.Util;

namespace PrinterAgent.Service
{
    public class PrinterConfigurationService
    {
        private PrinterConfigurationClient pcsClient = new PrinterConfigurationClient();
        

        public int? GetNetworkingPort()
        {
            return GetConfiguration()?.AgentListeningPort;
        }

        public PrintConfigurationResponseDto GetConfiguration()
        {
            try
            {
                var secret = GetPrinterAgentId();
                if (!string.IsNullOrEmpty(secret))
                    return pcsClient.GetConfiguration(secret);
            }
            catch (Exception e)
            {
                Logger.LogErrorToPrintConf(e.ToString());
            }
            return null;

        }

        private string GetPrinterAgentId()
        {
            var localSecret = SecretResolver.GetPrinterAgentId();

            if (string.IsNullOrEmpty(localSecret))
            {
                localSecret = RegisterComputer();
            }

            if (string.IsNullOrEmpty(localSecret))
            {
                Logger.LogError("Secret was not found. Registration failed.");
                throw new Exception("Secret was not found. Registration failed.");
            }
            return localSecret;
        }

        private string RegisterComputer()
        {
            Logger.LogInfo("Agent id does not exist in registry. Registering the computer on a backend service.");
            try
            {
                var id = CreateConfiguration()?.Secret;
                RegistryDataResolver.StorePrinterAgentId(id);
                return id;
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                return null;
            }

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
                    throw new Exception(requestHashAlgorithm + " is not recognized as a valid hash algorithm");
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
            catch (Exception e) { }
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
            var secret = GetPrinterAgentId();
            pcsClient.SendLog(secret, log);
        }
    }
}
