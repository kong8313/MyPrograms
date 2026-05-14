using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BootstrapperLibrary.Interfaces;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace BootstrapperLibrary
{
    public class InstalledProductSearcher : IInstalledProductSearcher
    {
        public string CurrentProductName { get; }
        public bool IsProductAlreadyInstalled { get; }
        public Version InstalledVersion { get; }
        public string ProductCode { get; }
        public string ProductName { get; }
        public string InstallLocation { get; }

        public InstalledProductSearcher(string currentProductName, string productNameMask, ILogger logger, IInstalledProductsReader installedProductsReader)
        {
            CurrentProductName = currentProductName;

            Dictionary<string, ProductInfo> installedProducts = installedProductsReader.GetInstalledProducts();

            var regEx = new Regex(productNameMask);

            foreach (KeyValuePair<string, ProductInfo> installedProduct in installedProducts)
            {
                if (regEx.IsMatch(installedProduct.Key))
                {
                    IsProductAlreadyInstalled = true;
                    InstalledVersion = new Version(installedProduct.Value.VersionString);
                    ProductCode = installedProduct.Value.ProductCode;
                    InstallLocation = installedProduct.Value.InstallLocation;
                    ProductName = installedProduct.Key;

                    logger.WriteLog("Information about installed product was found. ProductName={0} ProductCode={1} InstallationVersion={2} InstallLocation={3}",
                        ProductName, ProductCode, InstalledVersion, InstallLocation);
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(ProductCode))
            {
                logger.WriteLog("Information about installed product was not found");
            }
        }
    }
}