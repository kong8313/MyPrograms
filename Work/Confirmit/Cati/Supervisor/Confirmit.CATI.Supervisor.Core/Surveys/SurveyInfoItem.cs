using System;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
    /// <summary>
    /// Represents simple survey info.
    /// </summary>
    [Serializable]
    public class SurveyInfoItem
    {
        /// <summary>
        /// Gets item Id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets item name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets item description.
        /// </summary>
        public string ConfirmitID { get; protected set; }

        public int AssignedCallsCount { get; private set; }

        public int DefaultOrderId { get; set; }

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public SurveyInfoItem(int id)
        {
            Id = id;
            Name = String.Empty;
            ConfirmitID = String.Empty;
            DefaultOrderId = id;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SurveyInfoItem(int id, string name)
        {
            Id = id;
            Name = name;
            ConfirmitID = String.Empty;
            DefaultOrderId = id;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SurveyInfoItem(int id, string name, string confirmitID)
        {
            Id = id;
            Name = name;
            ConfirmitID = confirmitID;
            DefaultOrderId = id;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SurveyInfoItem(int id, string name, string confirmitID, int count)
        {
            Id = id;
            Name = name;
            ConfirmitID = confirmitID;
            AssignedCallsCount = count;
            DefaultOrderId = id;
        }

        public SurveyInfoItem(int id, string name, string confirmitID, int count, int defaultOrderId)
        {
            Id = id;
            Name = name;
            ConfirmitID = confirmitID;
            AssignedCallsCount = count;
            DefaultOrderId = defaultOrderId;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overridden Equals() method.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            SurveyInfoItem item = (SurveyInfoItem)obj;
            return (Id == item.Id) && (Name == item.Name);
        }

        /// <summary>
        /// Overridden GetHashCode() method.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (Id.ToString() + Name).GetHashCode();
        }

        #endregion

    }
}