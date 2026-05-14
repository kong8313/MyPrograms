using System.Collections.Generic;

namespace Confirmit.CATI.Installation.Common.Interfaces
{
    public interface IInstalledProductsReader
    {
        /// <summary>
        /// Get information about all installed products
        /// </summary>
        /// <returns></returns>
        Dictionary<string, ProductInfo> GetInstalledProducts();
    }
}
