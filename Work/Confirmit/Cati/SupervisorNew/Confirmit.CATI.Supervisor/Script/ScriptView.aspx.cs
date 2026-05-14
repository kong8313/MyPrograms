using System;
using System.Text;
using System.Web.UI;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Script.Classes;

namespace Confirmit.CATI.Supervisor.Script
{
    public partial class ScriptView : BaseForm
    {
        private BvScheduleEntity m_schedule;

        /// <summary>
        /// Use SessionPageStatePersister as PageStatePersister to store
        /// viewstate data in session.
        /// </summary>
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new SessionPageStatePersister(this);
            }
        }

        public override string Title
        {
            get { return Strings.ScriptProperties; }
        }

        protected int ScheduleID
        {
            get { return Convert.ToInt32(Request.QueryString["ID"]); }
        }

        protected BvScheduleEntity Schedule
        {
            get
            {
                if (m_schedule == null)
                {
                    m_schedule = ScheduleRepository.GetById(ScheduleID);
                }

                return m_schedule;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSchedule();
                CleanChanged();
            }

            //dialog.Title = string.Format("Script \"{0}\" using extended status group \"{1}\"", 
            //    Schedule.Name, StateGroupsManager.GetStateGroupForScript(Schedule).Name);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
        }

        protected void LoadSchedule()
        {
            string xmlSchedule = ScheduleRepository.GetByIdWithCheck(Schedule.ScheduleID).XmlUnderDev;
            Schedule schedule;

            if (!string.IsNullOrEmpty(xmlSchedule))
            {
                schedule = ScheduleManager.DeserializeSchedule(xmlSchedule);
            }
            else
            {
                schedule = new Schedule();
            }

            schedule.Id = Schedule.ScheduleID;
            Session[$"WorkingSchedule_{Schedule.ScheduleID}"] = schedule;
            Session[$"ScheduleChanged_{Schedule.ScheduleID}"] = false;
        }
    }
}
