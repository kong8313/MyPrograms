using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Core.Filters;
using Confirmit.CATI.Supervisor.Core.Exceptions;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.ServerControls;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Infragistics.Web.UI.GridControls;
using Strings = Confirmit.CATI.Supervisor.Resources.Strings;

namespace Confirmit.CATI.Supervisor.Filter.Controls
{
    /// <summary>
    ///		Summary description for FiltersList.
    /// </summary>
    public partial class FiltersList : BaseWUC
    {
        public int SurveyID { get; set; }
        public BvSurveyEntity Survey { get; set; }

        private readonly IFilterManager _filterManager = ServiceLocator.Resolve<FilterManager>();

        protected void DeleteFilter(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("DeleteFilter", DeadlockPriority.Supervisor))
                {
                    foreach (int filterId in filtersGrid.SelectedKeysInt)
                    {
                        try
                        {
                            _filterManager.DeleteFilter(filterId);
                        }
                        catch (FilterIsUsedException ex)
                        {
                            Page.AddUserMessage(
                                string.Format(
                                    Strings.TheFilterCannotBeDeleted,
                                    string.Join(", ", ex.DependentFilterNames.ToArray())),
                                ex);
                        }
                    }

                    transaction.Commit();

                    OnFiltersListChanged(sender, e);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected object GetPage(out int totalCount)
        {
            var list = FilterRepository.GetFiltersList(cbShowAllFilters.Checked, SurveyID);

            var args = new PagingArgs(filtersGrid.SortedColumnName, filtersGrid.SortIndicatorAsc)
            {
                SearchParameters = filtersGrid.SearchParameterCollection
            };

            return BaseMethods.GetPage(list, args, out totalCount);

        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Items.FindItemByKey("Type").Text = ((BvFiltersEntity)e.Row.DataItem).SurveySID == 0
                ? Strings.SiteSpecific
                : Strings.SurveySpecific;
        }                

        protected void Page_Load(object sender, EventArgs e)
        {
            filtersGrid.GetPage = null;
            filtersGrid.GetPage += GetPage;
            filtersGrid.InitializeRow += Grid_InitializeRow;
            filtersGrid.GridName = string.Format(Strings.FiltersForSurvey, Survey.Description, Survey.Name);

            var column = filtersGrid.Columns.FromKey("Type") as ISearchableField;

            if (column != null)
            {
                column.Items.Add(new ListItem(Strings.SiteSpecific, "0"));
            }
        }

        protected void OnFiltersListChanged(object sender, EventArgs e)
        {
            filtersGrid.RefreshData();

            Page.RegisterStartupScript("Common.fireGlobalEvent('FiltersListChangedEvent');");
        }
    }
}