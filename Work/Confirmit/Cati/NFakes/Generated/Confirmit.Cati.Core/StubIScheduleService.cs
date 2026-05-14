using System;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ScheduleDom.Script;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIScheduleService : IScheduleService 
    {
        private IScheduleService _inner;

        public StubIScheduleService()
        {
            _inner = null;
        }

        public IScheduleService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CheckStringDelegate(string serializedSchedule);
        public CheckStringDelegate CheckString;

        void IScheduleService.Check(string serializedSchedule)
        {

            if (CheckString != null)
            {
                CheckString(serializedSchedule);
            } else if (_inner != null)
            {
                ((IScheduleService)_inner).Check(serializedSchedule);
            }
        }

        public delegate void CheckScheduleDelegate(Schedule scheduleInDev);
        public CheckScheduleDelegate CheckSchedule;

        void IScheduleService.Check(Schedule scheduleInDev)
        {

            if (CheckSchedule != null)
            {
                CheckSchedule(scheduleInDev);
            } else if (_inner != null)
            {
                ((IScheduleService)_inner).Check(scheduleInDev);
            }
        }

        public delegate bool CheckParamValueScheduleInt32SchedulingParameterTypeInt32Delegate(Schedule schedule, int surveySid, SchedulingParameterType type, int value);
        public CheckParamValueScheduleInt32SchedulingParameterTypeInt32Delegate CheckParamValueScheduleInt32SchedulingParameterTypeInt32;

        bool IScheduleService.CheckParamValue(Schedule schedule, int surveySid, SchedulingParameterType type, int value)
        {


            if (CheckParamValueScheduleInt32SchedulingParameterTypeInt32 != null)
            {
                return CheckParamValueScheduleInt32SchedulingParameterTypeInt32(schedule, surveySid, type, value);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).CheckParamValue(schedule, surveySid, type, value);
            }

            return default(bool);
        }

        public delegate bool CheckParamValueInt32Int32SchedulingParameterTypeInt32Delegate(int scheduleId, int surveySid, SchedulingParameterType type, int value);
        public CheckParamValueInt32Int32SchedulingParameterTypeInt32Delegate CheckParamValueInt32Int32SchedulingParameterTypeInt32;

        bool IScheduleService.CheckParamValue(int scheduleId, int surveySid, SchedulingParameterType type, int value)
        {


            if (CheckParamValueInt32Int32SchedulingParameterTypeInt32 != null)
            {
                return CheckParamValueInt32Int32SchedulingParameterTypeInt32(scheduleId, surveySid, type, value);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).CheckParamValue(scheduleId, surveySid, type, value);
            }

            return default(bool);
        }

        public delegate bool CheckParamValueScheduleInt32SchedulingParameterTypeInt32StringOutDelegate(Schedule schedule, int surveySid, SchedulingParameterType type, int value, out string reason);
        public CheckParamValueScheduleInt32SchedulingParameterTypeInt32StringOutDelegate CheckParamValueScheduleInt32SchedulingParameterTypeInt32StringOut;

        bool IScheduleService.CheckParamValue(Schedule schedule, int surveySid, SchedulingParameterType type, int value, out string reason)
        {
            reason = default(string);


            if (CheckParamValueScheduleInt32SchedulingParameterTypeInt32StringOut != null)
            {
                return CheckParamValueScheduleInt32SchedulingParameterTypeInt32StringOut(schedule, surveySid, type, value, out reason);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).CheckParamValue(schedule, surveySid, type, value, out reason);
            }

            return default(bool);
        }

        public delegate bool CheckParamValueInt32Int32SchedulingParameterTypeInt32StringOutDelegate(int scheduleId, int surveySid, SchedulingParameterType type, int value, out string reason);
        public CheckParamValueInt32Int32SchedulingParameterTypeInt32StringOutDelegate CheckParamValueInt32Int32SchedulingParameterTypeInt32StringOut;

        bool IScheduleService.CheckParamValue(int scheduleId, int surveySid, SchedulingParameterType type, int value, out string reason)
        {
            reason = default(string);


            if (CheckParamValueInt32Int32SchedulingParameterTypeInt32StringOut != null)
            {
                return CheckParamValueInt32Int32SchedulingParameterTypeInt32StringOut(scheduleId, surveySid, type, value, out reason);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).CheckParamValue(scheduleId, surveySid, type, value, out reason);
            }

            return default(bool);
        }

        public delegate bool CheckParamValueInt32Int32Int32Int32StringOutDelegate(int scheduleId, int surveySID, int paramID, int value, out string reason);
        public CheckParamValueInt32Int32Int32Int32StringOutDelegate CheckParamValueInt32Int32Int32Int32StringOut;

        bool IScheduleService.CheckParamValue(int scheduleId, int surveySID, int paramID, int value, out string reason)
        {
            reason = default(string);


            if (CheckParamValueInt32Int32Int32Int32StringOut != null)
            {
                return CheckParamValueInt32Int32Int32Int32StringOut(scheduleId, surveySID, paramID, value, out reason);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).CheckParamValue(scheduleId, surveySID, paramID, value, out reason);
            }

            return default(bool);
        }

        public delegate bool DoesSheduleHaveParametersInUseInt32Delegate(int scheduleId);
        public DoesSheduleHaveParametersInUseInt32Delegate DoesSheduleHaveParametersInUseInt32;

        bool IScheduleService.DoesSheduleHaveParametersInUse(int scheduleId)
        {


            if (DoesSheduleHaveParametersInUseInt32 != null)
            {
                return DoesSheduleHaveParametersInUseInt32(scheduleId);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).DoesSheduleHaveParametersInUse(scheduleId);
            }

            return default(bool);
        }

        public delegate IActionCollection GetActionsDelegate();
        public GetActionsDelegate GetActions;

        IActionCollection IScheduleService.GetActions()
        {


            if (GetActions != null)
            {
                return GetActions();
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).GetActions();
            }

            return default(IActionCollection);
        }

        public delegate IEnumerable<SchedulingParameterType> GetMatchingTypesSchedulingParameterTypeDelegate(SchedulingParameterType actionType);
        public GetMatchingTypesSchedulingParameterTypeDelegate GetMatchingTypesSchedulingParameterType;

        IEnumerable<SchedulingParameterType> IScheduleService.GetMatchingTypes(SchedulingParameterType actionType)
        {


            if (GetMatchingTypesSchedulingParameterType != null)
            {
                return GetMatchingTypesSchedulingParameterType(actionType);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).GetMatchingTypes(actionType);
            }

            return default(IEnumerable<SchedulingParameterType>);
        }

        public delegate int GetParamValueInt32Int32StringDelegate(int scheduleID, int surveySID, string name);
        public GetParamValueInt32Int32StringDelegate GetParamValueInt32Int32String;

        int IScheduleService.GetParamValue(int scheduleID, int surveySID, string name)
        {


            if (GetParamValueInt32Int32String != null)
            {
                return GetParamValueInt32Int32String(scheduleID, surveySID, name);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).GetParamValue(scheduleID, surveySID, name);
            }

            return default(int);
        }

        public delegate int GetParamValueInt32Int32Int32Delegate(int scheduleID, int surveySID, int paramID);
        public GetParamValueInt32Int32Int32Delegate GetParamValueInt32Int32Int32;

        int IScheduleService.GetParamValue(int scheduleID, int surveySID, int paramID)
        {


            if (GetParamValueInt32Int32Int32 != null)
            {
                return GetParamValueInt32Int32Int32(scheduleID, surveySID, paramID);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).GetParamValue(scheduleID, surveySID, paramID);
            }

            return default(int);
        }

        public delegate List<BvSpShift_ListEntity> GetShiftListInt32Int32Int32Delegate(int sid, int shiftId, int timezoneId);
        public GetShiftListInt32Int32Int32Delegate GetShiftListInt32Int32Int32;

        List<BvSpShift_ListEntity> IScheduleService.GetShiftList(int sid, int shiftId, int timezoneId)
        {


            if (GetShiftListInt32Int32Int32 != null)
            {
                return GetShiftListInt32Int32Int32(sid, shiftId, timezoneId);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).GetShiftList(sid, shiftId, timezoneId);
            }

            return default(List<BvSpShift_ListEntity>);
        }

        public delegate List<BvSpShiftType_ListEntity> GetShiftTypeListInt32Delegate(int sid);
        public GetShiftTypeListInt32Delegate GetShiftTypeListInt32;

        List<BvSpShiftType_ListEntity> IScheduleService.GetShiftTypeList(int sid)
        {


            if (GetShiftTypeListInt32 != null)
            {
                return GetShiftTypeListInt32(sid);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).GetShiftTypeList(sid);
            }

            return default(List<BvSpShiftType_ListEntity>);
        }

        public delegate string GetValidActionsXmlDelegate();
        public GetValidActionsXmlDelegate GetValidActionsXml;

        string IScheduleService.GetValidActionsXml()
        {


            if (GetValidActionsXml != null)
            {
                return GetValidActionsXml();
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).GetValidActionsXml();
            }

            return default(string);
        }

        public delegate void LaunchInt32Delegate(int sid);
        public LaunchInt32Delegate LaunchInt32;

        void IScheduleService.Launch(int sid)
        {

            if (LaunchInt32 != null)
            {
                LaunchInt32(sid);
            } else if (_inner != null)
            {
                ((IScheduleService)_inner).Launch(sid);
            }
        }

        public delegate void ReGenerateScriptBvScheduleEntityDelegate(BvScheduleEntity scheduleEntity);
        public ReGenerateScriptBvScheduleEntityDelegate ReGenerateScriptBvScheduleEntity;

        void IScheduleService.ReGenerateScript(BvScheduleEntity scheduleEntity)
        {

            if (ReGenerateScriptBvScheduleEntity != null)
            {
                ReGenerateScriptBvScheduleEntity(scheduleEntity);
            } else if (_inner != null)
            {
                ((IScheduleService)_inner).ReGenerateScript(scheduleEntity);
            }
        }

        public delegate void SaveInt32StringDelegate(int scheduleId, string serializedSchedule);
        public SaveInt32StringDelegate SaveInt32String;

        void IScheduleService.Save(int scheduleId, string serializedSchedule)
        {

            if (SaveInt32String != null)
            {
                SaveInt32String(scheduleId, serializedSchedule);
            } else if (_inner != null)
            {
                ((IScheduleService)_inner).Save(scheduleId, serializedSchedule);
            }
        }

        public delegate void SetParamValueInt32Int32Int32Delegate(int surveySID, int paramID, int value);
        public SetParamValueInt32Int32Int32Delegate SetParamValueInt32Int32Int32;

        void IScheduleService.SetParamValue(int surveySID, int paramID, int value)
        {

            if (SetParamValueInt32Int32Int32 != null)
            {
                SetParamValueInt32Int32Int32(surveySID, paramID, value);
            } else if (_inner != null)
            {
                ((IScheduleService)_inner).SetParamValue(surveySID, paramID, value);
            }
        }

        public delegate Schedule XmlDeserializeStringDelegate(string serializedSchedule);
        public XmlDeserializeStringDelegate XmlDeserializeString;

        Schedule IScheduleService.XmlDeserialize(string serializedSchedule)
        {


            if (XmlDeserializeString != null)
            {
                return XmlDeserializeString(serializedSchedule);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).XmlDeserialize(serializedSchedule);
            }

            return default(Schedule);
        }

        public delegate string XmlSerializeScheduleDelegate(Schedule schedule);
        public XmlSerializeScheduleDelegate XmlSerializeSchedule;

        string IScheduleService.XmlSerialize(Schedule schedule)
        {


            if (XmlSerializeSchedule != null)
            {
                return XmlSerializeSchedule(schedule);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).XmlSerialize(schedule);
            }

            return default(string);
        }

        public delegate Rule GetSampleUpdateRuleScheduleDelegate(Schedule schedule);
        public GetSampleUpdateRuleScheduleDelegate GetSampleUpdateRuleSchedule;

        Rule IScheduleService.GetSampleUpdateRule(Schedule schedule)
        {


            if (GetSampleUpdateRuleSchedule != null)
            {
                return GetSampleUpdateRuleSchedule(schedule);
            } else if (_inner != null)
            {
                return ((IScheduleService)_inner).GetSampleUpdateRule(schedule);
            }

            return default(Rule);
        }

        public delegate void CopySchedulingScriptToDefaultInt32DateTimeDelegate(int customScriptId, DateTime clientTime);
        public CopySchedulingScriptToDefaultInt32DateTimeDelegate CopySchedulingScriptToDefaultInt32DateTime;

        void IScheduleService.CopySchedulingScriptToDefault(int customScriptId, DateTime clientTime)
        {

            if (CopySchedulingScriptToDefaultInt32DateTime != null)
            {
                CopySchedulingScriptToDefaultInt32DateTime(customScriptId, clientTime);
            } else if (_inner != null)
            {
                ((IScheduleService)_inner).CopySchedulingScriptToDefault(customScriptId, clientTime);
            }
        }

        public delegate void DeleteSchedulingScriptsIEnumerableOfInt32Delegate(IEnumerable<int> scriptIdList);
        public DeleteSchedulingScriptsIEnumerableOfInt32Delegate DeleteSchedulingScriptsIEnumerableOfInt32;

        void IScheduleService.DeleteSchedulingScripts(IEnumerable<int> scriptIdList)
        {

            if (DeleteSchedulingScriptsIEnumerableOfInt32 != null)
            {
                DeleteSchedulingScriptsIEnumerableOfInt32(scriptIdList);
            } else if (_inner != null)
            {
                ((IScheduleService)_inner).DeleteSchedulingScripts(scriptIdList);
            }
        }

        private int _DefaultScheduleId;
        public Func<int> DefaultScheduleIdGet;
        public Action<int> DefaultScheduleIdSetInt32;

        int IScheduleService.DefaultScheduleId
        {
            get
            {
                if (DefaultScheduleIdGet != null)
                {
                    return DefaultScheduleIdGet();
                } else if (_inner != null)
                {
                    return ((IScheduleService)_inner).DefaultScheduleId;
                }

                if (DefaultScheduleIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DefaultScheduleId;
                }

                return default(int);
            }

        }

    }
}