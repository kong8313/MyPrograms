using System;
using Confirmit.Security.Crypto;
using Confirmit.Security.Crypto.Web;

namespace CryptographicUtility
{
    public class Program
    {
        private string _originalText;
        private bool _useRsaAlgorithm;

        /// <summary>
        /// Print information about usage this utility
        /// </summary>
        private static void PrintHelp()
        {
            const string help = "Usage:  CryptographicUtility.exe <string to encrypt> [-RSA]";

            Console.WriteLine(help);
        }

        /// <summary>
        /// Validade command line arguments and initialize the program variables
        /// </summary>
        /// <param name="args">Program arguments</param>
        private void ValidateCommandLineParameters(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                PrintHelp();
                throw new Exception("Wrong count of parameters");
            }

            _originalText = args[0];

            _useRsaAlgorithm = args.Length > 1 && args[1].ToLowerInvariant() == "-rsa";
        }

        /// <summary>
        /// The main function of programm
        /// </summary>
        /// <param name="args">Program agruments</param>
        private void StartPropgram(string[] args)
        {
            ValidateCommandLineParameters(args);

            string enctyptedText = _useRsaAlgorithm
                ? new CryptComp().Encrypt(_originalText)
                : EncryptionUsingMachineKey.Encrypt(DataProtection.All, _originalText);

            Console.Write(enctyptedText);
        }

        private static void Main(string[] args)
        {
            try
            {
                new Program().StartPropgram(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
