using System;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
	/// <summary>
	/// Represents lite shift type object which has identifier, name and object identifier.
	/// </summary>
    public class ShiftType
	{
		#region Fields

		private int m_id;
		private String m_name;
		private int m_objectId;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes new shift type object with identifier, name and object identifier.
		/// </summary>
		/// <param name="id">Shift type identifier.</param>
		/// <param name="name">Shift type name.</param>
		/// <param name="objectId">Object identifier from Fusion. It is not shift
		/// type identifier.</param>
		public ShiftType(int id, string name, int objectId)
		{
			m_id = id;
			m_name = name;
			m_objectId = objectId;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Shift type identifier.
		/// </summary>
		public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

		/// <summary>
		/// Shift type name.
		/// </summary>
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

		/// <summary>
		/// Object identifier. This identifier differs from Id property.
		/// </summary>
		public int ObjectId
		{
			get { return m_objectId; }
			set { m_objectId = value; }
		}

		#endregion
	}
}
