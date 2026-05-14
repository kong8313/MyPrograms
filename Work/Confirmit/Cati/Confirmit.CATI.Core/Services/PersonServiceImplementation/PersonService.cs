using System;
using System.Data;
using System.Globalization;
using System.Linq;
using BvCallHandlerLibrary;
using System.Collections.Generic;
using Confirmit.CATI.Common.Encryption;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Security;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging;
using System.Data.SqlClient;
using System.Diagnostics;
using BvCallHandlerLibrary.Tools;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.PersonLogin;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Telephony;

namespace Confirmit.CATI.Core.Services
{
    public class PersonService : IPasswordSaver, IPersonPwdSetDateSetter, IPersonMessageService, IPersonService
    {
        private readonly IPasswordHash _passwordHash;
        private readonly ITaskRepository _taskRepository;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IDialerCollection _dialerCollection;
        private readonly ITelephony _telephony;

        public PersonService(
            IPasswordHash passwordHash,
            ITaskRepository taskRepository,
            ISurveyRepository surveyRepository,
            IDialerCollection dialerCollection,
            ITelephony telephony)
        {
            if (passwordHash == null)
            {
                throw new ArgumentNullException("passwordHash");
            }

            _passwordHash = passwordHash;
            _taskRepository = taskRepository;
            _surveyRepository = surveyRepository;
            _dialerCollection = dialerCollection;
            _telephony = telephony;
        }

        private enum GroupAction
        {
            Delete = 0,
            Skip = 1,
            Insert = 2
        }

        public static int[] GetParentGroups(int sid)
        {
            var list = BvMembershipAdapter.GetByCondition("ObjectSID = @ObjectSID", new SqlParameter("@ObjectSID", sid));
            return list.Select(x => x.ContainerSID).ToArray();
        }

        public static void SetParentGroups(int sid, int[] parentGroupsSid)
        {
            // TODO: Temporary just to avoid too many changes in tests in one hop, remove later and use interface instead of static method
            var me = ServiceLocator.Resolve<IPersonService>();
            me.SetParentGroups(sid, parentGroupsSid);
        }

        /// <summary>
        /// Sets parent groups for the PERSON.
        /// Not for the GROUP.
        /// To set parent groups for the group see PersonGroupService.SetParentGroups
        /// </summary>
        /// <param name="sid">Person SID</param>
        /// <param name="parentGroupsSid">Person parent groups</param>
        void IPersonService.SetParentGroups(int sid, int[] parentGroupsSid)
        {
            var groupsToAssign = new Dictionary<int, GroupAction>();
            var newGroups = new List<int>(parentGroupsSid);

            foreach (int oldGroupId in GetParentGroups(sid))
            {
                groupsToAssign[oldGroupId] = newGroups.Contains(oldGroupId) ? GroupAction.Skip : GroupAction.Delete;
            }

            //
            // new groups that do not exist in old groups list should be inserted
            foreach (int newGroupId in newGroups)
            {
                if (!groupsToAssign.ContainsKey(newGroupId))
                {
                    groupsToAssign[newGroupId] = GroupAction.Insert;
                }
            }

            bool isAssignChanges = false;

            //
            // process groups
            foreach (int groupId in groupsToAssign.Keys)
            {
                GroupAction action = groupsToAssign[groupId];

                if (action == GroupAction.Insert)
                {
                    BvSpMembership_InsertAdapter.ExecuteNonQuery(
                        groupId,
                        sid);

                    isAssignChanges = true;
                }
                else if (action == GroupAction.Delete)
                {
                    BvSpMembership_DeleteAdapter.ExecuteNonQuery(
                        groupId,
                        sid);

                    isAssignChanges = true;
                }
            }

            if (isAssignChanges) // means that list of groups has been changed
            {
                BvSpPerson_SpinUpAdapter.ExecuteNonQuery(sid);

                var autoSurvey = GetPersonAutomaticSurvey(sid);
                if (autoSurvey != null)
                {
                    var callCenterId = GetPersonCallCenterId(sid);
                    AssignmentService.ClearResourceAutoSurvey(autoSurvey.SID, sid, callCenterId, true);
                }

                OnPersonMemberShipUpdate(sid);
            }
        }

