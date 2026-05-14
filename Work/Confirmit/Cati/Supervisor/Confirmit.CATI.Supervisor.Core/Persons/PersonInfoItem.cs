using System;

namespace Confirmit.CATI.Supervisor.Core.Persons
{
	/// <summary>
	/// Represents simple representation of person info.
	/// </summary>
	public class PersonInfoItem
	{
		private int m_Id;
		private string m_Name;

		/// <summary>
		/// Gets item Id.
		/// </summary>
		public int Id
		{
			get { return m_Id; }
		}

		/// <summary>
		/// Gets item name.
		/// </summary>
		public string Name
		{
			get { return m_Name; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public PersonInfoItem(int id, string name)
		{
			m_Id = id;
			m_Name = name;
		}

		/// <summary>
		/// Overridden Equals() method.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != this.GetType())
				return false;
			PersonInfoItem item = (PersonInfoItem)obj;
			return (m_Id == item.Id) && (m_Name == item.Name);
		}

		/// <summary>
		/// Overridden GetHashCode() method.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (m_Id.ToString() + m_Name).GetHashCode();
		}
	}
}
