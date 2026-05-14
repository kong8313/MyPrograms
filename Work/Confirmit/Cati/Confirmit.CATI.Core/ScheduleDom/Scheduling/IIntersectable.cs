using System;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Provides the base interfase for objects which could intersect.
	/// </summary>
	/// <typeparam name="T">Type of object.</typeparam>
	public interface IIntersectable<T>
	{
		/// <summary>
		/// Determines if current object has intersection with given object.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <returns>true, if object intersects; otherwise false.</returns>
		bool HasIntersection( T obj );
	}
}