        /// <summary>
        /// Gets the opened surveys available for current interviewer.
        /// </summary>
        public static List<BvSurveyEntity> GetOpenedSurveysForInterviewer(int sid)
        {
            int[] surveySids = BvSpGetOpenedSurveysAdapter.ExecuteEntityList(sid)
                .Select(x => x.SID.GetValueOrDefault()).ToArray();

            return BvSurveyAdapter.GetAll().Where(x => surveySids.Contains(x.SID)).ToList();
        }


        public static bool IsNameUsed(
            string name,
            out int personId)
        {
            personId = 0;

            var person = PersonRepository.TryGetByName(name);

            if (person == null)
                return false;

            personId = person.SID;

            return true;
        }

        /// <summary>
        /// Returns all messages for interviewer
        /// </summary>        
        public static List<BvSpGetMessagesEntity> GetMessages(int sid)
        {
            var messages = BvSpGetMessagesAdapter.ExecuteEntityList(sid);
            PersonRepository.RefreshCache();
 
            return messages;
        }


        /// <summary>
        /// Returns true when there is at least one message
        /// Otherwise return false
        /// </summary>        
        /// <param name="sid">Person sid</param>
        public static bool CheckNewMessages(int sid)
        {
            return BvSpPersonCheckForNewMessageAdapter.ExecuteEntity(sid).HasNewMessage.GetValueOrDefault(false);
        }


        /// <summary>
        /// Deletes messages older expirationTime
        /// </summary>
        /// <param name="expirationTime">expiration time in days</param>
        public void CleanMessages(TimeSpan expirationTime)
        {
            BvSpCleanMessagesAdapter.ExecuteNonQuery((int)expirationTime.TotalDays);
        }

        /// <summary>
        /// Login person into CallsCache
        /// </summary>
        /// <remarks>
        /// Insert record into bvLogin for specified person
        /// Login person into CallsCache
        /// Does rescheduling.
        /// </remarks>
        public static BvTasksEntity LoginPerson(int personId, StationInfo station)
        {
            if (personId == 0)
            {
                throw ExceptionManager.NewArgumentException("personId");
            }

            var person = PersonRepository.GetById(personId);

            EventDetailsScope.Current.AddTiming("PersonRepository.GetById");

            var personMode = (AgentTaskChoiceMode)person.ManualSelection;
            var currentUtcDate = ServiceLocator.Resolve<ITimeService>().GetUtcNow();

            var task = new BvTasksEntity
            {
                PersonSID = personId,
                SurveySID = 0,
                DiallingMode = (int)DialingMode.Manual,
                CallOutcome = (int)CallOutcome.NotDefined,
                StatusLogout = (int)LoginState.LOGGED_IN,
                LastKeepAliveTime = currentUtcDate,
                TimeStateChanged = currentUtcDate,
                StationId = station.StationId,
                AuthenticationKey = Guid.NewGuid(),
                StartSessionTime = currentUtcDate,
                StartTime = currentUtcDate,
                DialerId = station.DialerId,
                StationExtensionNumber = station.ExtensionNumber,
                IsDialerAgentLocal = station.IsLocal,
                CallCenterID = person.CallCenterID,
                DialTypeId = person.DialTypeId
            };

            using (var encryptor = ServiceLocator.Resolve<ICatiSymmetricEncryptor>())
            {
                task.EncryptionKey = encryptor.Key;
                task.EncryptionIV = encryptor.IV;
            }

            EventDetailsScope.Current.AddTiming("new SymmetricEncryptorFactory().CreateEncryptor()");

            ServiceLocator.Resolve<ITaskRepository>().Merge(task);

            EventDetailsScope.Current.AddTiming("LoginPerson:ITaskRepository.Merge");

            if (personMode != AgentTaskChoiceMode.CampaignAssignment &&
                person.AllowedChoices.HasValue == false)
            {
                FillLoginGroupsAndAsyncReschedule(personId);

                EventDetailsScope.Current.AddTiming("LoginPerson:FillLoginGroupsAndAsyncReschedule");
            }

            return task;
        }

