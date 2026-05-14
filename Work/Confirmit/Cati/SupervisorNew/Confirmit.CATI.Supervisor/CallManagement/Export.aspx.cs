using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Core.Export;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    public partial class Export : BaseForm
    {
        private ISystemSettings _systemSettings;

        private int CallsLimit => _systemSettings.CallManagement.ExportCallsLimit;

        /// <summary>
        /// Gets/sets page index in call list. This value is taken from session by unique call list page identifier.
        /// Value should be previously stored in session by call list.
        /// </summary>
        private int PageIndex
        {
            get
            {
                var key = GetPropertyKey("PageIndex");
                return Session[key] == null ? 0 : (int)Session[key];
            }
            set
            {
                Session[GetPropertyKey("PageIndex")] = value;
            }
        }

        /// <summary>
        /// Gets/sets page size in call list. This value is taken from session by unique call list page identifier.
        /// Value should be previously stored in session by call list.
        /// </summary>
        private int PageSize
        {
            get
            {
                var key = GetPropertyKey("PageSize");
                return Session[key] == null ? 0 : (int)Session[key];
            }
            set
            {
                Session[GetPropertyKey("PageSize")] = value;
            }
        }

        /// <summary>
        /// Gets/sets total count of calls in call list. This value is taken from session by unique call list page identifier.
        /// Value should be previously stored in session by call list.
        /// </summary>
        public int TotalCount
        {
            get
            {
                var key = GetPropertyKey("TotalCount");
                return Session[key] == null ? 0 : (int)Session[key];
            }
            set
            {
                Session[GetPropertyKey("TotalCount")] = value;
            }
        }

        /// <summary>
        /// Gets/sets unique identifier of call list page which opens current page.
        /// </summary>
        private Guid CallListInstanceId
        {
            get
            {
                if (ViewState["CallListInstanceId"] == null)
                {
                    ViewState["CallListInstanceId"] = Guid.Empty;
                }

                return (Guid)ViewState["CallListInstanceId"];
            }
            set
            {
                ViewState["CallListInstanceId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _systemSettings = ServiceLocator.Resolve<ISystemSettings>();

            if (!IsPostBack)
            {
                CallListInstanceId = new Guid(Request.Params["CallListInstanceId"]);
                pageIndex.Value = PageIndex.ToString();
                pageSize.Value = PageSize.ToString();
                totalCount.Value = TotalCount.ToString();

                SetPagesRange();
            }

            rbCurrentPage.Attributes.Add("onclick", "radioButtonClicked();");
            rbAllPages.Attributes.Add("onclick", "radioButtonClicked();");
            rbRangePages.Attributes.Add("onclick", "radioButtonClicked();");
        }

        protected void OnExportClick(object sender, EventArgs e)
        {
            ExportList();
        }

        /// <summary>
            /// Exports call list.
            /// </summary>
            private void ExportList()
        {
            var scriptKey = "ExportDialog";

            if (neStart.ValueInt > neEnd.ValueInt)
            {
                // start index greate than end
                ClientScript.RegisterClientScriptBlock(
                    GetType(),
                    scriptKey,
                    $"alert('{Strings.WrongIntervalMessage}');",
                    true
                );
            }
            else if (int.TryParse(currentCallsCount.Value, out var calls) && calls > CallsLimit)
            {
                // too many calls
                ClientScript.RegisterClientScriptBlock(
                    GetType(), 
                    scriptKey,
                    $"alert('{GetResString("TooManyExportRecordsMessage", CallsLimit)}');", 
                    true
                );
            }
            else
            {
                var returnValue = string.Empty;
                if (rbAllPages.Checked == true)
                {
                    returnValue = "all";
                }
                else if (rbCurrentPage.Checked == true)
                {
                    returnValue = "current";
                }
                else if (rbRangePages.Checked == true)
                {
                    returnValue = $"{neStart.ValueInt},{neEnd.ValueInt}";
                }

                CloseOverlay(true, returnValue);
            }
        }

        /// <summary>
        /// Initializes page range in numeric controls.
        /// </summary>
        private void SetPagesRange()
        {
            var maxValue = ExportManager.GetMaxValueForPageRange(PageSize, TotalCount);
            neStart.MaxValue = maxValue;
            neEnd.MaxValue = maxValue;
        }

        /// <summary>
        /// Generates key value for properties stored in session.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Key.</returns>
        private string GetPropertyKey(string propertyName)
        {
            return CallListInstanceId + propertyName;
        }
    }
}
