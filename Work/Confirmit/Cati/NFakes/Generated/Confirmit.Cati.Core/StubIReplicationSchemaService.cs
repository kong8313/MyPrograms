using System;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation.Fakes
{
    public class StubIReplicationSchemaService : IReplicationSchemaService 
    {
        private IReplicationSchemaService _inner;

        public StubIReplicationSchemaService()
        {
            _inner = null;
        }

        public IReplicationSchemaService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void UpdateSurveyReplicationSchemeInt32ArrayOfTableInfoDelegate(int surveySid, TableInfo[] tables);
        public UpdateSurveyReplicationSchemeInt32ArrayOfTableInfoDelegate UpdateSurveyReplicationSchemeInt32ArrayOfTableInfo;

        void IReplicationSchemaService.UpdateSurveyReplicationScheme(int surveySid, TableInfo[] tables)
        {

            if (UpdateSurveyReplicationSchemeInt32ArrayOfTableInfo != null)
            {
                UpdateSurveyReplicationSchemeInt32ArrayOfTableInfo(surveySid, tables);
            } else if (_inner != null)
            {
                ((IReplicationSchemaService)_inner).UpdateSurveyReplicationScheme(surveySid, tables);
            }
        }

        public delegate bool IsReplicationSchemaChangedInt32ArrayOfTableInfoDelegate(int surveySid, TableInfo[] newTablesInfo);
        public IsReplicationSchemaChangedInt32ArrayOfTableInfoDelegate IsReplicationSchemaChangedInt32ArrayOfTableInfo;

        bool IReplicationSchemaService.IsReplicationSchemaChanged(int surveySid, TableInfo[] newTablesInfo)
        {


            if (IsReplicationSchemaChangedInt32ArrayOfTableInfo != null)
            {
                return IsReplicationSchemaChangedInt32ArrayOfTableInfo(surveySid, newTablesInfo);
            } else if (_inner != null)
            {
                return ((IReplicationSchemaService)_inner).IsReplicationSchemaChanged(surveySid, newTablesInfo);
            }

            return default(bool);
        }

        public delegate void UpdateQuotaBalancingConfigurationInt32ArrayOfTableInfoDelegate(int surveySid, TableInfo[] tables);
        public UpdateQuotaBalancingConfigurationInt32ArrayOfTableInfoDelegate UpdateQuotaBalancingConfigurationInt32ArrayOfTableInfo;

        void IReplicationSchemaService.UpdateQuotaBalancingConfiguration(int surveySid, TableInfo[] tables)
        {

            if (UpdateQuotaBalancingConfigurationInt32ArrayOfTableInfo != null)
            {
                UpdateQuotaBalancingConfigurationInt32ArrayOfTableInfo(surveySid, tables);
            } else if (_inner != null)
            {
                ((IReplicationSchemaService)_inner).UpdateQuotaBalancingConfiguration(surveySid, tables);
            }
        }

    }
}