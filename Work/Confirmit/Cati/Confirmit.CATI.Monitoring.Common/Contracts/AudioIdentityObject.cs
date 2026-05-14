using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Confirmit.CATI.Monitoring.Common.Contracts
{
    /// <summary>
    /// Class identifies single audio record.
    /// </summary>
    [DataContract(Namespace = "http://www.confirmit.com/")]
    [Serializable]
    public class AudioIdentityObject
    {
        /// <summary>
        /// Gets/sets identifier of audio record.
        /// </summary>
        [DataMember]
        public string ID { get; set; }

        /// <summary>
        /// Gets/sets name of audio file.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets moment of starting of audio file.
        /// </summary>
        [DataMember]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets ID of dialer
        /// </summary>
        [DataMember]
        public int DialerId { get; set; }

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

            var other = (AudioIdentityObject)obj;

            return (ID == other.ID);
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
            return ID;
        }
    }

    /// <summary>
    /// Represents collection of audio record identities.
    /// </summary>
    [XmlRoot("AudioIdentityObject")]
    public class AudioIdentityObjectCollection : List<AudioIdentityObject>
    {
    }
}
