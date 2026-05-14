using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.ScheduleDom.Resources;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// This class contains some utility methods for Scheduling namespace.
	/// </summary>
	public static class SchedulingUtilities
	{
		/// <summary>
		/// Compares two days of week. Compare depends on given first day of week.
		/// For example if first day of week is monday, monday is less than sunday.
		/// But if first day of week is sunday, monday is greater than sunday.
		/// </summary>
		/// <param name="first">First day.</param>
		/// <param name="second">Second day.</param>
		/// <param name="firstDayOfWeek">First day of week.</param>
		/// <returns>Less than zero, if first day is less than second;
		/// Zero, if first day equals second day;
		/// Greater than zero, if first day is greater than second day.</returns>
		internal static int CompareDaysOfWeek( DayOfWeek first, DayOfWeek second, DayOfWeek firstDayOfWeek )
		{
			int myFirst = SchedulingUtilities.DayOfWeekInInternalFormat( first );
			int mySecond = SchedulingUtilities.DayOfWeekInInternalFormat( second );
			int myFirstDayOfWeek = SchedulingUtilities.DayOfWeekInInternalFormat( firstDayOfWeek );

			int firstIndex = (7 + myFirst - myFirstDayOfWeek) % 7;
			int secondIndex = (7 + mySecond - myFirstDayOfWeek) % 7;

			return (firstIndex - secondIndex);
		}

		/// <summary>
		/// Compares two paires of (day of week, time).
		/// In this method we suppose that Monday
		/// is the first day of week.
		/// </summary>
		/// <param name="firstDay">First day of week.</param>
		/// <param name="firstTime">First time.</param>
		/// <param name="secondDay">Second day of week.</param>
		/// <param name="secondTime">Second time.</param>
		/// <returns>true, if first pair is less or equal than second.</returns>
		public static bool IsLessOrEqualPare( DayOfWeek firstDay, TimeSpan firstTime,
			DayOfWeek secondDay, TimeSpan secondTime )
		{
			return (CompareDayOfWeekTimePare( firstDay, firstTime, secondDay, secondTime ) <= 0);
		}

        /// <summary>
        /// Compares two paires of (day of week, time).
        /// In this method we suppose that Monday
        /// is the first day of week.
        /// </summary>
        /// <param name="firstDay">First day of week.</param>
        /// <param name="firstTime">First time.</param>
        /// <param name="secondDay">Second day of week.</param>
        /// <param name="secondTime">Second time.</param>
        /// <returns>true, if first pair is less or equal than second.</returns>
        public static bool IsLessPare(DayOfWeek firstDay, TimeSpan firstTime,
            DayOfWeek secondDay, TimeSpan secondTime)
        {
            return (CompareDayOfWeekTimePare(firstDay, firstTime, secondDay, secondTime) < 0);
        }

		/// <summary>
		/// Compares two paires of (day of week, time).
		/// In this method we suppose that Monday is the first day of week.
		/// </summary>
		/// <param name="firstDay">First day of week.</param>
		/// <param name="firstTime">First time.</param>
		/// <param name="secondDay">Second day of week.</param>
		/// <param name="secondTime">Second time.</param>
		/// <returns>Less than zero, if first day is less than second;
		/// Zero, if first day equals second day;
		/// Greater than zero, if first day is greater than second day.</returns>
		public static int CompareDayOfWeekTimePare( DayOfWeek firstDay, TimeSpan firstTime,
			DayOfWeek secondDay, TimeSpan secondTime )
		{
			int result = SchedulingUtilities.CompareDaysOfWeek( firstDay, secondDay, DayOfWeek.Monday );

			if(result == 0)
			{
				result = TimeSpan.Compare( firstTime, secondTime );
			}

			return result;
		}
        /// <summary>
        /// Return index of current day. Index depends on given first day of week.
        /// For example:
        /// If first day of week is monday, for monday 0 will be returned, for sunday - 6
        /// If first day of week is sunday, for monday 1 will be returned, for sunday - 0
        /// </summary>
        /// <param name="day"></param>
        /// <param name="firstDayOfWeek"></param>
        /// <returns></returns>
        public static int GetDayIndex(DayOfWeek day, DayOfWeek firstDayOfWeek)
        {
            int myDay = SchedulingUtilities.DayOfWeekInInternalFormat(day);
            int myFirstDayOfWeek = SchedulingUtilities.DayOfWeekInInternalFormat(firstDayOfWeek);
            return (7 + myDay - myFirstDayOfWeek) % 7;
        }

		/// <summary>
		/// Returns index of day of week in internal week representation.
		/// Internal week representation is:
		/// 0 - Sunday
		/// 1 - Monday
		/// 2 - Tuesday
		/// 3 - Wednesday
		/// 4 - Thursday
		/// 5 - Friday
		/// 6 - Saturday
		/// </summary>
		/// <param name="day">Day of week to convert.</param>
		/// <returns>Integer index.</returns>
		private static int DayOfWeekInInternalFormat( DayOfWeek day )
		{
			int result = 0;

			switch(day)
			{
				case DayOfWeek.Sunday: result = 0; break;
				case DayOfWeek.Monday: result = 1; break;
				case DayOfWeek.Tuesday: result = 2; break;
				case DayOfWeek.Wednesday: result = 3; break;
				case DayOfWeek.Thursday: result = 4; break;
				case DayOfWeek.Friday: result = 5; break;
				case DayOfWeek.Saturday: result = 6; break;
			}

			return result;
		}

		/// <summary>
		/// Converts given text for Xml properties in Scheduling.
		/// For now this function removes \r escape character from text.
		/// </summary>
		/// <param name="text">Text to convert.</param>
		/// <returns>Converted text.</returns>
		internal static string ConvertForXml( string text )
		{
			return (text == null ? String.Empty : text.Replace( "\r", "" ));
		}

		/// <summary>
		/// Combines two lists of items into one array with removing 
		/// duplicated items.
		/// </summary>
		/// <typeparam name="T">Type of items.</typeparam>
		/// <param name="list1">First list.</param>
		/// <param name="list2">Second list.</param>
		/// <returns>Array of items without duplications.</returns>
		internal static T[] Combine<T>( IEnumerable<T> list1, IEnumerable<T> list2 )
		{
			Dictionary<T, T> hash = new Dictionary<T, T>();

			foreach(T item in list1)
			{
				if(!hash.ContainsKey( item ))
				{
					hash.Add( item, item );
				}
			}

			foreach(T item in list2)
			{
				if(!hash.ContainsKey( item ))
				{
					hash.Add( item, item );
				}
			}

			T[] result = new T[hash.Keys.Count];
			hash.Keys.CopyTo( result, 0);

			return result;
		}

		/// <summary>
		/// Checks if given value could be converted to given type. Given type should
		/// implement IConvertible interface. If not, function returns false. The exception
		/// is Guid structure. Guid type is checked correctly in this method.
		/// </summary>
		/// <param name="value">String representation of value.</param>
		/// <param name="type">Type.</param>
		/// <returns>true, if string value could be converted; otherwise false.</returns>
		internal static bool CheckStringValueOfType( string value, Type type )
		{
			bool result = false;

			if(type != null)
			{
				try
				{
					if(type == Type.GetType( "System.Guid" ))
					{
						Guid guid = new Guid( value );
					}
					else
					{
						Convert.ChangeType( value, type );
					}

					result = true;
				}
				catch(ArgumentNullException /*ex*/)
				{
					// throws by Guid and ChangeType
				}
				catch(FormatException /*ex*/)
				{
					// throws by Guid and ChangeType
				}
				catch(OverflowException /*ex*/)
				{
					// throws by Guid
				}
				catch(InvalidCastException /*ex*/)
				{
					// throws by ChangeType
				}
			}

			return result;
		}

		/// <summary>
		/// Clones base collection object.
		/// </summary>
		/// <typeparam name="T">The type of the collection.</typeparam>
		/// <typeparam name="TItem">The type of elements of the collection.</typeparam>
		/// <typeparam name="TItemId">The type of identifier of element.</typeparam>
		/// <param name="obj">Object to clone.</param>
		/// <returns>Cloned object.</returns>
		internal static T CloneBaseCollection<T, TItem, TItemId>( T obj )
			where T : BaseCollection<TItem, TItemId>, new()
			where TItem : BaseObject<TItemId>
			where TItemId : struct
		{
			if(obj == null)
			{
				throw new ArgumentNullException( "obj", Strings.ItemNullExceptionMessage );
			}

			T result = new T();

			foreach(TItem item in obj)
			{
				result.Add( (TItem)item.Clone() );
			}

			return result;
		}

		/// <summary>
		/// Returns sub-rule from given schedule script by it's identifier.
		/// This function supposes that sub-rule identifier are unique in
		/// system.
		/// </summary>
		/// <param name="schedule">Schedule script in which sub-rule should be found.</param>
		/// <param name="subRuleId">Sub-rule identifier.</param>
		/// <returns>Sub-rule object, if exists; otherwise null.</returns>
		/// <exception cref="ArgumentNullException">Schedule script object if null.</exception>
		public static SubRule GetSubRuleById( Schedule schedule, Guid subRuleId )
		{
			if(schedule == null)
			{
				throw new ArgumentNullException( "schedule", Strings.ItemNullExceptionMessage );
			}

			SubRule result = null;

			foreach(Rule rule in schedule.Rules)
			{
				if((result = rule.SubRules.GetItemById( subRuleId )) != null)
				{
					break;
				}

			}

			return result;
		}
	}
}
