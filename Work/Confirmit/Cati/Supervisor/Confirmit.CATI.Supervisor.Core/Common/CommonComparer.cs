using System;
using System.Collections.Generic;
using System.Reflection;

namespace Confirmit.CATI.Supervisor.Core.Common
{
	/// <summary>
	/// Compares two objects on values of some property.
	/// </summary>
	public class CommonComparer<T>: IComparer<T>
	{
		private string m_PropertyName;
		private bool m_IsSortAsc;
		
		/// <summary>
		/// Creates comparer object for specified name of property to compare and sorting order.
		/// </summary>
		/// <param name="propertyName">Name of property to compare.</param>
		/// <param name="isSortAsc">Sorting order (true for ascending, false for descending).</param>
		public CommonComparer(string propertyName, bool isSortAsc)
		{
			m_PropertyName = propertyName;
			m_IsSortAsc = isSortAsc;
		}

		#region IComparer<T> Members
		/// <summary>
		/// Compares two objects. 
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>-1 if the first object is lesser, than the second; 0 if equal; 1 if greater</returns>
		public int Compare(T x, T y)
		{
            PropertyInfo propertyInfo = x.GetType().GetProperty(m_PropertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);
			if (propertyInfo == null)
			{
				throw new InvalidOperationException(String.Format("Type {0} doesn't contain property {1}.", x.GetType(), m_PropertyName));
			}

			object xProperty = propertyInfo.GetValue(x, null);
			object yProperty = propertyInfo.GetValue(y, null);

            int result;

            if (xProperty == null || yProperty == null)
            {
                if (xProperty != null)
                {
                    result = 1;
                }
                else if (yProperty != null)
                {
                    result = -1;
                }
                else
                {
                    result = 0;
                }
            }
            else if (xProperty is IComparable)
			{
                if (xProperty is String)
                {
                    result = String.Compare((string)xProperty, (string)yProperty, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    result = ((IComparable)xProperty).CompareTo(yProperty);
                }
			}
			else
			{
                throw new InvalidOperationException(String.Format("Property {0} of type {1} doesn't inherit IComparable interface.", m_PropertyName, xProperty.GetType()));
			}
			return m_IsSortAsc? result : -result;
		}
		#endregion
	}
}