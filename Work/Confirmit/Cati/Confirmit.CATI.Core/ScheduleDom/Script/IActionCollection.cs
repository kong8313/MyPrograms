using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.ScheduleDom.Script
{
    public interface IActionCollection
    {
        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The object to add to the collection.</param>
        /// <exception cref="ArgumentNullException">Action is null or it's identifier
        /// is not initialiazed.</exception>
        void Add(Action item);

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>true if item is found in the ICollection; otherwise, false.</returns>
        bool Contains(Action item);

        /// <summary>
        /// Copies the elements of the collection to an Array, starting at a particular Array index. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements 
        /// copied from collection. The Array must have zero-based indexing.
        ///</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        void CopyTo(Action[] array, int arrayIndex);

        /// <summary>
        /// Gets the number of elements contained in the collection. 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a value indicating whether the ICollection is read-only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// This property is used only for serialization, because we couldn't serialize
        /// IDictionary interface.
        /// </summary>
        Action[] Actions { get; set; }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection. 
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>true if item was successfully removed from the collection; 
        /// otherwise, false. This method also returns false if item is not found
        /// in the original collection. </returns>
        bool Remove(Action item);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator that can be used to iterate through the collection.</returns>
        IEnumerator<Action> GetEnumerator();

        /// <summary>
        /// Returns action object by it's identifier.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <returns>The action, if exists, otherwise null.</returns>
        Action GetActionById(int actionId);
    }
}