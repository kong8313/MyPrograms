using System;
using Confirmit.CATI.Supervisor.Classes;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;

namespace Confirmit.CATI.Supervisor.ActivityViews
{
    public class ViewInfo
    {
        private readonly string m_Key;
        private readonly string m_Name;
        private readonly string m_Description;

        public string Key { get { return m_Key; } }
        public string Name { get { return m_Name; } }
        public string Description { get { return m_Description; } }
        public ViewInfo(string key, string name, string description)
        {
            m_Key = key;
            m_Name = name;
            m_Description = description;
        }
    }

    public partial class ActivityList: BaseForm
    {
        public override string TopTitle
        {
            get
            {
                return Strings.ActivityViews;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            grid.GetPage += delegate(out int totalCount)
            {
                var src = new List<ViewInfo>
                {
                    new ViewInfo("Surveys", "Survey List", "List of open surveys with summary activity information available"),
                    new ViewInfo("Tasks", "Interviewer List", "List of logged in interviewers with summary activity information available"),
                    new ViewInfo("Appointments", "Appointment List", "List of upcoming appointments"),
                    new ViewInfo("InterviewerPerformance", "Performance List", "List of interviewers that have worked today")
                };
                totalCount = src.Count;
                return src;
            };

            grid.ClientEvents.DoubleClick = "OpenView";

            RegisterScripts();
        }

        private void RegisterScripts()
        {
            var surveysActivityCommand = GetActivityCommand("ActivityViews/SurveysActivityView.aspx");

            RegisterScriptBlock(string.Format("function OpenSurveysActivity(){{{0}}}",
                                          surveysActivityCommand.GetClientEventJavaScript(Page, grid)));

            var taskListCommand = GetActivityCommand("ActivityViews/TaskList.aspx");

            RegisterScriptBlock(string.Format("function OpenTaskList(){{{0}}}",
                           taskListCommand.GetClientEventJavaScript(Page, grid)));

            var appointmentListCommand = GetActivityCommand("ActivityViews/AppointmentList.aspx");

            RegisterScriptBlock(string.Format("function OpenAppointmentList(){{{0}}}",
                           appointmentListCommand.GetClientEventJavaScript(Page, grid)));

            var performanceListCommand = GetActivityCommand("ActivityViews/InterviewerPerformanceList.aspx");

            RegisterScriptBlock(string.Format("function OpenPerformanceList(){{{0}}}",
                           performanceListCommand.GetClientEventJavaScript(Page, grid)));
                       
            var script =
                @"function OpenView(gridID, eventArgs) {
                    if (eventArgs.get_type() != 'cell') // do not process header click
                        return;

                    var index = eventArgs.get_item().get_row().get_index();
                    if (index == 0) {
                        OpenSurveysActivity();
                    }
                    else if (index == 1) {
                        OpenTaskList();
                    }
                    else if (index == 2) {
                        OpenAppointmentList();
                    }
                    else if (index == 3) {
                        OpenPerformanceList();
                    }
                }";

            ClientScript.RegisterClientScriptBlock(Page.GetType(), "OpenView", script, true);
        }

        private ViewCommand GetActivityCommand(string url)
        {
            return new ViewCommand
                       {
                           Width = 1024,
                           Height = 768,
                           Key = "OpenActivity",
                           Caption = "OpenActivity",
                           URL = url
                       };
        }
    }
}