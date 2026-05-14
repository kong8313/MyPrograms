using System;
using System.Linq;
using System.Web.Services.Description;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Persons.Controls
{
    public partial class SelectSurvey : BaseWUC
    {
        private readonly ICallCenterProvider _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
        private IAssignmentManager _assignmentManager;

        #region Properties

        /// <summary>
        /// Gets/sets person identifier. If property is null, it means that we should
        /// fill survey list with all surveys supervisor is in charge of. Otherwise 
        /// we fill survey list with survey assigned to person.
        /// </summary>
        public int? PersonId
        {
            get
            {
                return (int?)ViewState["PersonId"];
            }
            set
            {
                ViewState["PersonId"] = value;
            }
        }

        /// <summary>
        /// Gets selected survey identifier.
        /// </summary>
        public int? SelectedSurveyId
        {
            get
            {
                int? result = null;
                string key = gridSurveys.SelectedKeys.FirstOrDefault();
                if (!String.IsNullOrEmpty(key))
                {
                    result = Int32.Parse(key);
                }

                return result;
            }
        }

        #endregion

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            InitGridDataSource();
            gridSurveys.GridName = "Select automatic survey";
        }

        #endregion

        #region Methods

        public SelectSurvey()
        {
            _assignmentManager = ServiceLocator.Resolve<IAssignmentManager>();
        }

        /// <summary>
        /// Initializes control data sources and fills it with proper data.
        /// </summary>
        public void Bind()
        {
            InitGridDataSource();
            gridSurveys.BindData();
        }

        /// <summary>
        /// Initializes grid data source according PersonId property value.
        /// If property is null we fill grid with all supervisor surveys, otherwise
        /// we take only person assigned surveys.
        /// </summary>
        private void InitGridDataSource()
        {
            if (PersonId.HasValue)
            {
                gridSurveys.GetPage += GetPersonAssignedSurveys;
            }
            else
            {
                gridSurveys.GetPage += GetAllSurveys;
            }
        }

        /// <summary>
        /// Returns all surveys for current supervisor.
        /// </summary>
        /// <param name="totalCount">Returns total count of surveys.</param>
        /// <returns>List of surveys.</returns>
        private object GetAllSurveys(out int totalCount)
        {
            PagingArgs pagingArgs = new PagingArgs(
                gridSurveys.PageIndex,
                gridSurveys.PageSize,
                gridSurveys.SortedColumnKey,
                gridSurveys.SortIndicatorAsc,
                gridSurveys.SearchParameterCollection);

            return SurveyRepository.GetPage(_callCenterProvider.GetCurrentId(), pagingArgs, User.Name, out totalCount);
        }

        /// <summary>
        /// Returns surveys assigned to person.
        /// </summary>
        /// <param name="totalCount">Returns total count of surveys.</param>
        /// <returns>List of surveys.</returns>
        private object GetPersonAssignedSurveys(out int totalCount)
        {
            var list = _assignmentManager.GetAssignedSurveyList(PersonId.Value, User.Name)                
                 .Select(x => new
                 {
                     SID = x.SurveySID,
                     Name = x.ProjectID,
                     Description = x.ProjectName
                 }).Distinct();            

            PagingArgs args = new PagingArgs(
                gridSurveys.PageIndex,
                gridSurveys.PageSize,
                gridSurveys.SortedColumnName,
                gridSurveys.SortIndicatorAsc,
                gridSurveys.SearchParameterCollection
            );

            return BaseMethods.GetPage(list, args, out totalCount);
        }
        
        #endregion
    }
}