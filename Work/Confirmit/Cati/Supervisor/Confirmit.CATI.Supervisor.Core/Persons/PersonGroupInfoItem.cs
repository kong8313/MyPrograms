using System;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Core.Persons
{
	/// <summary>
	/// Represents simple representation of person info.
	/// </summary>	
	public class PersonGroupInfoItem
	{
		public int Id { get; }
	    public string Name { get; }
	    public string Description { get; }
	    public InboundGroupBehavior InboundCallBehavior { get; }
	    public TransferGroupBehavior CallTransferBehavior { get; }
	    public int Count { get; }
	    public bool IsAdministrative { get; }


	    /// <summary>
		/// Constructor.
		/// </summary>
		public PersonGroupInfoItem(int id, string name, string description, InboundGroupBehavior inboundCallBehavior, TransferGroupBehavior callTransferBehavior, int count, bool isAdministrative = false)
		{
			Id = id;
			Name = name;
		    Description = description;
		    InboundCallBehavior = inboundCallBehavior;
		    CallTransferBehavior = callTransferBehavior;
            Count = count;
            IsAdministrative = isAdministrative;
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
			PersonGroupInfoItem item = (PersonGroupInfoItem)obj;
			return (Id == item.Id) 
			       && (Name == item.Name) 
			       && (Description == item.Description) 
			       && (InboundCallBehavior == item.InboundCallBehavior) 
			       && (CallTransferBehavior == item.CallTransferBehavior) 
			       && (Count == item.Count)
			       && (IsAdministrative == item.IsAdministrative);
		}

		/// <summary>
		/// Overridden GetHashCode() method.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (Id + Name + Description + InboundCallBehavior + CallTransferBehavior + Count + IsAdministrative).GetHashCode();
		}
	}
}
