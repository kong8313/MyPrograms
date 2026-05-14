using System;
using System.Security.Cryptography.X509Certificates;
using CustomActionLibrary.Properties;

namespace CustomActionLibrary
{
    /// <summary>
    /// Class for work with certificates
    /// </summary>
    public class CertificateStoreWorker
    {
        private readonly string _certificateKey;
        private readonly string _issuerName;

        private readonly SetupEngine _setupEngine;

        private readonly X509Store _store;

        public CertificateStoreWorker(
            SetupEngine setupEngine,
            StoreName storeName,
            StoreLocation storeLocation,
            string certificateKey,
            string issuerName)
        {
            if (!string.IsNullOrEmpty(certificateKey) && !certificateKey.StartsWith("CN="))
            {
                certificateKey = "CN=" + certificateKey;
            }

            _certificateKey = certificateKey;
            _issuerName = issuerName;

            _setupEngine = setupEngine;
            _store = new X509Store(storeName, storeLocation);
        }

        /// <summary>
        /// Add certificate to selected store and location
        /// Now we add only test root certificate
        /// </summary>        
        /// <param name="certificateData">Certificate data</param>
        public void InstallCertificate(byte[] certificateData)
        {
            InstallCertificate(new X509Certificate2(certificateData));
        }

        public void InstallCertificate(X509Certificate2 certificate)
        {
            _setupEngine.Logger.WriteLog("Begin InstallCertificate");
            _store.Open(OpenFlags.ReadWrite);
            _store.Add(certificate);
            _store.Close();

            _setupEngine.Logger.WriteLog("End InstallCertificate");
        }

        /// <summary>
        /// Get certificate thumbprint. System should has only one certificate with specified parameters
        /// </summary>
        /// <returns></returns>
        public string GetCertificateThumbprint()
        {
            _setupEngine.Logger.WriteLog("Begin GetCertificateThumbprint");
            try
            {
                X509Certificate2Collection certificates = GetCertificates();
                if (certificates.Count != 1)
                {
                    throw new Exception(string.Format(Resources.WrongCountOfCertificates, _certificateKey, certificates.Count));
                }

                return certificates[0].Thumbprint;
            }
            finally
            {
                _setupEngine.Logger.WriteLog("End GetCertificateThumbprint");
            }
        }

        /// <summary>
        /// Get count of certificates with specified parameters
        /// </summary>
        /// <returns></returns>
        public int GetCertificatesCount()
        {
            X509Certificate2Collection cers = GetCertificates();

            return cers.Count;
        }

        /// <summary>
        /// Get certificates with specified parameters
        /// </summary>
        /// <returns></returns>
        public X509Certificate2Collection GetCertificates()
        {
            _setupEngine.Logger.WriteLog("Begin GetCertificates");

            try
            {
                _store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection cers = _store.Certificates;

                if (!string.IsNullOrEmpty(_certificateKey))
                {
                    cers = cers.Find(X509FindType.FindBySubjectDistinguishedName, _certificateKey, false);
                }

                if (!string.IsNullOrEmpty(_issuerName))
                {
                    cers = cers.Find(X509FindType.FindByIssuerName, _issuerName, false);
                }

                return cers;
            }
            finally
            {
                _store.Close();
                _setupEngine.Logger.WriteLog("End GetCertificates");
            }
        }
    }
}
