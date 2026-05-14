using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes;

namespace Confirmit.CATI.Supervisor.Reports.Classes
{
    [CheckSurveyPermission(RequestParameterName = "Id", IsRequired = false)]
    public abstract class SurveyReportBase : BaseReportPage
    {
        [StoreInViewState]
        protected int? SurveyId;

        [StoreInViewState]
        protected string SelectedSurveysNames = String.Empty;

        [StoreInViewState]
        protected List<int> SelectedSurveys;

        protected abstract SourceList SourceList { get; }
        protected abstract Button SurveySelectionButton { get; }
        protected abstract CheckBoxList ItsCheckBoxList { get; }
        protected abstract IEnumerable<int> GetSurveysSelectedByUser();
        protected abstract void InitSelectedSurveys(bool isInitial);

        protected readonly ICachedLocalTimezoneManager _timezoneProvider;
        private readonly IUserSurveyListRepository _userSurveyListRepository;

        protected virtual Button PersonSelectionButton { get { return null; } }

        protected SurveyReportBase()
        {
            _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
            _userSurveyListRepository = ServiceLocator.Resolve<IUserSurveyListRepository>();
        }

        protected virtual IEnumerable<int> GetInterviewersSelectedByUser()
        {
            return new int[0];
        }

        protected override void OnLoad(EventArgs e)
        {
            if (IsPostBack == false)
            {
                if (Request["Id"] != null)
                {
                    SurveyId = Convert.ToInt32(Request["Id"]);
                    new SurveyIdsSessionProvider().SetSelectedSurveyIds(SourceList, new[] { SurveyId.Value });
                }

                InitSelectedSurveys(true);

                if (ItsCheckBoxList != null)
                {
                    BindIts(false);
                }

                if (SurveyId.HasValue)
                {
                    _userSurveyListRepository.Insert(UserSurveyListType.Recent, (int)SurveyId);
                }
            }

            /* If it's async postback from one of IG update panels, don't bind report.
               Needed to handle the actions: BuildReport, Export, Paging  */
            if (IsPostBack && !IsAsyncPostback)
            {
                BuildReport();
            }

            InitSurveySelectionClientClickHandler();
            InitPersonSelectionClientClickHandler();

            base.OnLoad(e);
        }

        private void InitSurveySelectionClientClickHandler()
        {
            var selectedSurveyId = IsPostBack ? null : SurveyId;
            var postBackReference = string.Format("\"{0}\"", ClientScript.GetPostBackEventReference(this, _SurveysSelected));
            SurveySelectionButton.OnClientClick = SurveysSelectionScriptProvider.Get(SourceList, selectedSurveyId, postBackReference);
            SurveysSelectedByUser += SurveyReportBase_SurveysSelected;

            HighlightSurveySelectionButton();
        }

        private void InitPersonSelectionClientClickHandler()
        {
            if (PersonSelectionButton == null)
                return;

            var postBackReference = string.Format("\"{0}\"", ClientScript.GetPostBackEventReference(this, _PersonsSelected));
            PersonSelectionButton.OnClientClick = InterviewersSelectionScriptProvider.Get(SourceList, UpdatePanel.ClientID, null, postBackReference);
            PersonsSelectedByUser += PersonReportBase_SurveysSelected;

            HighlightPersonSelectionButton();
        }

        private void SurveyReportBase_SurveysSelected(object sender, EventArgs e)
        {
            InitSelectedSurveys(false);

            if (ItsCheckBoxList != null)
            {
                BindIts(true);
            }

            HighlightSurveySelectionButton();
        }

        private void PersonReportBase_SurveysSelected(object sender, EventArgs e)
        {
            HighlightPersonSelectionButton();
        }

        private void HighlightSurveySelectionButton()
        {
            SurveySelectionButton.CssClass = SelectedSurveys.Count() > 0 && SelectedSurveysNames != "All"
                ? "comd-button--selected"
                : "";
        }

        private void HighlightPersonSelectionButton()
        {
            PersonSelectionButton.CssClass = GetInterviewersSelectedByUser().Count() > 0
                ? "comd-button--selected"
                : "";
        }

        protected virtual void BindIts(bool keepSelected)
        {
            if (SelectedSurveys.Count() == 1)
            {
                BindITS(SurveyService.GetTransientStates(SelectedSurveys.First()), keepSelected);
            }
            else
            {
                BindITS(StateGroupsManager.GetDefaultITSList(), keepSelected);
            }
        }

        private void BindITS(IEnumerable<BvSpState_ListEntity> dataSource, bool keepSelected)
        {
            var selectedItsIDs = ItsCheckBoxList.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList();

            ItsCheckBoxList.Items.Clear();

            foreach (var its in dataSource)
            {
                ItsCheckBoxList.Items.Add(new ListItem(its.Name, its.StateID.ToString()) { Selected = IsItsSelectedByDefault(its) }); // Only completed ITS should be selected by default.
            }

            if (keepSelected)
            {
                foreach (ListItem item in ItsCheckBoxList.Items)
                {
                    item.Selected = selectedItsIDs.Contains(item.Value);
                }
            }
        }

        protected virtual bool IsItsSelectedByDefault(BvSpState_ListEntity its)
        {
            return its.StateID == CompletedItsId;
        }

        protected string GetSelectedInterviewersNames(IEnumerable<int> interviewersIds)
        {
            var list = PersonManager.GetPersonList();

            var names = list.Where(x => interviewersIds.Contains(x.Id))
                            .Take(MaxNamesCount).Select(x => x.Name).Distinct();

            return ReportTools.MakeArrayStringEx(names, MaxLineLength, 2);
        }
    }
}
