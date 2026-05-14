using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Queries;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.ServerControls;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class DbUpdateLog : BaseForm
    {
        public override string TopTitle => Strings.DBUpdateLog;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_grid.GetPage += GetPage;

            InitSearchingToolBar();
        }

        private object GetPage(out int totalCount)
        {
            var bvVersionHistoryEntities = new VersionHistoryQuery().GetAll();
            var bvVersionHistoryPartList = bvVersionHistoryEntities.Select(bvVersionHistoryEntity => new
            {
                DbLogId = bvVersionHistoryEntity.Id,
                ScriptVersion = bvVersionHistoryEntity.ScriptNumber == -1
                    ? bvVersionHistoryEntity.Description.Split(new[] { ':' }, 2)[0]
                    : string.Format("{0}.{1}.{2}.{3}", bvVersionHistoryEntity.Major, bvVersionHistoryEntity.Minor, bvVersionHistoryEntity.BranchName, bvVersionHistoryEntity.ScriptNumber),
                Description = bvVersionHistoryEntity.ScriptNumber == -1
                    ? bvVersionHistoryEntity.Description.Split(new[] { ':' }, 2)[1]
                    : bvVersionHistoryEntity.Description,
                bvVersionHistoryEntity.ScriptAppliedDate,
                bvVersionHistoryEntity.Duration,
                IsAppliedDuringDBCreation = bvVersionHistoryEntity.IsAppliedDuringDBCreation ? "Yes" : "No",
                bvVersionHistoryEntity.DbUpateUtilityVersion,
                bvVersionHistoryEntity.ActiveUser
            });

            return BaseMethods.GetPage(bvVersionHistoryPartList, m_grid.PageArguments, out totalCount);
        }

        private void InitSearchingToolBar()
        {
            var column = (GeneralGridColumn)m_grid.Columns.FromKey("IsAppliedDuringDBCreation");

            column.Items.Add(new ListItem("Yes", "Yes"));
            column.Items.Add(new ListItem("No", "No"));
        }
    }
}