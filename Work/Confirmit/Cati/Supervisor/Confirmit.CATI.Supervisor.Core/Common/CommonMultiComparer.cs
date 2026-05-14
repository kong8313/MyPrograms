using System;
using System.Collections.Generic;
using System.Reflection;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Supervisor.Core.Common
{
	/// <summary>
	/// Performs multi-comparsion for two objects on passed sorting args collection (collection of pairs [PropertyName] :: [SortingDirection]).
	/// </summary>
	public class CommonMultiComparer<T> : IComparer<T>
	{
		private IEnumerable<SortingArgs> m_SortingArgsCollection;

		/// <summary>
		/// Creates comparer object for the specified sorting args collection (collection of pairs [PropertyName] :: [SortingDirection]).
		/// </summary>
		/// <param name="sortingArgsCollection">Collection of pairs [PropertyName] :: [SortingDirection].</param>
		public CommonMultiComparer(IEnumerable<SortingArgs> sortingArgsCollection)
		{
			m_SortingArgsCollection = sortingArgsCollection;
		}

		#region IComparer<T> Members
		/// <summary>
		/// Compares two objects. 
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns></returns>
		public int Compare(T x, T y)
		{
			int result = 0;
			foreach (SortingArgs args in m_SortingArgsCollection)
			{
                CommonComparer<T> comparer = new CommonComparer<T>(args.PropertyName, args.IsAscending);
                result = comparer.Compare(x, y);
				if (result != 0)
					break;
			}
			return result;
		}
		#endregion
	}
}