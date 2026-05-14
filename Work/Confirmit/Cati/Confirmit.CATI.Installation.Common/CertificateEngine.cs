using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.Installation.Common.Properties;

namespace Confirmit.CATI.Installation.Common
{
    public class CertificateEngine : ICertificateEngine
    {
        private const int CertificateExpiredAlertDaysCount = 14;

        private readonly IDialogService _dialogService;

        public CertificateEngine(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        /// <summary>
        /// Verify, that certificate won't expire during next 3 months
        /// </summary>
        /// <param name="certificate">Certificate object</param>
        /// <returns></returns>
        public string VerifyCertificateAllowability(X509Certificate2 certificate)
        {
            return VerifyCertificateAllowability(certificate, true);
        }

        /// <summary>
        /// Verify, that certificate won't expire during next 3 months
        /// </summary>
        /// <param name="certificate">Certificate object</param>
        /// <param name="showQuestions">Show questions or just return information about expiration</param>
        /// <returns></returns>
        public string VerifyCertificateAllowability(X509Certificate2 certificate, bool showQuestions)
        {
            if (DateTime.Now < certificate.NotBefore || DateTime.Now > certificate.NotAfter)
            {
                if (!showQuestions)
                {
                    return Resources.TheCertificateIsExpired;
                }

                if (DialogResult.No == _dialogService.Show(
                    Resources.QuestionAboutExpiredCertificate, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    return "Execution stoped";
                }
            }
            else if (certificate.NotAfter < DateTime.Now.AddDays(CertificateExpiredAlertDaysCount))
            {
                if (!showQuestions)
                {
                    return string.Format(Resources.TheCertificateWillExpireSoonFormat, CertificateExpiredAlertDaysCount);
                }

                if (DialogResult.No == _dialogService.Show(
                    string.Format(Resources.QuestionAboutSoonExpiredCertificate, CertificateExpiredAlertDaysCount),
                    Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    return "Execution stoped";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Verify that certificate from file is fine
        /// </summary>
        /// <param name="certificatePath">Path to certificate</param>
        /// <param name="certificatePassword">Password</param>
        /// <returns></returns>
        public string VerifyCertificateFromFile(string certificatePath, string certificatePassword)
        {
            if (string.IsNullOrEmpty(certificatePath))
            {
                return Resources.CertificatePathIsEmpty;
            }

            if (!File.Exists(certificatePath))
            {
                return Resources.CertificateFileIsNotFound;
            }

            try
            {
                var cert = new X509Certificate2(certificatePath, certificatePassword);

                return VerifyCertificateAllowability(cert);
            }
            catch (Exception ex)
            {
                return string.Format(Resources.WrongCertificateParametersFormat, ex.Message);
            }
        }
    }
}