        public static void LoginPersonOnSurveyForSurveySelectionMode(int personId, int surveyId)
        {
            var taskReposioty = ServiceLocator.Resolve<ITaskRepository>();
            var task = taskReposioty.GetByPerson(personId);
            task.SurveySID = surveyId;
            task.SelectedSurveyId = surveyId;
            taskReposioty.Update(task);

            FillLoginGroupsAndAsyncReschedule(personId);
        }

        public static void FillLoginGroupsAndAsyncReschedule(
            int personId)
        {
            BvSpLogin_SpinUpAdapter.ExecuteNonQuery(personId);
        }

        public static bool IsLoginNeeded(
            int sid,
            int surveySid)
        {
            var task = ServiceLocator.Resolve<ITaskRepository>().GetByPerson(sid);

            if (task == null)
            {
                return true;
            }

            return (task.SurveySID != surveySid);
        }

        /// <summary>
        /// Returns automatic survey for given user. If user doesn't have such survey,
        /// returns null.
        /// </summary>
        /// <param name="personId">Person identifier.</param>
        /// <returns>BvSurveyEntity or null.</returns>
        public static BvSurveyEntity GetPersonAutomaticSurvey(int personId)
        {
            return GetPersonAutomaticSurvey(PersonRepository.GetById(personId));
        }

        /// <summary>
        /// Returns automatic survey for given user. If user doesn't have such survey,
        /// returns null.
        /// </summary>
        /// <param name="person">Person.</param>
        /// <returns>BvSurveyEntity or null.</returns>
        public static BvSurveyEntity GetPersonAutomaticSurvey(BvPersonEntity person)
        {
            if (person == null)
            {
                throw new ArgumentNullException("person");
            }

            if (!person.AutomaticSurveyID.HasValue)
                return null;

            BvSurveyEntity survey = SurveyRepository.GetById(person.AutomaticSurveyID.Value);

            return survey.State != (int)SurveyState.SoftDeleted
                ? survey
                : null;
        }
        
        public static void ChangeTaskChoice(IEnumerable<int> personSids, AgentTaskChoiceMode taskChoice, TaskChoicePermissions? permissions, bool updateCache)
        {
            try
            {
                foreach (int sid in personSids)
                {
                    BvPersonEntity person = PersonRepository.GetById(sid);

                    var evt = new ChangeInterviewerTaskChoiceEvent(person.SID, person.Name, taskChoice, permissions);

                    UpdatePersonMode(person, taskChoice, permissions, false);

                    evt.Finish();
                }
            }
            finally
            {
                if (updateCache)
                {
                    PersonRepository.RefreshCache();
                }
            }
        }

        /// <summary>
        /// Updates person mode
        /// </summary>
        /// <param name="person">person represented by BvPersonEntity</param>
        /// <param name="mode">Person task choice mode</param>
        /// <param name="taskChoicePermissions">Task choice permissions for "Choice" mode</param>
        /// <param name="updateCache">Update cache for persons or not</param>
        public static void UpdatePersonMode(BvPersonEntity person, AgentTaskChoiceMode mode, TaskChoicePermissions? taskChoicePermissions, bool updateCache)
        {
            if (mode != AgentTaskChoiceMode.Choice)
            {
                person.AllowedChoices = null;
                person.ManualSelection = (int)mode;
            }
            else
            {
                if (taskChoicePermissions != null)
                {
                    person.AllowedChoices = (int?)taskChoicePermissions;
                }
                else
                {
                    throw new UserMessageException("You should specify at least one task choice.");
                }
            }

            if ((mode == AgentTaskChoiceMode.CampaignAssignment ||
                (mode == AgentTaskChoiceMode.Choice &&
                (taskChoicePermissions.Value & TaskChoicePermissions.SurveyAssignment) == TaskChoicePermissions.SurveyAssignment)) == false)
            {
                person.AutomaticSurveyID = null;
            }

            PersonRepository.Update(person, updateCache);
        }

