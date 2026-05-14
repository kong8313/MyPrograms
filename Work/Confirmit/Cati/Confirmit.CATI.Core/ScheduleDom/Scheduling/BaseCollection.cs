using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Resources;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
    /// <summary>
    /// Provides the base class for collections in Scheduling namespace.
    /// </summary>
    /// <typeparam name="TItem">The type of elements of the collection.</typeparam>
    /// <typeparam name="TItemId">The type of identifier of element.</typeparam>
    [Serializable]
    public abstract class BaseCollection<TItem, TItemId> : IBaseCollection<TItem, TItemId>, ICloneable
        where TItem : BaseObject<TItemId>
        where TItemId : struct
    {
        /// <summary>
        /// Container for collection data.
        /// </summary>
        protected Collection<TItem> m_collection = new Collection<TItem>();

        /// <summary>
        /// Gets the number of elements actually contained in the BaseCollection.
        /// </summary>
        public int Count
        {
            get
            {
                return m_collection.Count;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero and index is greater than Count.</exception>
        /// <exception cref="ArgumentNullException">Assigned data is null.</exception>
        /// <exception cref="ArgumentException">Assigned item is in invalid state.</exception>
        /// <remarks>This function has unit tests.</remarks>
        public virtual TItem this[int index]
        {
            get
            {
                return m_collection[index];
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("item", Strings.ItemNullExceptionMessage);
                }

                var validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
                ErrorCollection errors;
                if (!validator.Validate(value, out errors))
                {
                    throw new ArgumentException(errors.ToString(), "item");
                }

                if (!validator.ValidateWithCollection(this, value, out errors))
                {
                    throw new ArgumentException(errors.ToString(), "item");
                }

                m_collection[index] = value;
            }
        }

        /// <summary>
        /// Adds item to the BaseCollection.
        /// </summary>
        /// <param name="item">The object to add to the BaseCollection.</param>
        /// <exception cref="ArgumentNullException">Assigned item is null.</exception>
        /// <exception cref="ArgumentException">Assigned item is in invalid state.</exception>
        /// <remarks>This function has unit tests.</remarks>
        public virtual void Add(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", Strings.ItemNullExceptionMessage);
            }

            var validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
            ErrorCollection errors;
            if (!validator.Validate(item, out errors))
            {
                throw new ArgumentException(errors.ToString(), "item");
            }

            if (!validator.ValidateWithCollection(this, item, out errors))
            {
                throw new ArgumentException(errors.ToString(), "item");
            }

            m_collection.Add(item);
        }

        /// <summary>
        /// Inserts an element into the BaseCollection at the specified index. 
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be a null reference for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero and index is greater than Count.</exception>
        /// <exception cref="ArgumentNullException">Assigned item is null.</exception>
        /// <exception cref="ArgumentException">Assigned item is in invalid state.</exception>
        /// <remarks>This function has unit tests.</remarks>
        public virtual void Insert(int index, TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", Strings.ItemNullExceptionMessage);
            }

            var validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
            ErrorCollection errors;
            if (!validator.Validate(item, out errors))
            {
                throw new ArgumentException(errors.ToString(), "item");
            }

            if (!validator.ValidateWithCollection(this, item, out errors))
            {
                throw new ArgumentException(errors.ToString(), "item");
            }

            m_collection.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the BaseCollection. 
        /// </summary>
        /// <param name="item">The object to remove from the BaseCollection. 
        /// The value can be a null reference for reference types.</param>
        /// <param name="errors">Returns the collection of errors.</param>
        /// <returns>true if item is successfully removed; otherwise, false. 
        /// This method also returns false if item was not found in the original BaseCollection.</returns>
        /// <exception cref="ArgumentNullException">Removed item is null.</exception>
        /// <remarks>This function has unit tests.</remarks>
        public bool Remove(TItem item, out ErrorCollection errors)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", Strings.RemovedItemCannotBeNullException);
            }

            bool result = true;
            int index = IndexOf(item);
            if (index < 0)
            {
                result = false;
                errors = new ErrorCollection();
                errors.Add(new Error(String.Format(Strings.ItemNotFoundMessage, item.Id.ToString())));
            }
            else
            {
                result = RemoveAt(index, out errors);
            }

            return result;
        }

        /// <summary>
        /// Removes the element at the specified index of the BaseCollection. 
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <param name="errors">Returns the collection of errors.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero and index is greater than Count.</exception>
        /// <returns>true, if item was successfully removed, otherwise false.</returns>
        public virtual bool RemoveAt(int index, out ErrorCollection errors)
        {
            bool result = true;
            errors = new ErrorCollection();
            RemovingEventArgs e = new RemovingEventArgs(index, false);
            OnRemoving(e);
            if (e.Cancel)
            {
                errors = e.Errors;
                result = false;
            }
            else
            {
                m_collection.RemoveAt(index);
            }

            return result;
        }

        /// <summary>
        /// Removes all elements from the BaseCollection.
        /// </summary>
        public void Clear()
        {
            m_collection.Clear();
        }

        /// <summary>
        /// Returns new identifier for object. This identifier doesn't exists in this collection.
        /// </summary>
        public abstract TItemId GetNewId();

        /// <summary>
        /// Returns item by it's identifier.
        /// </summary>
        /// <param name="id">Item identifier.</param>
        /// <returns>The item, if exists; otherwise default value for item type.</returns>
        /// <remarks>This function has unit tests.</remarks>
        public TItem GetItemById(TItemId id)
        {
            TItem result = default(TItem);

            foreach (TItem item in m_collection)
            {
                if (item.Id.HasValue && Object.Equals(item.Id.Value, id))
                {
                    result = item;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Searches for the specified object and returns the 
        /// zero-based index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="item">Item identifier.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire Collection, 
        /// if found; otherwise, -1. </returns>        
        public int IndexOf(TItem item)
        {
            return m_collection.IndexOf(item);
        }

        /// <summary>
        /// Removes item with specified identifier.
        /// </summary>
        /// <param name="id">Item identifier.</param>
        /// <returns>true, if item was successfully removed; otherwise false.</returns>
        /// <param name="errors">Returns the collection of errors.</param>
        /// <remarks>This function has unit tests.</remarks>
        public bool RemoveById(TItemId id, out ErrorCollection errors)
        {
            bool result = false;
            errors = new ErrorCollection();
            TItem foundItem = GetItemById(id);

            if (foundItem != default(TItem))
            {
                result = Remove(foundItem, out errors);
            }

            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// It is the part of <![CDATA[IBaseCollection<>]]> interface inherited from <![CDATA[IEnumerable<>]]>
        /// interface.
        /// </summary>
        /// <returns>An IEnumerator for the collection.</returns>
        public IEnumerator<TItem> GetEnumerator()
        {
            return m_collection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// It is the part of IBaseCollection generic interface inherited from IEnumerable 
        /// interface.
        /// </summary>
        /// <returns>An IEnumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_collection.GetEnumerator();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance. 
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public abstract object Clone();

        /// <summary>
        /// Swaps to objects within the BaseCollection.
        /// </summary>
        /// <param name="index1">First object index.</param>
        /// <param name="index2">Second object index.</param>
        /// <exception cref="ArgumentOutOfRangeException">Indices are less than zero,
        /// or are greater than collection size.</exception>
        public void Swap(int index1, int index2)
        {
            if (index1 < 0 || index1 >= m_collection.Count)
            {
                throw new ArgumentOutOfRangeException("index1", Strings.IndexOutOfRangeExceptionMessage);
            }

            if (index2 < 0 || index2 >= m_collection.Count)
            {
                throw new ArgumentOutOfRangeException("index2", Strings.IndexOutOfRangeExceptionMessage);
            }

            if (index1 != index2)
            {
                TItem item1 = m_collection[index1];
                TItem item2 = m_collection[index2];

                m_collection.RemoveAt(index1);
                m_collection.Insert(index1, item2);
                m_collection[index2] = item1;
            }
        }

        /// <summary>
        /// Adds item to the BaseCollection without validity check.
        /// </summary>
        /// <param name="item">The object to add to the BaseCollection.</param>
        /// <exception cref="ArgumentNullException">Assigned item is null.</exception>
        protected virtual void AddWithoutValidityCheck(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", Strings.ItemNullExceptionMessage);
            }

            m_collection.Add(item);
        }

        /// <summary>
        /// Occurs when the collection item is removing. 
        /// </summary>
        public event RemovingEventHandler Removing;

        /// <summary>
        /// Raises the Removing event. 
        /// </summary>
        /// <param name="e">A RemovingEventArgs that contains the event data.</param>
        protected void OnRemoving(RemovingEventArgs e)
        {
            if (Removing != null)
            {
                Removing(this, e);
            }
        }
    }
}
