using System;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents base class for objects in Scheduling namespace.
	/// This class is parameterized with type of identifier of object.
	/// </summary>
	/// <typeparam name="T">Type of object identifier. It must be value type.
	/// </typeparam>
	[Serializable]
	public abstract class BaseObject<T> : ICloneable
		where T : struct
	{
	    private T? m_id;

	    /// <summary>
		/// Identifier. It is nullable value. If this value is null that means
		/// that object is not initialized.
		/// </summary>
		public virtual T? Id
		{
			get { return m_id; }
			set { m_id = value; }
		}

	    /// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public abstract object Clone();
	}
}
