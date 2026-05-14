using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Resources;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.PersonImport
{
    public class PersonImportService : IPersonImportService
    {
        private readonly IPersonService _personService;
        private readonly IPersonRepository _personRepository;
        private readonly IInvalidSymbolsRepairer _invalidSymbolsRepairer;
        private readonly IInterviewerPasswordSettingsGroup _passwordSettingsGroup;

        public PersonImportService(
            IPersonService personService,
            IPersonRepository personRepository,
            IInvalidSymbolsRepairer invalidSymbolsRepairer,
            IInterviewerPasswordSettingsGroup passwordSettingsGroup)
        {
            _personService = personService;
            _personRepository = personRepository;
            _invalidSymbolsRepairer = invalidSymbolsRepairer;
            _passwordSettingsGroup = passwordSettingsGroup;
        }

        public ImportResult ImportPersons(int callCenterId, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions)
        {
            if (!columnRoleMap.ContainsValue(ColumnRole.Group) &&
                !columnRoleMap.ContainsValue(ColumnRole.Login))
            {
                throw new RolesRequiredException();
            }

            var totalResult = new ImportResult();

            foreach (DataRow dr in dataTable.Rows)
            {
                if (!importOptions.ImportFirstRow && dr == dataTable.Rows[0])
                {
                    continue;
                }

                AssignmentData rowData = GetRowData(dr, columnRoleMap);

                var singleRowResult = new ImportResult();

                try
                {
                    using (var transaction = new DatabaseTransactionScope("ImportPersons", DeadlockPriority.Supervisor))
                    {
                        singleRowResult = ProcessRow(callCenterId, rowData, importOptions);

                        transaction.Commit();
                    }
                    totalResult.GroupsCreated += singleRowResult.GroupsCreated;
                    totalResult.PersonsCreated += singleRowResult.PersonsCreated;
                    totalResult.AutomaticSurveySet += singleRowResult.AutomaticSurveySet;
                    totalResult.AutomaticSurveyReset += singleRowResult.AutomaticSurveyReset;
                }
                catch (Exception ex)
                {
                    singleRowResult.Log +=
                        String.Format("<span style=\"color:red;\">An error has occurred while processing row.<br/>Source: '{0}'<br/>Error message: '{1}'<br/></span>", ex.Source, ex.Message);
                }

                totalResult.RowsProcessed += singleRowResult.RowsProcessed;

                totalResult.Interviewers.AddRange(singleRowResult.Interviewers);

                totalResult.Log += singleRowResult.Log + "<hr>";
            }

            PersonRepository.RefreshCache();
            
            return totalResult;
        }

        /// <summary>
        /// Process person/group data from data row.
        /// </summary>
        /// <param name="callCenterId"></param>
        /// <param name="rowData">Single row data.</param>
        /// <param name="importOptions">Import options.</param>        
        private ImportResult ProcessRow(int callCenterId, AssignmentData rowData, ImportOptions importOptions)
        {
            var importResult = new ImportResult();
            importResult.RowsProcessed++;

            rowData = _invalidSymbolsRepairer.Repair(rowData, importResult);

            var groupSids = new List<int>();

            if (string.IsNullOrEmpty(rowData.GroupName) == false)
            {
                var groupNames = rowData.GroupName.Split(',').Select(x => x.Trim()).Where(x => !String.IsNullOrEmpty(x)).Distinct();
                groupSids = ProcessGroups(groupNames, importResult);
            }

            if (string.IsNullOrEmpty(rowData.PersonName) == false)
            {
                int? personSid = ProcessPerson(callCenterId, rowData, groupSids, importOptions, importResult);

                if (personSid.HasValue)
                {
                    var taskChoice = (AgentTaskChoiceMode)_personRepository.GetById(personSid.Value).ManualSelection;

                    importResult.Interviewers.Add(
                        new InterviewerImportDetails
                        {
                            Id = personSid.Value,
                            GroupIds = groupSids.ToArray(),
                            Name = rowData.PersonName,
                            TaskChoice = taskChoice
                        });

                    // ADD PERSON TO GROUP
                    if (groupSids.Any() && importOptions.OverwriteExistentRelations)
                    {
                        var currentParentGroups = PersonService.GetParentGroups(personSid.Value);
                        var totalParentGroups = groupSids.Union(currentParentGroups);
                        var groupsToAdd = groupSids.Except(currentParentGroups);
                        var groupsAlreadyIn = groupSids.Intersect(currentParentGroups);
                        if (groupsToAdd.Any())
                        {
                            _personService.SetParentGroups(personSid.Value, totalParentGroups.ToArray());
                            importResult.Log += String.Format(
                                "Interviewer '{0}' was added to groups: {1}.<br/>",
                                rowData.PersonName,
                                String.Join(
                                    ", ",
                                    groupsToAdd.Select(x => "'" + PersonGroupRepository.GetById(x).Name + "'").ToArray()));
                        }

                        if (groupsAlreadyIn.Any())
                        {
                            importResult.Log += String.Format(
                                "Interviewer '{0}' is already in groups: {1}.<br/>",
                                rowData.PersonName,
                                String.Join(
                                    ", ",
                                    groupsAlreadyIn.Select(x => "'" + PersonGroupRepository.GetById(x).Name + "'").
                                        ToArray()));
                        }
                    }
                }
            }

            importResult.Log += String.Join("<br/>", importResult.Warnings);

            return importResult;
        }

        /// <summary>
        /// Process person from data row.
        /// </summary>
        /// <param name="singleRowResult">Import result.</param>
        /// <param name="callCenterId"></param>
        /// <param name="ad">Assignment data</param>
        /// <param name="definedParentGroups">The list of parent group sids defined in the importing Excel file.</param>
        /// <param name="importOptions">Import options.</param>
        /// <returns>Person identifier</returns>
        private int? ProcessPerson(int callCenterId, AssignmentData ad, IEnumerable<int> definedParentGroups, ImportOptions importOptions, ImportResult singleRowResult)
        {
            int? personSid = null;
            int foundSid;
            if (PersonService.IsNameUsed(ad.PersonName, out foundSid))
            {
                // If person exists just update its properties
                singleRowResult.Log += String.Format("Interviewer '{0}' already exists.<br/>", ad.PersonName);
                personSid = foundSid;

                bool hasParentGroups = PersonService.GetParentGroups(personSid.Value).Any();

                // If EXISTENT user has no group, add him to CATI Interviewers group
                if (!definedParentGroups.Any() && !hasParentGroups)
                {
                    _personService.SetParentGroups(personSid.Value, new[] { PersonGroupService.RootGroupId });
                }

                if (importOptions.OverwriteExistentData)
                {
                    BvPersonEntity person = PersonRepository.GetById(personSid.Value);

                    var oldAutomaticSurveyId = person.AutomaticSurveyID;

                    if (String.IsNullOrEmpty(ad.PersonPassword))
                    {
                        singleRowResult.Log += String.Format(Strings.PersonImport_UpdatePersonWithEmptyPassword, ad.PersonName);
                    }
                    else
                    {
                        SetPersonPassword(person, ad.PersonPassword);
                    }

                    person.Description = ad.PersonDescription ?? String.Empty;
                    person.Location = ad.PersonLocation ?? String.Empty;

                    _personRepository.Update(person, false);

                    UpdatePersonTaskChoiceMode(person, ad, ref singleRowResult);
                    UpdatePersonAutomaticSurvey(callCenterId, person, ad, ref singleRowResult, oldAutomaticSurveyId);

                    LogUpdatePersonProcess(ad, singleRowResult);
                }
            }
            else
            {
                if (String.IsNullOrEmpty(ad.PersonPassword))
                {
                    singleRowResult.Log += String.Format(Strings.PersonImport_CreatePersonWithEmptyPassword, ad.PersonName);
                }
                else
                {
                    // If Person doesn't exist, create one with specified properties
                    var isPasswordNeedsChangeEnabled = _passwordSettingsGroup.IsChangeAfterFirstLoginRequired;
                    BvPersonEntity person = PersonService.CreateCatiPerson(
                        callCenterId, ad.PersonName, ad.PersonDescription ?? String.Empty, "", ad.PersonLocation, AgentTaskChoiceMode.Automatic, PersonAssignmentListMode.AssignedCallsOnly, 0, DialType.Landline, AgentType.LiveAgent, true, isPasswordNeedsChangeEnabled);

                    personSid = person.SID;

                    SetPersonPassword(person, ad.PersonPassword);

                    var parentGroups = definedParentGroups.Any()
                                           ? definedParentGroups
                                           : new[] { PersonGroupService.RootGroupId };

                    _personService.SetParentGroups(personSid.Value, parentGroups.ToArray());

                    UpdatePersonTaskChoiceMode(person, ad, ref singleRowResult);
                    UpdatePersonAutomaticSurvey(callCenterId, person, ad, ref singleRowResult);

                    LogCreatePersonProcess(ad, singleRowResult);

                    singleRowResult.PersonsCreated++;

                }
            }

            return personSid;
        }

        private void LogUpdatePersonProcess(AssignmentData ad, ImportResult result)
        {
            if (String.IsNullOrEmpty(ad.PersonPassword))
            {
                result.Log += String.Format(Strings.PersonImport_PersonPropertiesUpdated, ad.PersonName);
            }
            else
            {
                result.Log += String.Format(Strings.PersonImport_PersonPropertiesAndPasswordUpdated, ad.PersonName);
            }

            if (result.AutomaticSurveySet > 0)
            {
                result.Log += String.Format(Strings.PersonImport_AutomaticSurveyWasSet, ad.AutomaticSurvey);
            }
            else if (result.AutomaticSurveyReset > 0)
            {
                result.Log += Strings.PersonImport_AutomaticSurveyWasReset;
            }
        }

        private void LogCreatePersonProcess(AssignmentData ad, ImportResult result)
        {
            result.Log += String.Format("New interviewer '{0}' was created.<br/>", ad.PersonName);

            if (result.AutomaticSurveySet > 0)
            {
                result.Log += String.Format(Strings.PersonImport_AutomaticSurveyWasSet, ad.AutomaticSurvey);
            }
        }

        private void SetPersonPassword(BvPersonEntity person, string password)
        {
            var passwordSaver = ServiceLocator.Resolve<IPasswordSaver>();
            passwordSaver.Save(person, password);
        }

        /// <summary>
        /// Process groups from data row.
        /// </summary>
        /// <param name="singleRowResult">Import results</param>
        /// <param name="groups">Groups to process</param>
        /// <returns>Group IDs</returns>
        private List<int> ProcessGroups(IEnumerable<string> groups, ImportResult singleRowResult)
        {
            var result = new List<int>();

            foreach (string groupName in groups)
            {
                int groupSid;
                if (PersonGroupService.IsNameUsed(groupName, out groupSid))
                {
                    // If group is already exists, just remember its SID
                    singleRowResult.Log += String.Format("Group '{0}' already exists.<br/>", groupName);
                }
                else
                {
                    // If group doesn't exist, create one and remember its SID
                    groupSid = PersonGroupService.CreatePersonGroup(groupName, string.Empty, new[] { PersonGroupService.RootGroupId });
                    singleRowResult.GroupsCreated++;
                    singleRowResult.Log += String.Format("New group '{0}' was created.<br/>", groupName);
                }

                result.Add(groupSid);
            }

            return result;
        }

        /// <summary>
        /// Returns AssignmentData for single data row taking into account specified columns roles.
        /// </summary>
        /// <param name="dr">DataTable row.</param>
        /// <param name="columnRoleMap">Column to role map.</param>
        /// <returns>AssignmentData</returns>        
        private AssignmentData GetRowData(DataRow dr, Dictionary<string, ColumnRole> columnRoleMap)
        {
            var ad = new AssignmentData();

            foreach (string columnName in columnRoleMap.Keys)
            {
                var value = dr[columnName].ToString();

                switch (columnRoleMap[columnName])
                {
                    case ColumnRole.Group:
                        ad.GroupName = value;
                        break;
                    case ColumnRole.Login:
                        ad.PersonName = value;
                        break;
                    case ColumnRole.Password:
                        ad.PersonPassword = value;
                        break;
                    case ColumnRole.PersonDescription:
                        ad.PersonDescription = value;
                        break;
                    case ColumnRole.TaskChoice:
                        ad.TaskChoice = value;
                        break;
                    case ColumnRole.PersonLocation:
                        ad.PersonLocation = value;
                        break;
                    case ColumnRole.AutomaticSurvey:
                        ad.AutomaticSurvey = value;
                        break;
                }
            }

            return ad;
        }

        private void UpdatePersonAutomaticSurvey(int callCenterId, BvPersonEntity person, AssignmentData ad, ref ImportResult result, int? oldAutomaticSurveyId = null)
        {
            try
            {
                if (person.ManualSelection == (int)AgentTaskChoiceMode.CampaignAssignment)
                {
                    if (string.IsNullOrEmpty(ad.AutomaticSurvey))
                    {
                        PersonService.ClearAutomaticSurvey(person.SID, false);
                        if (oldAutomaticSurveyId != null)
                            result.AutomaticSurveyReset++;
                    }
                    else
                    {
                        var survey = SurveyRepository.GetByName(ad.AutomaticSurvey);
                        if (survey == null)
                        {
                            result.Log += String.Format(Strings.ErrorWhilePersonAutomaticSurveyUpdateSurveyNotFound);
                            result.Log += "<br/>";
                            return;
                        }

                        AssignmentService.AssignResourcesToSurvey(survey.SID, new List<int> { person.SID }, callCenterId);
                        PersonService.SetAutomaticSurvey(person.SID, survey.SID, false);
                        result.AutomaticSurveySet++;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(ad.AutomaticSurvey)) return;

                    result.Log += String.Format(Strings.ErrorWhilePersonAutomaticSurveyUpdateWrongTaskChoice);
                    result.Log += "<br/>";
                }
            }
            catch
            {
                result.Log += String.Format(Strings.ErrorWhilePersonAutomaticSurveyUpdate);
                result.Log += "<br/>";
            }
        }

        /// <summary>
        /// Sets task choice for given person
        /// If task choice is "Choice" all modes are allowed
        /// </summary>
        private void UpdatePersonTaskChoiceMode(BvPersonEntity person, AssignmentData ad, ref ImportResult result)
        {
            try
            {
                AgentTaskChoiceMode mode = AgentTaskChoiceMode.Automatic;

                TaskChoicePermissions? permission = null;

                if (String.IsNullOrEmpty(ad.TaskChoice))
                {
                    result.Log += String.Format(Strings.TaskChoiceIsNotSpecifiedAutomaticModeWillBeUsed, ad.TaskChoice);
                    result.Log += "<br/>";
                }
                else if (ParsePersonMode(ad.TaskChoice, out mode) == false)
                {
                    result.Log += String.Format(Strings.SpecifiedTaskChoiceIsIncorrectAutomaticModeWillBeUsed, ad.TaskChoice);
                    result.Log += "<br/>";
                }

                if (mode == AgentTaskChoiceMode.Choice)
                {
                    permission = TaskChoicePermissions.Automatic | TaskChoicePermissions.Manual | TaskChoicePermissions.SurveyAssignment;
                }

                PersonService.UpdatePersonMode(person, mode, permission, false);

            }
            catch
            {
                result.Log += String.Format(Strings.ErrorWhilePersonTaskChoiceModeUpdate, ad.TaskChoice);
                result.Log += "<br/>";
            }
        }

        /// <summary>
        /// Parse person mode from string representation
        /// </summary>
        /// <remarks>
        /// 1 = Automatic
        /// 2 = Manual
        /// 3 = Survey
        /// 4 = Choice
        /// </remarks>
        /// <param name="value">User-provided string-representation of person mode</param>
        /// <param name="mode">Person mode as out parameter</param>
        /// <returns>True if parse has been successfully finished otherwise false</returns>
        public bool ParsePersonMode(string value, out AgentTaskChoiceMode mode)
        {
            value = value.Trim().ToLower();

            switch (value)
            {
                case "1":
                case "automatic":
                    mode = AgentTaskChoiceMode.Automatic;
                    return true;
                case "2":
                case "manual":
                    mode = AgentTaskChoiceMode.Manual;
                    return true;
                case "3":
                case "survey":
                case "surveyassignment":
                case "survey_assignment":
                    mode = AgentTaskChoiceMode.CampaignAssignment;
                    return true;
                case "4":
                case "choice":
                    mode = AgentTaskChoiceMode.Choice;
                    return true;
                default:
                    mode = AgentTaskChoiceMode.Automatic;
                    return false;
            }
        }
    }
}