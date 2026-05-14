using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class StateProperties : BaseForm
    {
        [StoreInViewState]
        public int GroupId;

        [StoreInViewState]
        public int StateId;

        private readonly IStateGroupService _stateGroupService;

        public StateProperties()
        {
            _stateGroupService = ServiceLocator.Resolve<IStateGroupService>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                StateId = int.Parse(Request["StateId"]);
                GroupId = Int32.Parse(Request.Params["GroupId"]);

                InitFields(StateRepository.GetById(GroupId, StateId));
            }            
        }

        private void InitFields(BvStateEntity state)
        {
            tbxID.Text = state.StateID.ToString();
            tbxName.Text = state.Name;
            tbxPriority.Value = state.Priority;
            cbDA.Checked = state.DA != 0;
            cbFcdAction.Checked = state.FcdAction;
            tbxAAPOR.Text = state.AaporCode ?? string.Empty;
            if (_stateGroupService.IsSystemState(state))
            {
                tbxName.Enabled = false;
            }
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                UpdateState(tbxName.Text, tbxPriority.ValueInt, cbDA.Checked, cbFcdAction.Checked, tbxAAPOR.Text);

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void UpdateState(string name, int priority, bool disallowActivation, bool notRemoveOnQuotaClose, string AaporCode = null)
        {
            using (var transaction = new DatabaseTransactionScope("Supervisor.UpdateState", DeadlockPriority.Supervisor))
            {
                BvStateEntity state = StateRepository.GetById(GroupId, StateId);

                state.Name = name;
                state.Priority = priority;
                state.DA = disallowActivation ? 1 : 0;
                state.FcdAction = notRemoveOnQuotaClose;
                state.AaporCode = AaporCode;

                StateRepository.Update(state);

                transaction.Commit();
            }
        }
    }
}