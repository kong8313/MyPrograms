using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    [CheckSurveyPermission(RequestParameterName = "SurveyID")]
    public partial class AudioPlayer : BaseForm
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider =
            ServiceLocator.Resolve<ICachedLocalTimezoneManager>();

        private readonly ISupervisorServiceClient _supervisorServiceClient =
            ServiceLocator.Resolve<ISupervisorServiceClient>();

        [StoreInViewState]
        public int InterviewId;

        [StoreInViewState]
        public int SurveyId;

        public override string Title
        {
            get { return Strings.AudioPlayer; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            dialogControl.CancelButton.InnerText = "Close";

            if (!IsPostBack)
            {
                InterviewId = Int32.Parse(Request["InterviewID"]);
                SurveyId = Int32.Parse(Request["SurveyID"]);
            }

            m_grid.GetPage += delegate(out int totalCount)
            {
                totalCount = 0;
                IEnumerable<AudioRecordInfo> data;
                try
                {
                    data = _supervisorServiceClient.GetInterviewRecordings(SurveyId, InterviewId).OrderBy(x => x.DateTime);
                }
                catch (Exception ex)
                {
                    //In previous version of InterviewRecordingManager().GetInterviewRecordings eats exceptions.
                    //Now we have to do it here. Is it worth to show a message to a user that a dialer returns an exception?
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                    data = null;
                }

                if (data == null || !data.Any())
                {
                    CloseWindowEx(String.Format("alert('{0}')", Strings.NoAudioAvailable));
                }
                else
                {
                    RegisterStartupScript(String.Format("playRecording('{0}',true);", data.First().Url));
                }

                return data;
            };

            m_grid.RowDataBound += m_grid_RowDataBound;
            var survey = SurveyRepository.GetById(SurveyId);
            dialogControl.Title = string.Format("Survey: {0}  Interview: {1}", survey.Name, InterviewId);

            bool isIE = Request.Browser.Browser.Equals("IE", StringComparison.OrdinalIgnoreCase);

            // IE8 doesn't support audio tag, IE9 doesn't support WAV format in audio tag. So we have to always use ActiveX for IE.
            if (isIE)
            {
                ActivexPlayer.Visible = true;
                Html5Player.Visible = false;
            }
            else
            {
                ActivexPlayer.Visible = false;
                Html5Player.Visible = true;
            }
        }

        protected void m_grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Collect columns indexes.
            Dictionary<string, int> columns = m_grid.Columns.OfType<BoundField>().ToDictionary(
                field => field.DataField,
                field => m_grid.Columns.IndexOf(field));

            if (e.Row.RowType == DataControlRowType.DataRow)
            { 
                var info = (AudioRecordInfo)e.Row.DataItem;

                var ibPlay = (ServerControls.ImageButton)e.Row.FindControl("ibPlay");
                ibPlay.OnClientClick = String.Format("playRecording('{0}');return false;", info.Url);

                var dateTimeCell = e.Row.Cells[columns["DateTime"]];
                dateTimeCell.Text = _timezoneProvider.ConvertToLocalTime(info.DateTime).ToString();
                dateTimeCell.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");

                string fileName = new Uri(info.Url).Segments.LastOrDefault();
                var lbDownload = (HyperLink)e.Row.FindControl("lbDownload");
                lbDownload.Text = fileName;
                lbDownload.NavigateUrl = info.Url;
            }
        }
    }
}
