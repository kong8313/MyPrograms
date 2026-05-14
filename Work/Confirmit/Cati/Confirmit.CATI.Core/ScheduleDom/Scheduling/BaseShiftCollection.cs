using System;
using Confirmit.CATI.Core.ScheduleDom.Resources;
//using FusionLib.Common;
//using FusionLib.Timezones;
using System.Diagnostics;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Provides the base class for shift collections in Scheduling namespace.
	/// </summary>
	/// <typeparam name="TShift">The type of the shift of the collection.</typeparam>
	/// <typeparam name="TShiftData">Shift data type. This type have to implement
	/// <see cref="IVerifiable"/> interface.</typeparam>
	[Serializable]
	public abstract class BaseShiftCollection<TShift, TShiftData> : BaseIdInt32Collection<TShift>
		where TShift : BaseShift<TShiftData>
		where TShiftData : IIntersectable<TShiftData>
	{
	    /// <summary>
		/// Gets or sets the element at the specified index. If setting is successful, adds event handler to 
		/// item validating event.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// index is less than zero and index is greater than Count.</exception>
		/// <exception cref="ArgumentNullException">Assigned data is null.</exception>
		/// <exception cref="ArgumentException">Assigned item is in invalid state.</exception>
		public override TShift this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				if(value == null)
				{
					throw new ArgumentNullException( "item", Strings.ItemNullExceptionMessage );
				}

                base[index] = value;
			}
		}

	    /// <summary>
		/// Adds item to the BaseShiftCollection. If success, adds event handler to 
		/// item validating event.
		/// </summary>
		/// <param name="item">The object to add to the BaseShiftCollection.</param>
		/// <exception cref="ArgumentNullException">Assigned item is null.</exception>
		/// <exception cref="ArgumentException">Assigned item is in invalid state.</exception>
		public override void Add( TShift item )
		{
			if(item == null)
			{
				throw new ArgumentNullException( "item", Strings.ItemNullExceptionMessage );
			}

            base.Add(item);
		}

		/// <summary>
		/// Inserts an element into the BaseShiftCollection at the specified index. 
		/// If success, adds event handler to item validating event.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert. The value can be a null reference for reference types.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// index is less than zero and index is greater than Count.</exception>
		/// <exception cref="ArgumentNullException">Assigned item is null.</exception>
		/// <exception cref="ArgumentException">Assigned item is in invalid state.</exception>
		public override void Insert( int index, TShift item )
		{
			if(item == null)
			{
				throw new ArgumentNullException( "item", Strings.ItemNullExceptionMessage );
			}

            base.Insert(index, item);
		}

		/// <summary>
		/// Removes the element at the specified index of the BaseCollection.
		/// If success, removes event handler from item validating event.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <param name="errors">Returns the collection of errors which was occured during removing.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// index is less than zero and index is greater than Count.</exception>
		public override bool RemoveAt( int index, out ErrorCollection errors )
		{
			var result = base.RemoveAt( index, out errors );

			return result;
		}

	    /// <summary>
		/// Returns shifts which have data for specified timezone.
		/// </summary>
		/// <typeparam name="TCollection">The type of collection to be returned.</typeparam>
		/// <param name="timezoneId">Timezone identifier.</param>
		/// <returns>The collection of shifts.</returns>
		/// <remarks>This method has unit tests in ShiftCollectionTest class.</remarks>
		public TCollection GetItemsForTimezone<TCollection>( int timezoneId )
			where TCollection : BaseShiftCollection<TShift, TShiftData>
		{
			TCollection result =
				Activator.CreateInstance<TCollection>();

			foreach(TShift shift in this)
			{
				if(shift.HasTimezone( timezoneId ))
				{
					result.AddWithoutValidityCheck( shift );
				}
			}

			return result;
		}
	

		/// <summary>
		/// Determines if collection contains items with given shift type or not.
		/// </summary>
		/// <param name="shiftType">Shift type.</param>
		/// <param name="IsExclusion">Flag indicates in which collection we are searching.
		/// true means exclusions, otherwise shifts.</param>
		/// <param name="errors">Error collection.</param>
		/// <returns>true, if collection contains items with given shift type, otherwise false.</returns>
		public bool ContainsItemsWithShiftType( ShiftType shiftType, bool IsExclusion, ErrorCollection errors )
		{
			int oldCount = errors.Count;

			foreach(BaseShift<TShiftData> item in this)
			{
				if(item.ShiftTypeId.Value == shiftType.Id.Value)
				{
					errors.Add( new Error(
						String.Format( 
							Strings.ShiftTypeIsInUseMessage, 
							shiftType.Name, 
							IsExclusion ? "exclusion" : "shift", 
							item.Id 
							) ) );
				}
			}

			return (oldCount != errors.Count);
		}
	}
}
