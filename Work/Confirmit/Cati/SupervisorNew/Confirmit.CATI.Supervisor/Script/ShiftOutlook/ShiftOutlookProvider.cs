using System;
using System.Collections;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Resources;

using Infragistics.WebUI.WebSchedule;
using Infragistics.WebUI.Data;
using Infragistics.WebUI.Shared;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Supervisor.Classes.Script;
using ResourceWrapper = Confirmit.CATI.Supervisor.Core.Common.ResourceWrapper;

namespace Confirmit.CATI.Supervisor.Script.ShiftOutlook
{
    public class OutlookProvider : WebScheduleDataProviderBase, IDataFetch, IDataUpdate
    {
        #region Fields
        private readonly ShiftInfo[] m_shifts;
        private readonly ExclusionInfo[] m_exclusions;
        private readonly ShiftTypeCollection m_shiftTypes;
        private DateTime m_WeekStartDate;
        private readonly DayOfWeek m_FirstWeekDay = DayOfWeek.Monday;
        private CurrentDisplay m_CurrentDisplay = CurrentDisplay.Both;
        private List<Appointment> m_Appointments;

        #endregion

        #region Properties

        /// <summary>
        /// Gets start current week date 
        /// </summary>
        DateTime WeekStartDate
        {
            get { return m_WeekStartDate; }
        }

        /// <summary>
        /// Gets first day of week
        /// By default it is Monday, but it also can be Sunday
        /// </summary>
        DayOfWeek FirstWeekDay
        {
            get { return m_FirstWeekDay; }
        }


        #endregion

        public OutlookProvider( ShiftInfo[] shifts,
                            ExclusionInfo[] exclusions, 
                            ShiftTypeCollection shiftTypes, 
                            DateTime weekStartDate, 
                            DayOfWeek firstWeekDay,
                            CurrentDisplay currentDisplay)
        {
            m_shifts = shifts;
            m_exclusions = exclusions;
            m_shiftTypes = shiftTypes;
            m_WeekStartDate = weekStartDate;
            m_FirstWeekDay = firstWeekDay;
            m_CurrentDisplay = currentDisplay;
        }

        #region IDataFetch Members

        public void Fetch( DataContext context )
        {

            if (context.Operation == "FetchActivities")
            {
                FetchActivities( context as FetchActivitiesContext );
            }
        }

        protected void FetchActivities( FetchActivitiesContext context )
        {
            context.Activities.Clear();
            Activity[] activities = GetAppointments().ToArray();

            if (activities.Length > 0)
            {
                foreach (Activity activity in activities)
                {
                    ( (IList)context.Activities ).Add( activity );
                }
            }
        }
        
