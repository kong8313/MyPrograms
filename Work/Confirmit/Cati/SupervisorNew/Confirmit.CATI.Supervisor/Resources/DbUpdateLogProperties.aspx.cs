using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Queries;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class DbUpdateLogProperties : BaseForm
    {
        private BvVersionHistoryEntity _bvVersionHistoryEntity;

        public BvVersionHistoryEntity BvVersionHistoryEntity
        {
            get {
                return _bvVersionHistoryEntity ??
                       (_bvVersionHistoryEntity = new VersionHistoryQuery().GetById(Convert.ToInt32(Request["DbLogId"])));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }
    }
}