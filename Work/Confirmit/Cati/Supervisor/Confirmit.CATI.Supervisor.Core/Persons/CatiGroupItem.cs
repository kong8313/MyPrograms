using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;

namespace Confirmit.CATI.Supervisor.Core.Persons
{
    /// <summary>
    /// Stores information about CATI person groups.
    /// </summary>
    [Serializable]
    public class CatiGroupItem : ICatiPersonItem
    {
        private int m_Id = 0;
        private string m_Name = String.Empty;
        private string m_Description = String.Empty;

        #region Constructors
        /// <summary>
        /// Creates a new instance of CatiGroupItem object with given ID.
        /// </summary>
        /// <param name="id">Person group ID.</param>
        public CatiGroupItem(int id)
        {
            m_Id = id;
        }

        /// <summary>
        /// Creates a new instance of CatiGroupItem object with given ID and name.
        /// </summary>
        /// <param name="id">Person group ID.</param>
        /// <param name="name">Person group name.</param>
        public CatiGroupItem(int id, string name)
        {
            m_Id = id;
            m_Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Person group ID.
        /// </summary>
        public int Id
        {
            get { return m_Id; }
        }

        /// <summary>
        /// Person group name.
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_Name))
                    Init();
                return m_Name;
            }
            set { m_Name = value; }
        }

        /// <summary>
        /// Person group description.
        /// </summary>
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(m_Description))
                {
                    Init();
                }
                return m_Description;
            }
            set { m_Description = value; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified Object is equal to the current CatiGroupItem object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return GetHashCode() == obj.GetHashCode();
        }
        /// <summary>
        /// Serves as a hash function for a CatiGroupItem.
        /// </summary>
        /// <returns>A hash code for the current CatiGroupItem.</returns>
        public override int GetHashCode()
        {
            return Id;
        }

        //Lazy load
        public void Init()
        {
            BvPersonGroupEntity person = PersonGroupRepository.GetById(Id);
            m_Name = person.Name;
            m_Description = person.Description;
        }

        #endregion

    }
}