        public static void ChangeEnableSoftphoneIntegration(IEnumerable<int> personSIDs, bool enableSoftphoneIntegration)
        {
            try
            {
                foreach (var sid in personSIDs)
                {
                    var person = PersonRepository.GetById(sid);

                    var evt = new ChangeInterviewerSoftphoneIntegrationEvent(person.SID, person.Name);

                    person.EnableSoftphoneIntegration = enableSoftphoneIntegration;
                    PersonRepository.Update(person, false);

                    evt.Finish();
                }
            }
            finally
            {
                PersonRepository.RefreshCache();
            }
        }

        public static void ChangeLocation(IEnumerable<int> personSIDs, string location)
        {
            try
            {
                // TODO: Maybe we could create a stored procedure to update all persons at one call to DB.
                var valueToSet = string.IsNullOrWhiteSpace(location) ? null : location;
                foreach (var sid in personSIDs)
                {
                    var person = PersonRepository.GetById(sid);

                    var evt = new ChangeInterviewerLocationEvent(person.SID, person.Name, location);

                    person.Location = valueToSet;
                    PersonRepository.Update(person, false);

                    evt.Finish();
                }
            }
            finally
            {
                PersonRepository.RefreshCache();
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> GetPersonDialerAttributes(BvPersonEntity person)
        {
            if (person == null)
            {
                throw new ArgumentNullException("person");
            }

            var location = person.Location ?? "";

            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Location", location),
                new KeyValuePair<string, string>("CallCenterId", person.CallCenterID.ToString(CultureInfo.InvariantCulture))
            };
        }

        /// <summary>
        /// Indicates that given person is logged in Cati Console.
        /// </summary>
        /// <param name="personId">Person identifier.</param>
        /// <returns>true, if person is logged in; otherwise false.</returns>
        public static bool IsPersonLoggedIn(int personId)
        {
            return (ServiceLocator.Resolve<ITaskRepository>().GetByPerson(personId) != null);
        }

        public static BvPersonEntity CreateCatiPerson(
            int callCenterId,
            string name,
            string description,
            string fullName,
            string location,
            AgentTaskChoiceMode mode,
            PersonAssignmentListMode assignmentListMode = PersonAssignmentListMode.AssignedCallsOnly,
            int callGroupId = 0,
            DialType dialType = DialType.Landline,
            AgentType agentType = AgentType.LiveAgent,
            bool enableSoftphoneIntegration = true,
            bool passwordNeedsChange = false,
            string[] attributes = null)
        {
            if (attributes == null)
            {
                attributes = new string[5];
            }

            var person = new BvPersonEntity
            {
                Name = name,
                Description = description,
                FullName = fullName ?? "",
                ManualSelection = (int)mode,
                AssignmentsListMode = (int)assignmentListMode,
                CallGroupID = callGroupId == 0 ? null : (int?)callGroupId,
                CallCenterID = callCenterId,
                Location = location,
                PwdSetDate = DateTime.UtcNow,
                DialTypeId = (byte)dialType,
                Type = (byte)agentType,
                EnableSoftphoneIntegration = enableSoftphoneIntegration,
                PasswordNeedsChange = passwordNeedsChange,
                Attribute1 = attributes[0],
                Attribute2 = attributes[1],
                Attribute3 = attributes[2],
                Attribute4 = attributes[3],
                Attribute5 = attributes[4]
            };

            var personRepository = ServiceLocator.Resolve<IPersonRepository>();
            int id = personRepository.Insert(person);

            return PersonRepository.GetById(id);
        }

        /// <summary>
        /// Creates or updates person.
        /// Sets person properties.
        /// </summary>
        /// <param name="callCenterId">Call center id</param>
        /// <param name="personSid">Person SID</param>
        /// <param name="name">Person name</param>
        /// <param name="description">Person description</param>
        /// <param name="fullName">Person full name</param>
        /// <param name="password">Person password</param>
        /// <param name="mode">Person task choice mode</param>
        /// <param name="assignmentListMode"> </param>
        /// <param name="permissions">Task choice permissions</param>
        /// <param name="parentGroups">Person parent groups' ids</param>
        /// <param name="autoSurveyId">Automatic survey id</param>
        /// <param name="callGroupId">Call group ID</param>
        /// <param name="location">Person location</param>
        /// <param name="dialType"></param>
        /// <param name="agentType"></param>
        /// <param name="enableSoftphoneIntegration">Use default dialer setting or no SSO</param>
        /// <param name="passwordNeedsChange">If true interviewer will be asked to change his password on first login</param>
        /// <param name="attributes">Person attributes</param>
        public int CreateOrUpdatePerson(
            int callCenterId,
            int personSid,
            string name,
            string description,
            string fullName,
            string password,
            AgentTaskChoiceMode mode,
            PersonAssignmentListMode assignmentListMode,
            TaskChoicePermissions? permissions,
            List<int> parentGroups,
            int? autoSurveyId,
            int callGroupId,
            string location,
            DialType dialType,
            AgentType agentType,
            bool enableSoftphoneIntegration,
            bool passwordNeedsChange,
            string[] attributes = null)
        {
            if (attributes == null)
            {
                attributes = new string[5];
            }
            
            BvPersonEntity person;
            try
            {
                var evt = personSid == 0
                    ? (IManagementActivityEvent)
                    new CreateInterviewerEvent(personSid, name, parentGroups, mode, permissions, location, dialType,
                        agentType)
                    : new UpdateInterviewerEvent(personSid, name, parentGroups, mode, permissions, location, dialType,
                        agentType);

                if (personSid == 0)
                {
                    person = CreateCatiPerson(callCenterId, name, description, fullName,
                        string.IsNullOrWhiteSpace(location) ? null : location, mode, assignmentListMode, callGroupId,
                        dialType, agentType, enableSoftphoneIntegration, passwordNeedsChange, attributes);
                    evt.ObjectId = person.SID;
                }
                else
                {
                    person = PersonRepository.GetById(personSid);
                    person.Name = name;
                    person.Description = description;
                    person.AssignmentsListMode = (int)assignmentListMode;
                    person.CallGroupID = callGroupId == 0 ? null : (int?)callGroupId;
                    person.Location = string.IsNullOrWhiteSpace(location) ? null : location;
                    person.DialTypeId = (byte)dialType;
                    person.EnableSoftphoneIntegration = enableSoftphoneIntegration;
                    person.FullName = fullName ?? "";
                    person.Attribute1 = attributes[0];
                    person.Attribute2 = attributes[1];
                    person.Attribute3 = attributes[2];
                    person.Attribute4 = attributes[3];
                    person.Attribute5 = attributes[4];
                    //person.Type = (byte) personType;//we should not allow to change person type
                    PersonRepository.Update(person, false);
                }

                UpdatePersonMode(person, mode, permissions, false);

                if (mode == AgentTaskChoiceMode.CampaignAssignment ||
                    (mode == AgentTaskChoiceMode.Choice && (permissions & TaskChoicePermissions.SurveyAssignment) ==
                        TaskChoicePermissions.SurveyAssignment))
                {
                    if (autoSurveyId.HasValue && autoSurveyId != 0)
                    {
                        SetAutomaticSurvey(person.SID, autoSurveyId.Value, false);
                    }
                    else
                    {
                        ClearAutomaticSurvey(person.SID, false);
                    }
                }

                SetParentGroups(person.SID, parentGroups.ToArray());

                if (personSid == 0)
                {
                    var saver = ServiceLocator.Resolve<IPasswordSaver>();
                    saver.Save(person.SID, password);
                }

                evt.Finish();
            }
            finally
            {
                PersonRepository.RefreshCache();
            }

            return person.SID;
        }

        /// <summary>
        /// Changes automatic survey for given person.
        /// </summary>
        /// <param name="personId">Person Id.</param>
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="updateCache">Update cache for persons or not</param>
        public static void SetAutomaticSurvey(int personId, int surveyId, bool updateCache)
        {
            var person = PersonRepository.GetById(personId);
            var survey = SurveyRepository.GetById(surveyId);

            person.AutomaticSurveyID = surveyId;

            var evt = new SetInterviewerAutomaticSurveyEvent(personId, person.Name, surveyId, survey.ProjectId);

            PersonRepository.Update(person, updateCache);

            evt.Finish();
        }

        public static BvSpPerson_SetAutomaticSurveyEntity SetAutomaticSurveySeamless(int personId, int surveyId)
        {
            var person = PersonRepository.GetById(personId);
            var survey = SurveyRepository.GetById(surveyId);

            person.AutomaticSurveyID = surveyId;

            var evt = new SetInterviewerAutomaticSurveyEvent(personId, person.Name, surveyId, survey.ProjectId);

            var result = BvSpPerson_SetAutomaticSurveyAdapter.ExecuteEntityList(personId, surveyId).SingleOrDefault();

            evt.Finish();
            PersonRepository.RefreshCache();
            
            return result;
        }

        /// <summary>
        /// Clears automatic survey for given person.
        /// </summary>
        /// <param name="personId">Person Id.</param>
        /// <param name="updateCache">Update cache for persons or not</param>
        public static void ClearAutomaticSurvey(int personId, bool updateCache)
        {
            BvPersonEntity person = PersonRepository.GetById(personId);
            person.AutomaticSurveyID = null;

            var evt = new ClearInterviewerAutomaticSurveyEvent(personId, person.Name);

            PersonRepository.Update(person, updateCache);

            evt.Finish();
        }

        /// <summary>
        /// Locks account for given person by supervisor.
        /// </summary>
        /// <param name="personId">Person Id.</param>
        public void LockPersonBySupervisor(int personId)
        {
            LockPerson(personId, true);

            TaskService.TerminateTask(
                personId,
                new DatabaseTransactionOptions("PersonService.LockPerson", DeadlockPriority.Supervisor));
        }

        public void LockPersonsBySupervisor(List<int> personIds)
        {
            try
            {
                foreach (var personId in personIds)
                {
                    LockPerson(personId, false);

                    TaskService.TerminateTask(
                        personId,
                        new DatabaseTransactionOptions("PersonService.LockPerson", DeadlockPriority.Supervisor));
                }
            }
            finally
            {
                PersonRepository.RefreshCache();
            }
        }
        
        /// <summary>
        /// Locks account for given person.
        /// </summary>
        public static void LockPerson(int personId, bool updateCache)
        {
            LockPerson(personId, true, updateCache);
        }
        
        public static void UnlockPerson(int personId, bool updateCache)
        {
            LockPerson(personId, false, updateCache);
        }
        
        public static void UnlockPersons(List<int> personIds)
        {
            try
            {
                foreach (var personId in personIds)
                {
                    UnlockPerson(personId, false);
                }
            }
            finally
            {
                PersonRepository.RefreshCache();
            }
        }

        private static void LockPerson(int personId, bool isLocked, bool updateCache)
        {
            var person = PersonRepository.GetById(personId);

            person.IsLocked = isLocked;

            if (isLocked)
            {
                person.LockedDate = DateTime.UtcNow;
            }
            else
            {
                person.LockedDate = null;
                SetFailedLoginAttempts(personId, 0);
            }

            PersonRepository.Update(person, updateCache);
        }

        public static void SetFailedLoginAttempts(int personId, int count)
        {
            var entity = new BvPersonFailedLoginAttemptsEntity() { PersonId = personId, Count = count };
            BvPersonFailedLoginAttemptsAdapter.Update(entity);
        }

        public static int GetFailedLoginAttempts(int personId)
        {
            return BvPersonFailedLoginAttemptsAdapter.GetByCondition("PersonID = @PersonID", new SqlParameter("@PersonID", personId)).Single().Count;
        }

        /// <summary>
        /// Sets the appropriate PwdSetDate to all persons.
        /// </summary>
        /// <param name="pwdSetDate"></param>
        public void SetPwdSetDateToAllPersons(DateTime pwdSetDate)
        {
            //TODO: add this event if needed - var evt = new ChangePwdSetDateEvent(pwdSetDate);

            var databaseEngine = new DatabaseEngine();
            databaseEngine.ExecuteNonQuery(
                "UPDATE [BvPerson] SET [PwdSetDate]=@PasswordDate",
                CommandType.Text,
                new SqlParameter("@PasswordDate", pwdSetDate));

            PersonRepository.RefreshCache();
            // evt.Finish();
        }

        void IPasswordSaver.Save(int personId, string password)
        {
            var salt = _passwordHash.GenerateSaltValue();
            var encryptedPassword = _passwordHash.ComputeHash(password, salt);

            var databaseEngine = new DatabaseEngine();
            var result = databaseEngine.ExecuteScalar<int>(
                "UPDATE [BvPerson] SET [PwdHashTxt]=@PasswordHash, [PwdSaltTxt]=@Salt WHERE [SID]=@PersonID; SELECT @@ROWCOUNT",
                CommandType.Text,
                new SqlParameter("@PasswordHash", encryptedPassword),
                new SqlParameter("@Salt", salt),
                new SqlParameter("@PersonID", personId)
                );

            if (result == 0)
            {
                throw new InvalidOperationException(string.Format("Person with id {0} is not found", personId));
            }

            PersonRepository.RefreshCache();
        }

        void IPasswordSaver.Save(BvPersonEntity person, string password)
        {
            person.PwdSaltTxt = _passwordHash.GenerateSaltValue();
            person.PwdHashTxt = _passwordHash.ComputeHash(password, person.PwdSaltTxt);
            person.PwdSetDate = DateTime.UtcNow;
        }

        internal static int GetPersonCallCenterId(int personId)
        {
            var person = PersonRepository.GetById(personId);
            return person.CallCenterID;
        }

        /// <summary>
        /// Send to dialer all group sids of person
        /// if Membership is updated
        /// </summary>
        /// <param name="personSid"></param>
        public void OnPersonMemberShipUpdate(int personSid)
        {
            BvTasksEntity task = null;
            int[] sids = null;

            try
            {
                task = _taskRepository.GetByPerson(personSid);

                if ((task == null) || (task.SurveySID == 0))
                {
                    return;
                }

                if (task.IsLoginRCToDialer)
                {
                    // IsLoginRCToDialer is in fact a predictive/non-predictive flag.
                    // (IsLoginRCToDialer == true) means it's a non-predictive mode.
                    return;
                }

                if (!BvCallHandlerRoot.IsLoggedInToDialer(task))
                {
                    return;
                }

                // person logged in a predictive survey
                sids = PersonTools.GetUserGroups(personSid);
                var survey = _surveyRepository.GetById(task.SurveySID);

                if (!_dialerCollection.IsDialerInitialized(task.DialerId))
                {
                    Trace.TraceWarning(
                        "BvCallHandlerRoot.OnPersonMemberShipUpdate: Dialer [{0}] is unavailable, so SetGroups is not called. /// personSID={1}",
                        task.DialerId, personSid);

                    return;
                }

                _telephony.SendSetGroups(task.DialerId, survey.CampaignId, personSid, sids);
            }
            catch (Exception ex)
            {
                var logSidsStr = (sids == null) ?
                    "[null]"
                    : string.Format("[{0}]", string.Join(",", sids));

                var logTaskStr = (task == null) ?
                    "[null]"
                    : string.Format("[DialerId='{0}', SurveySID='{1}', StatusLogout='{2}', LoggedInToDialerState='{3}']",
                        task.SurveySID, task.DialerId, task.StatusLogout, task.LoggedInToDialerState);

                Trace.TraceError(
                    "BvCallHandlerRoot.OnPersonMemberShipUpdate: {0} /// personSID={1}, sids: {2}, task: {3}",
                    ex, personSid, logSidsStr, logTaskStr);
            }
        }
    }
}
