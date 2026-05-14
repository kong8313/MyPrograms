using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Monitoring
{
    /// <summary>
    /// Class identifies single audio record.
    /// </summary>
    [Serializable]
    public class AudioIdentityObject
    {
        #region Properties

        /// <summary>
        /// Gets/sets identifier of audio record.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets/sets audio file name.
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is AudioIdentityObject))
            {
                return false;
            }

            AudioIdentityObject other = (AudioIdentityObject)obj;
            return this.ID == other.ID;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return ID.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Represents collection of audio record identities.
    /// </summary>
    [XmlRoot("AudioIdentityObject")]
    public class AudioIdentityObjectCollection : List<AudioIdentityObject>
    {
    }
}
