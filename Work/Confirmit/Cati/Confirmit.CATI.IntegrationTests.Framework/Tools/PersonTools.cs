using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class PersonTools
    {
        public static int CreatePerson(string name)
        {
            return CreatePerson(name, null, AgentTaskChoiceMode.Manual, null, CallCenterTools.DefaultId);
        }

        public static int CreatePerson(string name, AgentTaskChoiceMode mode = AgentTaskChoiceMode.Manual)
        {
            return CreatePerson(name, null, mode, null, CallCenterTools.DefaultId);
        }


        public static int CreatePerson(string name, string password, AgentTaskChoiceMode mode, int[] parentSids, DialType dialType = DialType.Landline)
        {
            return CreatePerson(name, password, mode, parentSids, CallCenterTools.DefaultId, dialType: dialType);
        }

        public static int CreatePerson(string name, string password, AgentTaskChoiceMode mode,
            int[] parentSids, int callCenterId, AgentType personType = AgentType.LiveAgent, DialType dialType = DialType.Landline)
        {
            return CreatePerson(name, password, "", mode, parentSids, callCenterId, personType, dialType);
        }

        /// <summary>
        /// Creates new CATI Interviewer.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="password">The password.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="mode">Interviewer task choice mode.</param>
        /// <param name="parentSids">SIDs of parent groups.</param>
        /// <param name="callCenterId">Id of call center.</param>
        /// <param name="personType">Type of a person. Ivr or Live agent.</param>
        /// <returns>SID of created interviewer.</returns>
        public static int CreatePerson(string name, string password, string displayName, AgentTaskChoiceMode mode, int[] parentSids, int callCenterId, AgentType personType = AgentType.LiveAgent, DialType dialType = DialType.Landline)
        {
            if (name == null)
                name = "test person";

            if (parentSids == null)
            {
                parentSids = new[] { PersonGroupService.RootGroupId };
            }

            var person = new BvPersonEntity
            {
                Name = name,
                ManualSelection = (int)mode,
                Description = "",
                FullName = displayName,
                CallCenterID = callCenterId,
                Type = (byte)personType,
                DialTypeId = (byte)dialType
            };

            var personSID = ServiceLocator.Resolve<IPersonRepository>().Insert(person);

            PersonService.SetParentGroups(personSID, parentSids);

            if (password != null)
            {
                ServiceLocator.Resolve<IPasswordSaver>().Save(personSID, password);
            }

            return personSID;
        }

        public static int CreatePerson(string personName, string password, AgentTaskChoiceMode mode, DialType dialType = DialType.Landline)
        {
            return CreatePerson(personName, password, mode, null, CallCenterTools.DefaultId, dialType: dialType);
        }

        public static int CreatePerson(string personName, string password, AgentTaskChoiceMode mode, int callCenterId, DialType dialType = DialType.Landline)
        {
            return CreatePerson(personName, password, mode, null, callCenterId, dialType: dialType);
        }

        public static int CreatePersonGroup(string personGroupName, bool IsAdministrative = false)
        {
            if (String.IsNullOrEmpty(personGroupName))
                personGroupName = "test person group " + Guid.NewGuid();

            var bvPersonGroupEntity = new BvPersonGroupEntity
            {
                Name = personGroupName,
                IsAdministrative = IsAdministrative
            };
            int personGroupSid = ServiceLocator.Resolve<IPersonGroupRepository>().Insert(bvPersonGroupEntity);
            return personGroupSid;
        }

        public static int CreatePersonGroup(string personGroupName, int[] parentGroupsIDs)
        {
            int personGroupSid = CreatePersonGroup(personGroupName);
            List<int> parentGroups = PersonGroupService.GetParentGroups(personGroupSid).ToList();
            parentGroups.AddRange(parentGroupsIDs);
            PersonGroupService.SetParentGroups(personGroupSid, parentGroups.ToArray());

            return personGroupSid;
        }

        public static int CreateGroupAsRootChild(string personGroupName)
        {
            int personGroupSid = CreatePersonGroup(personGroupName);
            PersonGroupService.SetParentGroups(personGroupSid, new int[] { PersonGroupService.RootGroupId });

            return personGroupSid;
        }

        public static int CreateAssignAndLoginPersonOnSurvey(int surveySid, string userName, AgentTaskChoiceMode mode, DialType dialType = DialType.Landline)
        {
            int personSid = CreatePerson(userName, "p1", mode, dialType);

            BackendTools.AssignCatiPersonToSurvey(surveySid, personSid);

            BackendTools.LoginPerson(personSid, "");

            if (mode == AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personSid, surveySid);
            }

            return personSid;
        }

        public static void UpdatePersonMode(int personId, AgentTaskChoiceMode mode)
        {
            BvPersonEntity person = PersonRepository.GetById(personId);
            PersonService.UpdatePersonMode(person, mode, null, true);
        }

        public static void UpdateAttributesAndFullName(int personId, string fullName, string[] attributes)
        {
            BvPersonEntity person = PersonRepository.GetById(personId);
            person.FullName = fullName;
            person.Attribute1 = attributes[0];
            person.Attribute2 = attributes[1];
            person.Attribute3 = attributes[2];
            person.Attribute4 = attributes[3];
            person.Attribute5 = attributes[4];
            PersonRepository.Update(person);
        }
        
        public static int CreateAndAssignPersonGroupOnSurvey(int surveySid, string groupName)
        {
            int groupSid = CreatePersonGroup(groupName);

            BackendTools.AssignCatiPersonToSurvey(surveySid, groupSid);

            return groupSid;
        }

        public static void RemovePerson(int personId)
        {
            ServiceLocator.Resolve<IPersonRepository>().Delete(personId);
        }
        
        public static void RemovePersonGroup(int groupId)
        {
            PersonGroupRepository.Delete(groupId);
        }
    }
}
