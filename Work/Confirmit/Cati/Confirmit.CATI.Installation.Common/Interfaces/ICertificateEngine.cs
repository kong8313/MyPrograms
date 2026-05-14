using System.Security.Cryptography.X509Certificates;

namespace Confirmit.CATI.Installation.Common.Interfaces
{
    public interface ICertificateEngine
    {

        /// <summary>
        /// Verify, that certificate won't expire during next 3 month
        /// </summary>
        /// <param name="certificate">Certificate object</param>
        /// <returns></returns>
        string VerifyCertificateAllowability(X509Certificate2 certificate);

        /// <summary>
        /// Verify, that certificate won't expire during next 3 months
        /// </summary>
        /// <param name="certificate">Certificate object</param>
        /// <param name="showQuestions">Show questions or just return information about expiration</param>
        /// <returns></returns>
        string VerifyCertificateAllowability(X509Certificate2 certificate, bool showQuestions);

        /// <summary>
        /// Verify that certificate from file is fine
        /// </summary>
        /// <param name="certificatePath">Path to certificate</param>
        /// <param name="certificatePassword">Password</param>
        /// <returns></returns>
        string VerifyCertificateFromFile(string certificatePath, string certificatePassword);
    }
}
