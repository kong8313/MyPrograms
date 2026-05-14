namespace Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces
{
    public interface IValidator
    {
        /// <summary>
        /// Check, that all databases for update are ok
        /// </summary>
        /// <param name="productionDatabases">List of databases for update</param>
        void CheckDatabases(string[] productionDatabases);

        /// <summary>
        /// Check, that resources with update scripts exist and have no missing scripts
        /// </summary>
        void CheckUpdateScripts(); 
    }
}