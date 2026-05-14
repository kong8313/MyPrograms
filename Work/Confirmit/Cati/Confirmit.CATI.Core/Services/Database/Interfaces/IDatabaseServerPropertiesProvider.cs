using System;

namespace Confirmit.CATI.Core.Services.Database.Interfaces
{
    public enum EngineEdition
    {
        Personal = 1,
        Standard = 2,
        Enterprise = 3,
        Express = 4,
        AzureSql = 5,
        AzureManagedInstance = 8,
    }

    public enum SqlServerMajorVersion
    {
        Sql2000 = 8,
        Sql2005 = 9,
        Sql2008 = 10,
        Sql2012 = 11,
        Sql2014 = 12,
    }

    public interface IDatabaseServerPropertiesProvider
    {
        EngineEdition GetEngineEdition();
        Version GetProductVersion();
    }
}