using System.Data;

namespace Confirmit.CATI.Supervisor.Core.Persons
{
	/// <summary>
	/// Class represents single row of persons with sites assigned to them.
	/// </summary>
	public class PersonsListRow
	{
	    /// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="row"></param>
		public PersonsListRow(DataRow row)
		{
			PersonSID = (int)row["PersonSID"];
            PersonName = (string)row["PersonName"];
            LoggedIn = (bool)row["LoggedIn"];
			PersonDescription = (string)row["PersonDescription"];
		}

        /// <summary>
        /// Determines whether the specified Object is equal to the current PersonsListRow.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) 
            {
                return false;
            }

            return GetHashCode() == obj.GetHashCode();
        }
        /// <summary>
        /// Serves as a hash function for a PersonsListRow.
        /// </summary>
        /// <returns>A hash code for the current PersonsListRow.</returns>
        public override int GetHashCode()
        {
            return PersonSID;
        }

	    /// <summary>
	    /// Person's SID.
	    /// </summary>
	    public int PersonSID { get; set; }

	    /// <summary>
	    /// Person's name.
	    /// </summary>
	    public string PersonName { get; set; }

	    /// <summary>
	    /// 'Logged in' flag. True if person is logged in, false otherwise.
	    /// </summary>
	    public bool LoggedIn { get; set; }

	    /// <summary>
	    /// Person's description.
	    /// </summary>
	    public string PersonDescription { get; set; }
	}
}