using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Core.SupervisorService;

namespace Confirmit.CATI.Supervisor.Core.Persons
{
    /// <summary>
    /// Base class for CATI user info classes.
    /// </summary>
    [Serializable]
    public class CatiUserItem : ICatiPersonItem
    {
        protected int m_Id;
        protected string m_Name;
        protected string m_Description;

        /// <summary>
        /// Creates a new instance of BaseUserInfo object with given ID.
        /// </summary>
        /// <param name="id"></param>
        public CatiUserItem(int id)
        {
            m_Id = id;
            Init();
        }

        /// <summary>
        /// Creates a new instance of CatiUserItem object with given ID and name.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="name">User login.</param>
        public CatiUserItem(int id, string name)
        {
            m_Id = id;
            m_Name = name;
            Init();
        }

        public CatiUserItem(int id, string name, string description)
        {
            m_Id = id;
            m_Name = name;
            m_Description = description;
        }

        /// <summary>
        /// User ID.
        /// </summary>
        public int Id
        {
            get { return m_Id; }
        }

        /// <summary>
        /// User login.
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// User description.
        /// </summary>
        public string Description
        {
            get
            {
                return m_Description;
            }
            set { m_Description = value; }
        }

        /// <summary>
        /// Exclude user from group.
        /// </summary>
        /// <param name="gId">Group ID to exclude from.</param>
        /// <param name="root">Root users group ID.</param>
        /// <remarks>If user becomes excluded from all groups, it included in the root group.</remarks>
        public void ExcludeFrom(int gId, int root)
        {
            List<int> groups = PersonService.GetParentGroups(Id).ToList();

            groups.Remove(gId);

            if (groups.Count == 0)
            {
                groups.Add(root);
            }

            var supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            supervisorServiceClient.SetPersonParentGroups(Id, groups.ToArray());
        }

        /// <summary>
        /// Assign user to group.
        /// </summary>
        /// <param name="gId">Group ID to assign to.</param>
        /// <remarks>If user was assigned to root group, it excluded from it.</remarks>
        public void AssignTo(int gId)
        {
            List<int> groups = PersonService.GetParentGroups(Id).ToList();

            groups.Add(gId);

            var supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            supervisorServiceClient.SetPersonParentGroups(Id, groups.ToArray());
        }

        /// <summary>
        /// Initialize object fields with values received from Fusion.
        /// </summary>
        public void Init()
        {
            BvPersonEntity person = PersonRepository.GetById(Id);

            if (string.IsNullOrEmpty(m_Name))
            {
                m_Name = person.Name;
            }
            if (string.IsNullOrEmpty(m_Description))
            {
                m_Description = person.Description;
            }
        }
    }
}