        public List<Appointment> GetAppointments()
        {
            if (m_Appointments == null)
            {
                m_Appointments = new List<Appointment>();

                m_WeekStartDate = m_WeekStartDate.AddTicks(-1*m_WeekStartDate.TimeOfDay.Ticks);

                DateTime startDateTime;
                Appointment appt;
                DateTime endDateTime;
                if (m_CurrentDisplay == CurrentDisplay.Shifts ||
                    m_CurrentDisplay == CurrentDisplay.Both)
                {
                    foreach (var shiftInfo in m_shifts)
                    {
                        int startDayIndex = SchedulingUtilities.GetDayIndex(shiftInfo.StartDay, FirstWeekDay);
                        startDateTime = WeekStartDate.AddDays(startDayIndex).AddTicks(shiftInfo.StartTime.Ticks);

                        int endDayIndex = SchedulingUtilities.GetDayIndex(shiftInfo.EndDay, FirstWeekDay);

                        //For end date if time is 00:00 it's means previous day, 23:59:59 time
                        //Needed for correct displaying
                        if (shiftInfo.EndTime.Ticks == 0)
                        {
                            endDayIndex = (endDayIndex + 6)%7;
                            endDateTime = WeekStartDate.AddDays(endDayIndex).AddHours(23).AddMinutes(59).AddSeconds(59);
                        }
                        else
                        {
                            endDateTime = WeekStartDate.AddDays(endDayIndex).AddTicks(shiftInfo.EndTime.Ticks);
                        }


                        if (endDateTime.CompareTo(startDateTime) >= 0)
                        {
                            string key = shiftInfo.Id.Value.ToString();
                            appt = GetAppointment(key, startDateTime, endDateTime, shiftInfo.ShiftStatus,
                                                  shiftInfo.ShiftTypeId, shiftInfo.Id.Value);
                            m_Appointments.Add(appt);
                        }
                        else
                        {
                            //in case end date is less than start date we add two appointemnts
                            //one from start of week to end date, second from start date to end of week
                            string key = string.Format("{0}_0", shiftInfo.Id.Value);
                            appt = GetAppointment(key, m_WeekStartDate, endDateTime, shiftInfo.ShiftStatus,
                                                  shiftInfo.ShiftTypeId, shiftInfo.Id.Value);
                            m_Appointments.Add(appt);

                            key = string.Format("{0}_1", shiftInfo.Id.Value);
                            appt = GetAppointment(key, startDateTime, WeekStartDate.AddDays(7), shiftInfo.ShiftStatus,
                                                  shiftInfo.ShiftTypeId, shiftInfo.Id.Value);
                            m_Appointments.Add(appt);
                        }
                    }
                }

                if (m_CurrentDisplay == CurrentDisplay.Exclusions ||
                    m_CurrentDisplay == CurrentDisplay.Both)
                {
                    foreach (ExclusionInfo exclusionInfo in m_exclusions)
                    {
                        startDateTime = exclusionInfo.StartDay.AddTicks(exclusionInfo.StartTime.Ticks);
                        endDateTime = exclusionInfo.EndDay.AddTicks(exclusionInfo.EndTime.Ticks);                        
                        appt = GetAppointment(exclusionInfo.Id.Value.ToString(), startDateTime, endDateTime,
                                              exclusionInfo.ShiftStatus, exclusionInfo.ShiftTypeId,
                                              exclusionInfo.Id.Value);
                        m_Appointments.Add(appt);
                    }
                }
            }
            
            return m_Appointments;
        }     

        protected Appointment GetAppointment( string key, DateTime startDateTime, DateTime endDateTime, ShiftStatus shiftStatus, int shiftTypeId, int shiftId )
        {
            Appointment appt = new Appointment( this.WebScheduleInfo );
            
            appt.StartDateTime = new SmartDate( startDateTime );
            appt.EndDateTime = new SmartDate( endDateTime );
            appt.Importance = Importance.High;
            appt.Location = String.Empty;
            appt.Status = ActivityStatus.Normal;
            appt.Subject = string.Format("{0} :{1}", Strings.Shift, shiftId);
            appt.AllDayEvent = false;

            ShiftType shiftType = m_shiftTypes.GetItemById( shiftTypeId );

            if (!shiftType.IsExclusionType)
            {
                appt.Key = GetResString( "Shift" ) + key;
                appt.DataKey = appt.Key;
                if (shiftStatus == ShiftStatus.Default)
                {
                    appt.ShowTimeAs = ShowTimeAs.Busy;
                }
                else
                {
                    appt.ShowTimeAs = ShowTimeAs.Free;
                }
            }
            else
            {
                appt.Key = GetResString( "Exclusion" ) + key;
                appt.DataKey = appt.Key;
                if (shiftStatus == ShiftStatus.Default)
                {
                    appt.ShowTimeAs = ShowTimeAs.OutofOffice;
                }
                else
                {
                    appt.ShowTimeAs = ShowTimeAs.Tentative;
                }
            }

            appt.Description += "\n" + GetResString( "Status" ) + ": " + shiftStatus.ToString();
            appt.Description += "\n" + GetResString( "ShiftType" ) + ": " + shiftType.Name;
            appt.Style.BackColor = shiftType.Color.Value;

            return appt;
        }

        private string GetResString( string key )
        {
            return ( ResourceWrapper.Instance.GetString( key ) );
        }

        #endregion

        #region IDataUpdate Members

        public void Update( DataContext context )
        {
            throw new NotImplementedException( "The method or operation is not implemented." );
        }

        #endregion
    }
}
