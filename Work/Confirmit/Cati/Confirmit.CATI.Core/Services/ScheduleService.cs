using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

using BvDotNetEngine;

using BvSchScriptGen;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Script;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Resources;

namespace Confirmit.CATI.Core.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IPersonGroupRepository _personGroupRepository;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IStateGroupRepository _stateGroupRepository;
        private readonly IShiftServiceFactory _shiftServiceFactory;

        public ScheduleService(
            IPersonRepository personRepository,
            IPersonGroupRepository personGroupRepository,
            ISurveyRepository surveyRepository,
            IStateRepository stateRepository,
            IStateGroupRepository stateGroupRepository,
            IShiftServiceFactory shiftServiceFactory)
        {
            _personRepository = personRepository;
            _personGroupRepository = personGroupRepository;
            _surveyRepository = surveyRepository;
            _stateRepository = stateRepository;
            _stateGroupRepository = stateGroupRepository;
            _shiftServiceFactory = shiftServiceFactory;
        }

        private Schedule DeserializeScheduleFromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return new Schedule();
            }

            var xmlSerializer = new XmlSerializer(typeof(Schedule));
            var settings = new XmlReaderSettings();

            using (var stringReader = new StringReader(xml))
            using (var xmlReader = XmlReader.Create(stringReader, settings))
            {
                return (Schedule)xmlSerializer.Deserialize(xmlReader);
            }
        }

        public void Save(int scheduleId, string serializedSchedule)
        {
            var evt = new ScriptSaveEvent(scheduleId, string.Empty);

            Check(serializedSchedule);

            BvScheduleEntity schedule = ScheduleRepository.GetById(scheduleId);
            schedule.XmlUnderDev = serializedSchedule;

            evt.ObjectName = schedule.Name;

            ScheduleRepository.Update(schedule);

            evt.Finish();
        }

        public void ReGenerateScript(BvScheduleEntity scheduleEntity)
        {
            string scriptSource = GenerateAndValidateScript(scheduleEntity.XmlInUse);
            scheduleEntity.ScriptSource = scriptSource;
            scheduleEntity.RegenerateIsRequired = false;

            ScheduleRepository.UpdateByCondition(
                   scheduleEntity,
                   "[RegenerateIsRequired] = 1 AND [ScheduleID] = @ScheduleID"); // This update operation implies change of scheduleEntity.ModifyDate,
            // so the scheduling script assembly will be recompiled.             
        }

        public static void Launch(int sid)
        {
            var me = ServiceLocator.Resolve<IScheduleService>();
            me.Launch(sid);
        }

        void IScheduleService.Launch(int sid)
        {
            var evt = new ScriptLaunchEvent(sid, String.Empty);

            // get and deserialize xml to Schedule DOM
            var scheduleEntity = ScheduleRepository.GetById(sid);

            evt.ObjectName = scheduleEntity.Name;

            var scheduleInDev = DeserializeScheduleFromXml(scheduleEntity.XmlUnderDev);
            var scheduleInUse = DeserializeScheduleFromXml(scheduleEntity.XmlInUse);

            string scriptSource = GenerateAndValidateScript(scheduleEntity.XmlUnderDev);

            //
            // apply configuration: save shift types, shifts and exclusions to their DB tables
            var configurationApplier = new ScheduleXmlConfigurationApplier(sid);

            configurationApplier.Apply(
                scheduleInDev,
                scheduleInUse);

            //
            // save data to schedule and script tables
            scheduleEntity.XmlInUse = scheduleEntity.XmlUnderDev;
            scheduleEntity.ScriptSource = scriptSource;
            scheduleEntity.IsSampleUpdateRuleSet = GetSampleUpdateRule(scheduleInDev) != null;
            ScheduleRepository.Update(scheduleEntity);

            _shiftServiceFactory.DropScheduleCache();

            evt.Finish();
        }

        public void Check(string serializedSchedule)
        {
            Check(XmlDeserialize(serializedSchedule));
        }

        public void Check(Schedule scheduleInDev)
        {
            if (scheduleInDev.ShiftTypes.Count == 0)
                throw new UserMessageException(
                    string.Format(
                        "Scheduling script '{0}' should contain at least one shift type",
                        scheduleInDev.Name));

            if (scheduleInDev.Shifts.Count == 0)
                throw new UserMessageException(
                    string.Format(
                        "Scheduling script '{0}' should contain at least one shift",
                        scheduleInDev.Name));

            ValidateThereIsJustOneSampleUpdateRule(scheduleInDev);

            //
            // check that shifts have valid shifttypes
            ShiftService.CheckShiftsHaveValidShiftTypes(
                scheduleInDev.Shifts,
                scheduleInDev.ShiftTypes);

            //
            // check configuration
            var shiftService = ShiftService.Create(scheduleInDev);

            shiftService.CheckConfiguration();

            //
            // serialize Schedule object to XML
            string scriptSourceXml;

            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(Schedule));
                serializer.Serialize(stringWriter, scheduleInDev);
                scriptSourceXml = stringWriter.ToString();
            }
        }

        public static void ValidateThereIsJustOneSampleUpdateRule(Schedule schedule)
        {
            var sampleUpdateRules = schedule.Rules.Where(x => x.SampleUpdate);

            if (sampleUpdateRules.Count() > 1)
            {
                throw new UserMessageException(
                    string.Format(
                        "Scheduling script '{0}' should contain just one rule which has to be executed during sample update",
                        schedule.Name));
            }
        }

        ScheduleDom.Scheduling.Rule IScheduleService.GetSampleUpdateRule(Schedule schedule)
        {
            return GetSampleUpdateRule(schedule);
        }

        public static ScheduleDom.Scheduling.Rule GetSampleUpdateRule(Schedule schedule)
        {
            return schedule.Rules.FirstOrDefault(x => x.SampleUpdate);
        }

        public string GetValidActionsXml()
        {
            return Resource.Actions;
        }

        public IActionCollection GetActions()
        {
            ActionCollection result;

            string actionXmlString = GetValidActionsXml();

            using (TextReader stringReader = new StringReader(actionXmlString))
            {
                var serializer = new XmlSerializer(typeof(ActionCollection));
                result = (ActionCollection)serializer.Deserialize(stringReader);
            }

            return result;
        }

        public List<BvSpShift_ListEntity> GetShiftList(
            int sid,
            int shiftId,
            int timezoneId)
        {
            return BvSpShift_ListAdapter.ExecuteEntityList(
                sid,
                shiftId,
                timezoneId);
        }

        public List<BvSpShiftType_ListEntity> GetShiftTypeList(int sid)
        {
            return BvSpShiftType_ListAdapter.ExecuteEntityList(sid);
        }

        public int DefaultScheduleId
        {
            get
            {
                return new DatabaseEngine().ExecuteScalar<int>(
                    "SELECT MIN( ScheduleID ) FROM BvSchedule", CommandType.Text);
            }
        }

        /// <summary>
        /// Serializes Schedule object into Xml representation.
        /// </summary>
        /// <param name="schedule">Object to serialize.</param>
        /// <returns>Xml string.</returns>
        public string XmlSerialize(Schedule schedule)
        {
            var ser = new XmlSerializer(typeof(Schedule));
            using (var mem = new MemoryStream())
            {
                ser.Serialize(mem, schedule);
                mem.Seek(0, SeekOrigin.Begin);

                using (var r = new StreamReader(mem))
                {
                    return r.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Deserializes Schedule object from Xml string.
        /// </summary>
        /// <param name="serializedSchedule">Xml string.</param>
        /// <returns>Schedule object.</returns>
        public Schedule XmlDeserialize(string serializedSchedule)
        {
            using (TextReader text = new StringReader(serializedSchedule))
            {
                var ser = new XmlSerializer(typeof(Schedule));
                return (Schedule)ser.Deserialize(text);
            }
        }

        public int GetParamValue(int scheduleId, int surveySid, int paramId)
        {
            var param = BvScheduleParamCache.Instance.GetByScheduleIDSurveySIDParamID(scheduleId, surveySid, paramId);

            if (param == null)
                throw new InternalErrorException(
                    String.Format("parameter info for scheduleId = {0}, surveySID = {1}, paramName = {2} not found",
                    scheduleId, surveySid, paramId));

            return param.Value;
        }

        public int GetParamValue(int scheduleId, int surveySid, string name)
        {
            var param = BvScheduleParamCache.Instance.GetByScheduleIDSurveySIDName(scheduleId, surveySid, name);

            if (param == null)
                throw new InternalErrorException(
                    String.Format("parameter info for scheduleId = {0}, surveySID = {1}, paramName = {2} not found",
                    scheduleId, surveySid, name));

            return param.Value;
        }

        public void SetParamValue(int surveySid, int paramId, int value)
        {
            var survey = SurveyRepository.GetById(surveySid);

            string reason;
            if (!CheckParamValue(survey.ScheduleID, surveySid, paramId, value, out reason))
                throw new UserMessageException(String.IsNullOrEmpty(reason) ? "Invalid parameter value" : reason);

            BvSpScheduleParam_SetAdapter.ExecuteNonQuery(surveySid, paramId, value);

            BvScheduleParamCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishScheduleParamsUpdated();
        }

        public bool CheckParamValue(int scheduleId, int surveySid, int paramId, int value, out string reason)
        {
            var paramInfo = BvScheduleParamCache.Instance.GetByScheduleIDSurveySIDParamID(scheduleId, surveySid, paramId);

            if (paramInfo == null)
                throw new InternalErrorException(
                    String.Format("parameter info for scheduleId = {0}, surveySID = {1}, paramID = {2} not found",
                    scheduleId, surveySid, paramId));

            return CheckParamValue(scheduleId, surveySid, (SchedulingParameterType)paramInfo.Type, value, out reason);
        }

        public bool CheckParamValue(int scheduleId, int surveySid, SchedulingParameterType type, int value)
        {
            string reason;
            return CheckParamValue(scheduleId, surveySid, type, value, out reason);
        }

        public bool CheckParamValue(int scheduleId, int surveySid, SchedulingParameterType type, int value, out string reason)
        {
            return CheckParamValue(() => new ShiftServiceFactory().Get(scheduleId), surveySid, type, value, out reason);
        }

        public bool CheckParamValue(Schedule schedule, int surveySid, SchedulingParameterType type, int value)
        {
            string reason;
            return CheckParamValue(schedule, surveySid, type, value, out reason);
        }

        public bool CheckParamValue(Schedule schedule, int surveySid, SchedulingParameterType type, int value, out string reason)
        {
            return CheckParamValue(() => ShiftService.Create(schedule), surveySid, type, value, out reason);
        }

        private bool CheckParamValue(Func<IShiftService> shiftService, int surveySid, SchedulingParameterType type, int value, out string reason)
        {
            var callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();

            reason = null;

            ShiftService.MatchingShift shift;

            switch (type)
            {
                case SchedulingParameterType.Integer:

                    return true;

                case SchedulingParameterType.ExtendedStatus:
                    int stateGroupId = (surveySid != 0)
                        ? _surveyRepository.GetById(surveySid).StateGroupID
                        : _stateGroupRepository.GetDefault().ID;

                    var state = _stateRepository.GetByItsAndStateGroupId(value, stateGroupId);

                    return state != null;

                case SchedulingParameterType.Resource:

                    switch (value)
                    {
                        //[Unchanged]
                        case -1:
                            return true;
                        //[Last Person]
                        case -2:
                            return true;
                        //[Survey Interviewers]
                        case -3:
                            return true;
                        //explicit assign
                        default:
                            if (value <= 0)
                            {
                                reason = "Invalid parameter value.";
                                return false;
                            }

                            if (_personRepository.TryGetById(value) == null)
                                if (_personGroupRepository.TryGetById(value) == null)
                                {
                                    reason = String.Format("Invalid parameter value. There is no person or person group with id = {0}.", value);
                                    return false;
                                }

                            return true;
                    }

                case SchedulingParameterType.Shift:

                    shift = shiftService().GetNextShiftByID(DateTime.UtcNow, callCenterRepository.Default.LocalTimezoneId, value);

                    if (shift != null)
                    {
                        return true;
                    }

                    reason = String.Format("Invalid parameter value. There are no shifts with id = {0}.", value);
                    return false;

                case SchedulingParameterType.ShiftType:
                    shift = shiftService().GetNextShiftOfSpecifiedType(DateTime.UtcNow, callCenterRepository.Default.LocalTimezoneId, value);
                    if (shift != null)
                    {
                        return true;
                    }

                    reason = String.Format("Invalid parameter value. There is no shift type with id = {0} or there are no shifts for this shift type.", value);
                    return false;

                default:
                    reason = "Invalid parameter value. Unknown parameter type.";
                    return false;//Unknown parameter type
            }
        }

        /// <summary>
        /// Returns if schedule has parameters in use.
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        public bool DoesSheduleHaveParametersInUse(int scheduleId)
        {
            return BvScheduleParamAdapter.GetByCondition(
                "[ScheduleID] = @ScheduleID",
                new SqlParameter("@ScheduleID", scheduleId)).Any();

        }

        public IEnumerable<SchedulingParameterType> GetMatchingTypes(SchedulingParameterType actionType)
        {
            switch (actionType)
            {
                case SchedulingParameterType.Integer:
                    foreach (var value in Enum.GetValues(typeof(SchedulingParameterType)))
                        yield return (SchedulingParameterType)value;
                    break;
                default:
                    yield return actionType;
                    break;
            }
        }

        private string GenerateAndValidateScript(string scriptSourceXml)
        {
            var scriptGenerator = new ScriptGenerator();
            string scriptSource = scriptGenerator.GenerateScript(scriptSourceXml);

            var scriptExecutor = new ScheduleScriptExecutor();
            scriptExecutor.Validate(scriptSource);
            return scriptSource;
        }

        public void CopySchedulingScriptToDefault(int customScriptId, DateTime clientTime)
        {
            var defaultScript = ScheduleRepository.GetById(DefaultScheduleId);

            if (customScriptId == defaultScript.ScheduleID)
            {
                throw new UserMessageException(string.Format(Strings.Error_TryToCopyDefaultSchedulingScriptIntoItself, defaultScript.Name));
            }

            var backupDefaultEntity = new BvScheduleEntity
            {
                XmlUnderDev = defaultScript.XmlUnderDev,
                Name = $"{defaultScript.Name} backup ({clientTime.ToString("dd/MMM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)})"
            };

            ScheduleRepository.Insert(backupDefaultEntity);

            var customScheduleScript = ScheduleRepository.GetById(customScriptId);

            var evt = new CopyToDefaultSchedulingScriptEvent(defaultScript.ScheduleID, defaultScript.Name, customScheduleScript.ScheduleID, customScheduleScript.Name);

            defaultScript.XmlUnderDev = customScheduleScript.XmlUnderDev;
            ScheduleRepository.Update(defaultScript);

            evt.Finish();
        }

        public void DeleteSchedulingScripts(IEnumerable<int> scriptIdList)
        {
            using (var transactionScope = new DatabaseTransactionScope("ScriptsList.DeleteScript", DeadlockPriority.Supervisor))
            {
                foreach (var scriptId in scriptIdList)
                {
                    var scriptName = ScheduleRepository.GetById(scriptId).Name;

                    var evt = new ScriptDeleteEvent(scriptId, scriptName);

                    ScheduleRepository.Delete(scriptId);

                    evt.Finish();
                }

                transactionScope.Commit();
            }
        }
    }
}
