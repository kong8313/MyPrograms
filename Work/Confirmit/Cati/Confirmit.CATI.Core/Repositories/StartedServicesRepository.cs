using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class StartedServicesRepository : IStartedServicesRepository
    {
        /// <summary>
        /// Return rtue - if service exist in BvStartedServices table, otherwise - false
        /// It is "internal" for testing purposes
        /// </summary>
        /// <param name="machineName">Machine name</param>
        /// <param name="serviceName">Service name</param>
        /// <returns></returns>
        internal bool IsServiceStarted(string machineName, string serviceName)
        {
            var parameters = new[]
            {
                new SqlParameter("MachineName", machineName),
                new SqlParameter("ServiceName", serviceName)
            };

            using (new ConnectionScope(BackendInstance.Current.DefaultInstanceConnectionString))
            {
                return (BvStartedServicesAdapter.GetByCondition("MachineName=@MachineName AND ServiceName=@ServiceName", parameters)).Count == 1;
            }

        }

        /// <summary>
        /// Add information about started service to BvStartedServices table
        /// </summary>
        /// <param name="machineName">Machine name</param>
        /// <param name="serviceName">Service name</param>
        public void AddStartedServiceInfo(string machineName, string serviceName)
        {
            var entity = new BvStartedServicesEntity
            {
                MachineName = machineName,
                ServiceName = serviceName
            };

            if (!IsServiceStarted(machineName, serviceName))
            {
                using (new ConnectionScope(BackendInstance.Current.DefaultInstanceConnectionString))
                {
                    BvStartedServicesAdapter.Insert(entity);
                }
            }
        }


        /// <summary>
        /// Remove information about started service from BvStartedServices table
        /// </summary>
        /// <param name="machineName">Machine name</param>
        /// <param name="serviceName">Service name</param>
        public void RemoveStartedServiceInfo(string machineName, string serviceName)
        {
            var parameters = new[]
            {
                new SqlParameter("MachineName", machineName),
                new SqlParameter("ServiceName", serviceName)
            };

            if (IsServiceStarted(machineName, serviceName))
            {
                using (new ConnectionScope(BackendInstance.Current.DefaultInstanceConnectionString))
                {
                    BvStartedServicesAdapter.DeleteByCondition("MachineName=@MachineName AND ServiceName=@ServiceName", parameters);
                }
            }            
        }
    }
}