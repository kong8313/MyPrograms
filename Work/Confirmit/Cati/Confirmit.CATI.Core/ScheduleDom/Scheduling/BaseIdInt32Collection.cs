using System;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Provides the base class for collections in Scheduling namespace. 
	/// This collection contains objects with identifiers of integer type.
	/// </summary>
	/// <typeparam name="TItem">The type of elements of the collection.</typeparam>
	[Serializable]
	public abstract class BaseIdInt32Collection<TItem> : BaseCollection<TItem, int>
		where TItem : BaseObject<int>
	{
	    private int m_maxId = 0;

	    /// <summary>
		/// Adds item to the BaseCollection.
		/// </summary>
		/// <param name="item">The object to add to the BaseCollection.</param>
		/// <exception cref="ArgumentNullException">Assigned item is null.</exception>
		/// <exception cref="ArgumentException">Assigned item is in invalid state.</exception>
		/// <remarks>This function has unit tests.</remarks>
		public override void Add( TItem item )
		{
			base.Add( item );

			if(item.Id.Value > m_maxId)
			{
				m_maxId = item.Id.Value;
			}
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
		public override void Insert( int index, TItem item )
		{
			base.Insert( index, item );

			if(item.Id.Value > m_maxId)
			{
				m_maxId = item.Id.Value;
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
		public override TItem this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				base[index] = value;

				if(value.Id.Value > m_maxId)
				{
					m_maxId = value.Id.Value;
				}
			}
		}

		/// <summary>
		/// Adds item to the BaseCollection without validity check.
		/// </summary>
		/// <param name="item">The object to add to the BaseCollection.</param>
		/// <exception cref="ArgumentNullException">Assigned item is null.</exception>
		protected override void AddWithoutValidityCheck( TItem item )
		{
			base.AddWithoutValidityCheck( item );

			if(item.Id.Value > m_maxId)
			{
				m_maxId = item.Id.Value;
			}
		}

		/// <summary>
		/// Returns new identifier for object. This identifier doesn't exists in this collection.
		/// </summary>
		/// <remarks>This function has unit tests in ShiftTypeCollection class.</remarks>
		public override int GetNewId()
		{
			return ++m_maxId;
		}
	}
}
