using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Timezones
{
    /// <summary>
    /// Represents collection of BvTimezoneEntity objects.
    /// </summary>
    [Serializable]
    public class BvTimezoneEntityCollection : ICollection<BvTimezoneEntity>
    {
        #region Fields

        private Dictionary<int, BvTimezoneEntity> m_dictionary = new Dictionary<int, BvTimezoneEntity>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes new instance of BvTimezoneEntityCollection class.
        /// </summary>
        public BvTimezoneEntityCollection()
        {
        }

        /// <summary>
        /// Initializes new instance of BvTimezoneEntityCollection class and fills it with 
        /// given data.
        /// </summary>
        /// <param name="timezones">BvTimezoneEntity items collection.</param>
        public BvTimezoneEntityCollection(IEnumerable<BvTimezoneEntity> timezones)
        {
            if (timezones == null)
            {
                throw new ArgumentNullException("timezones");
            }

            foreach (BvTimezoneEntity timezone in timezones)
            {
                Add(timezone);
            }
        }

        #endregion

        #region ICollection<BvTimezoneEntity> Members

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The object to add to the collection.</param>
        /// <exception cref="ArgumentNullException">TimezoneLite is null.</exception>
        public void Add(BvTimezoneEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            m_dictionary.Add(item.ID, item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            m_dictionary.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>true if item is found in the ICollection; otherwise, false.</returns>
        /// <remarks>Item is null.</remarks>
        public bool Contains(BvTimezoneEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return m_dictionary.ContainsKey(item.ID);
        }

        /// <summary>
        /// Copies the elements of the collection to an Array, starting at a particular Array index. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements 
        /// copied from collection. The Array must have zero-based indexing.
        ///</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(BvTimezoneEntity[] array, int arrayIndex)
        {
            m_dictionary.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the collection. 
        /// </summary>
        public int Count
        {
            get { return m_dictionary.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the ICollection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection. 
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>true if item was successfully removed from the collection; 
        /// otherwise, false. This method also returns false if item is not found
        /// in the original collection. </returns>
        /// <remarks>Item is null.</remarks>
        public bool Remove(BvTimezoneEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return m_dictionary.Remove(item.ID);
        }

        #endregion

        #region IEnumerable<TimezoneLite> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<BvTimezoneEntity> GetEnumerator()
        {
            return m_dictionary.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_dictionary.Values.GetEnumerator();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns time zone object by it's identifier.
        /// </summary>
        /// <param name="timezoneId">Timezone identifier.</param>
        /// <param name="timezone">Returns TimezoneLite object, if exists; otherwise
        /// defualt value for TimezoneLite.</param>
        /// <returns>true, if time zone exists, otherwise false.</returns>
        public bool TryGetItemById(int timezoneId, out BvTimezoneEntity timezone)
        {
            timezone = null;

            return m_dictionary.TryGetValue(timezoneId, out timezone);
        }

        #endregion
    }
}
