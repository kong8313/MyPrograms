using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace CustomActionLibrary
{
    public enum ValidationState
    { 
        Valid,
        Expired,
        ExpireSoon
    }

    public class CertificateValidator
    {
        private const int CertificateExpiredMonthCount = 3;

        private readonly ILogger _logger;
        private readonly string _certificatePath;
        private readonly string _certificatePassword;

        public CertificateValidator(
           ILogger logger, 
           string certificatePath,
           string certificatePassword)
        {
            _certificatePassword = certificatePassword;
            _certificatePath = certificatePath;
            _logger = logger;
        }

        public string GetValidationWarning(ValidationState validationState)
        {
            switch (validationState)
            { 
                case ValidationState.Valid:
                    return string.Empty;
                case ValidationState.Expired:
                    return "WARNING: Selected certificate has expired.";
                case ValidationState.ExpireSoon:
                    return string.Format("WARNING: Selected certificate will expire during next {0} months.", CertificateExpiredMonthCount);
                default:
                    throw new Exception("Unknown validate state: " + validationState);
            }
        }

        public ValidationState Validate()
        {
            X509Certificate2 cert = GetCertificate();

            if (DateTime.Now < cert.NotBefore || DateTime.Now > cert.NotAfter)
            {
                return ValidationState.Expired;
            }

            if (cert.NotAfter < DateTime.Now.AddMonths(CertificateExpiredMonthCount))
            {
                return ValidationState.ExpireSoon;
            }

            return ValidationState.Valid;
        }

        private X509Certificate2 GetCertificate()
        {
            if (!File.Exists(_certificatePath))
            {
                throw new Exception("You should specify a valid path to a certificate file before continue");
            }

            try
            {
                return new X509Certificate2(_certificatePath, _certificatePassword);
            }
            catch (CryptographicException ex)
            {
                _logger.WriteLog(TraceEventType.Error, ex.ToString());
                throw new Exception("The password of selected certificate is incorrect or the selected file has wrong type.", ex);
            }
        }
    }
}
