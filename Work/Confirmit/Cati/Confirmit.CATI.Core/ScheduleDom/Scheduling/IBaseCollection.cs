using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents basic interface of all collections in Scheduling namespace. 
	/// It is generic interface parameterized by class TItem.
	/// </summary>
	/// <typeparam name="TItem">The type of elements of the collection.</typeparam>
	/// <typeparam name="TItemId">The type of identifier of element.</typeparam>
	public interface IBaseCollection<TItem, TItemId> : IEnumerable<TItem>
		where TItem : BaseObject<TItemId>
		where TItemId : struct
	{
		/// <summary>
		/// Gets the number of elements actually contained in the IBaseCollection.
		/// </summary>
		int Count { get;}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// index is less than zero and index is greater than Count.</exception>
		/// <exception cref="ArgumentNullException">assigned data is null.</exception>
		TItem this[int index] { get; set; }

		/// <summary>
		/// Adds item to the IBaseCollection.
		/// </summary>
		/// <param name="item">The object to add to the IBaseCollection.</param>
		/// <exception cref="ArgumentNullException">assigned item is null.</exception>
		void Add( TItem item );

		/// <summary>
		/// Inserts an element into the IBaseCollection at the specified index. 
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert. The value can be a null reference for reference types.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// index is less than zero and index is greater than Count.</exception>
		/// <exception cref="ArgumentNullException">assigned item is null.</exception>
		void Insert( int index, TItem item );

		/// <summary>
		/// Removes the first occurrence of a specific object from the IBaseCollection. 
		/// </summary>
		/// <param name="item">The object to remove from the IBaseCollection. 
		/// The value can be a null reference for reference types.</param>
		/// <param name="errors">Returns the collection of errors which was occured during removing.</param>
		/// <returns>true if item is successfully removed; otherwise, false. 
		/// This method also returns false if item was not found in the original IBaseCollection.</returns>
		/// <exception cref="ArgumentNullException">removed item is null.</exception>
		bool Remove( TItem item, out ErrorCollection errors );

		/// <summary>
		/// Removes the element at the specified index of the IBaseCollection. 
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <param name="errors">Returns the collection of errors which was occured during removing.</param>
		/// <exception cref="ArgumentOutOfRangeException">index is less than zero and index is greater than Count.</exception>
		bool RemoveAt( int index, out ErrorCollection errors );

		/// <summary>
		/// Removes all elements from the IBaseCollection.
		/// </summary>
		void Clear();

		/// <summary>
		/// Returns item by it's identifier.
		/// </summary>
		/// <param name="id">Item identifier.</param>
		/// <returns>The item, if exists; otherwise default value for item type.</returns>
		TItem GetItemById( TItemId id );

		/// <summary>
		/// Removes item with specified identifier.
		/// </summary>
		/// <param name="id">Item identifier.</param>
		/// <param name="errors">Returns the collection of errors.</param>
		/// <returns>true, if item was successfully removed; otherwise false.</returns>
		bool RemoveById( TItemId id, out ErrorCollection errors );
	}
}
