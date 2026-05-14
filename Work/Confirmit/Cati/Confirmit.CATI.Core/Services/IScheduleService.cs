using System;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Script;

namespace Confirmit.CATI.Core.Services
{
    public interface IScheduleService
    {
        int DefaultScheduleId { get; }

        void Check(string serializedSchedule);
        void Check(Schedule scheduleInDev);
        bool CheckParamValue(Schedule schedule, int surveySid, SchedulingParameterType type, int value);
        bool CheckParamValue(int scheduleId, int surveySid, SchedulingParameterType type, int value);
        bool CheckParamValue(Schedule schedule, int surveySid, SchedulingParameterType type, int value, out string reason);
        bool CheckParamValue(int scheduleId, int surveySid, SchedulingParameterType type, int value, out string reason);
        bool CheckParamValue(int scheduleId, int surveySID, int paramID, int value, out string reason);
        bool DoesSheduleHaveParametersInUse(int scheduleId);
        IActionCollection GetActions();
        IEnumerable<SchedulingParameterType> GetMatchingTypes(SchedulingParameterType actionType);
        int GetParamValue(int scheduleID, int surveySID, string name);
        int GetParamValue(int scheduleID, int surveySID, int paramID);
        List<BvSpShift_ListEntity> GetShiftList(int sid, int shiftId, int timezoneId);
        List<BvSpShiftType_ListEntity> GetShiftTypeList(int sid);
        string GetValidActionsXml();
        void Launch(int sid);
        void ReGenerateScript(BvScheduleEntity scheduleEntity);
        void Save(int scheduleId, string serializedSchedule);
        void SetParamValue(int surveySID, int paramID, int value);
        Schedule XmlDeserialize(string serializedSchedule);
        string XmlSerialize(Schedule schedule);
        Rule GetSampleUpdateRule(Schedule schedule);
        void CopySchedulingScriptToDefault(int customScriptId, DateTime clientTime);
        void DeleteSchedulingScripts(IEnumerable<int> scriptIdList);
    }
}