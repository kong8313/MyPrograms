using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Controls
{
    public partial class DoubleSurveysGrid : BaseWUC
    {
        private List<SurveyInfoItem> _allSurveys;

        private readonly SessionVariable<int[]> _selectedSurveysIds =
                                new SessionVariable<int[]>("_doubleSurveyGridSelectedSurveysIds");

        public bool UseOnlyOpenSurveys { get; set; }

        public bool SingleSelectionMode { get; set; }

        public int[] SelectedSurveysIds
        {
            get { return _selectedSurveysIds.Value; }
            set { _selectedSurveysIds.Value = value != null ? value.ToArray() : null; }
        }

        private IEnumerable<SurveyInfoItem> AllSurveys
        {
            get
            {
                if (_allSurveys == null)
                {
                    _allSurveys = UseOnlyOpenSurveys ?
                        SurveyManager.GetOpenSurveys(User.Name, String.Empty) :
                        SurveyManager.GetSurveys(User.Name, string.Empty);
                }

                return _allSurveys;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            selectedSurveysGrid.GetPage += GetSelectedSurveysListPage;
            allSurveysGrid.GetPage += GetAllButSelectedSurveysListPage;

            if (SelectedSurveysIds == null)
            {
                SelectedSurveysIds = new int[0];
            }

            allSurveysGrid.HideSelectedColumn = SingleSelectionMode;
            selectedSurveysGrid.HideSelectedColumn = SingleSelectionMode;

            miAddOpenSurveys.Visible = !SingleSelectionMode;
            allSurveysGrid.TopToolbarLayout = ToolbarLayout.LabelAndMenu;

            allSurveysGrid.GridName = UseOnlyOpenSurveys ? 
                                       Strings.DoubleSurveysGrid_OpenSurveys : 
                                       Strings.DoubleSurveysGrid_AvailableSurveys;
        }        
        
        protected void AddSurveys(object sender, EventArgs e)
        {
            var keys = allSurveysGrid.SelectedKeysInt;

            if (!keys.Any()) return;

            SelectedSurveysIds = (SingleSelectionMode ? keys : SelectedSurveysIds.Union(keys)).ToArray();
            allSurveysGrid.ClearSelectedKeys();
        }

        protected void AddAllOpenSurveys(object sender, EventArgs e)
        {
            var keys = SurveyManager.GetOpenSurveys(User.Name, string.Empty).Select(x => x.Id).ToArray();

            if (!keys.Any()) return;

            SelectedSurveysIds = (SingleSelectionMode ? keys : SelectedSurveysIds.Union(keys)).ToArray();

            allSurveysGrid.ClearSelectedKeys();
        }

        protected void RemoveSurveys(object sender, EventArgs e)
        {
            var keys = selectedSurveysGrid.SelectedKeysInt;

            if (!keys.Any()) return;

            SelectedSurveysIds = SelectedSurveysIds.Except(keys).ToArray();
            selectedSurveysGrid.ClearSelectedKeys();
        }

        protected void RemoveAll(object sender, EventArgs e)
        {
            SelectedSurveysIds = Array.Empty<int>();
            selectedSurveysGrid.ClearSelectedKeys();
        }
                                   
        protected object GetAllButSelectedSurveysListPage(out int totalCount)
        {
            var list = AllSurveys.Where(x => !SelectedSurveysIds.Contains(x.Id)).ToList();

            return BaseMethods.GetPage(list, allSurveysGrid.PageArguments, out totalCount);
        }

        protected object GetSelectedSurveysListPage(out int totalCount)
        {
            var list = AllSurveys.Where(x => SelectedSurveysIds.Contains(x.Id));

            return BaseMethods.GetPage(list, selectedSurveysGrid.PageArguments, out totalCount);
        }             
    }
}
