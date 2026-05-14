using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Microsoft.SqlServer.Management.Smo;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache
{
    public class SystemFormDesc : FormDescBase
    {
        public SystemFormDesc(int surveyId, string projectId, BvReplicationColumnsEntity column)
        {
            SurveyId = surveyId;
            IsReplicated = true;
            FormLevel = ReplicationSchemaService.GetDestinationTableName(surveyId);
            ProjectId = projectId;
            FormName = column.ColumnName;
            this.VariableType = VariableDataType.Background;
            switch ((SqlDataType)column.ColumnType)
            {
                case SqlDataType.Bit:
                    this.BOOL = true;
                    break;
                case SqlDataType.Date:
                case SqlDataType.DateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.SmallDateTime:
                    this.DATE = true;
                    break;
                default:
                    break;
            }


            LoopPath = new string[]{FormLevel};
        }
    }
}