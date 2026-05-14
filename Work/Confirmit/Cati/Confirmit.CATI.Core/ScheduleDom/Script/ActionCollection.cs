using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Confirmit.CATI.Core.ScheduleDom.Resources;

namespace Confirmit.CATI.Core.ScheduleDom.Script
{
    /// <summary>
    /// Represents the collection of actions.
    /// </summary>
    [XmlRoot("Actions")]
    public class ActionCollection : IActionCollection, ICollection<Action>
    {
        private readonly Dictionary<int, Action> _dictionary = new Dictionary<int, Action>();


        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The object to add to the collection.</param>
        /// <exception cref="ArgumentNullException">Action is null or it's identifier
        /// is not initialiazed.</exception>
        public void Add(Action item)
        {
            if (item == null || !item.Id.HasValue)
            {
                throw new ArgumentNullException("item", Strings.ItemNotInitializedExceptionMessage);
            }

            _dictionary.Add(item.Id.Value, item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>true if item is found in the ICollection; otherwise, false.</returns>
        public bool Contains(Action item)
        {
            return _dictionary.ContainsValue(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an Array, starting at a particular Array index. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements 
        /// copied from collection. The Array must have zero-based indexing.
        ///</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Action[] array, int arrayIndex)
        {
            _dictionary.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the collection. 
        /// </summary>
        public int Count
        {
            get { return _dictionary.Count; }
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
        public bool Remove(Action item)
        {
            bool result = false;
            int foundKey = -1;

            foreach (KeyValuePair<int, Action> pair in _dictionary)
            {
                if (pair.Value == item)
                {
                    result = true;
                    foundKey = pair.Key;

                    break;
                }
            }

            if (result)
            {
                result = _dictionary.Remove(foundKey);
            }

            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<Action> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns action object by it's identifier.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <returns>The action, if exists, otherwise null.</returns>
        public Action GetActionById(int actionId)
        {
            Action result;
            _dictionary.TryGetValue(actionId, out result);

            return result;
        }

        /// <summary>
        /// This property is used only for serialization, because we couldn't serialize
        /// IDictionary interface.
        /// </summary>
        public Action[] Actions
        {
            get
            {
                List<Action> result = new List<Action>(_dictionary.Values);

                return result.ToArray();
            }
            set
            {
                if (value != null)
                {
                    foreach (Action action in value)
                    {
                        Add(action);
                    }
                }
            }
        }
    }
}