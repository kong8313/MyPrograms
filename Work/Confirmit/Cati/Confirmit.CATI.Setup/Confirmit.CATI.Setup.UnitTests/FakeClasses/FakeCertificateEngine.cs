using System.Security.Cryptography.X509Certificates;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Setup.UnitTests.FakeClasses
{
    class FakeCertificateEngine : ICertificateEngine
    {
        string CrtificateFromFileVerificationResult { get; set; }
        string CertificateAllowabilityVerificationResult { get; set; }

        public FakeCertificateEngine()
        {
            CrtificateFromFileVerificationResult = CertificateAllowabilityVerificationResult = string.Empty;
        }

        public string VerifyCertificateFromFile(string certificatePath, string certificatePassword)
        {
            return CrtificateFromFileVerificationResult;
        }

        public string VerifyCertificateAllowability(X509Certificate2 certificate)
        {
            return VerifyCertificateAllowability(certificate, true);
        }

        public string VerifyCertificateAllowability(X509Certificate2 certificate, bool showQuestions)
        {
            return CertificateAllowabilityVerificationResult;
        }

        public X509Certificate2 GetCertificateFromStore(string certificateThumbprint, string certificateName, StoreName storeName, StoreLocation storeLocation)
        {
            return null;
        }
    }
}
