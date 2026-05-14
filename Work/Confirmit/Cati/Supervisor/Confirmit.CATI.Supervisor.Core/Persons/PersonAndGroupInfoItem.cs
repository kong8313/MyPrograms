namespace Confirmit.CATI.Supervisor.Core.Persons
{
    /// <summary>
    /// Simple representation of person or person group info.
    /// </summary>
    public class PersonAndGroupInfoItem
    {
        /// <summary>
        /// Gets item Id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets item name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets item description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Determins if object is group.
        /// </summary>
        public bool IsGroup { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PersonAndGroupInfoItem(int id, string name, string description, bool isGroup)
        {
            Id = id;
            Name = name;
            Description = description;
            IsGroup = isGroup;
        }

        /// <summary>
        /// Overridden Equals() method.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            PersonAndGroupInfoItem item = (PersonAndGroupInfoItem)obj;
            return (Id == item.Id) && (Name == item.Name);
        }

        /// <summary>
        /// Overridden GetHashCode() method.
        /// </summary>
        public override int GetHashCode()
        {
            return (Id + Name).GetHashCode();
        }
    }
}
