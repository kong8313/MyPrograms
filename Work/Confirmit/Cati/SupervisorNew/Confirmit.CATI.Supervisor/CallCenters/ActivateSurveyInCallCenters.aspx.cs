using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallCenters;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.CallCenters
{
    public partial class ActivateSurveyInCallCenters : CallCenterBaseForm
    {
        private IEnumerable<BvCallCenterEntity> _allCallCenters;
        private readonly SessionVariable<List<int>> _selectedCallCentersIds = new SessionVariable<List<int>>("_activateSurveysInCallCenters");
        [StoreInViewState]
        protected int SelectedSurveyId;
 
        public override string TopTitle
        {
            get
            {
                return Strings.ActivateSurveyInCallCenters;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                SelectedSurveyId = Int32.Parse(Request["ID"]);
                _selectedCallCentersIds.Value = GetAssignedCallCentersIds();
            }

            _allCallCenters = CallCenterRepository.GetAll();
            _allCallCentersGrid.GetPage = GetAllButSelectedCallCenters;
            _selectedCallCentersGrid.GetPage = GetSelectedCallCenters;
            _doubleGrid.HideRemoveAllButton();
        }

        protected void AddCallCenters(object sender, EventArgs e)
        {
            var keys = _allCallCentersGrid.SelectedKeysInt;
            _selectedCallCentersIds.Value = _selectedCallCentersIds.Value.Union(keys).ToList();
            _allCallCentersGrid.ClearSelectedKeys();
        }

        protected void RemoveCallCenters(object sender, EventArgs e)
        {
            var keys = _selectedCallCentersGrid.SelectedKeysInt;
            _selectedCallCentersIds.Value = _selectedCallCentersIds.Value.Except(keys).ToList();
            _selectedCallCentersGrid.ClearSelectedKeys();
        }

        protected void Activate(object sender, EventArgs e)
        {
            if (ValidateInput(_selectedCallCentersIds.Value) == false)
            {
                return;
            }

            var failedDeactivations = CallCenterService.ReassignSurveys(_selectedCallCentersIds.Value, new[] {SelectedSurveyId});

            if (failedDeactivations.Any())
            {
                _selectedCallCentersIds.Value = GetAssignedCallCentersIds();
                _allCallCentersGrid.RefreshData();
                _selectedCallCentersGrid.RefreshData();

                var message = string.Format(Strings.ErrorSurveyCallCenterDeassignment,
                    string.Join(",", failedDeactivations.Select(item => "'" + CallCenterRepository.Get(item.CallCenterId).Name + "'").ToArray()));
                AddUserMessage(message);
                dialogControl.SetCancelAction(GetCloseOverlayScript(true));
            }
            else
            {
                CloseOverlay(true);
            }
        }

        private List<int> GetAssignedCallCentersIds()
        {
            return CallCenterRepository.GetAssignedToSurvey(SelectedSurveyId).Select(x => x.ID).ToList();
        }

        private IEnumerable<BvCallCenterEntity> GetAllButSelectedCallCenters(out int totalCount)
        {
            var selected =
                _allCallCenters.Where(item => _selectedCallCentersIds.Value.Contains(item.ID) == false).ToList();

            return BaseMethods.GetPage(selected, _allCallCentersGrid.PageArguments, out totalCount);
        }

        private IEnumerable<BvCallCenterEntity> GetSelectedCallCenters(out int totalCount)
        {
            var selected = (from callCenter in _allCallCenters
                            join selectedId in _selectedCallCentersIds.Value on callCenter.ID equals selectedId
                            select callCenter).ToList();

            return BaseMethods.GetPage(selected, _selectedCallCentersGrid.PageArguments, out totalCount);
        }

        private bool ValidateInput(IEnumerable<int> selectedCallCenterIds)
        {
            if (selectedCallCenterIds.Any() == false)
            {
                AddUserMessage(Strings.CallCenterSelectionWarning);
                return false;
            }

            return true;
        }
    }
}