using System;
using System.Xml.Serialization;
using Confirmit.CATI.Core.ScheduleDom.Resources;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents sub-rule. Sub-rule contains identifier, shift type and ITS identifiers,
	/// description, filter and collection of sub-rule actions. 
	/// The rule identifier is Guid identifier. This class does nothing, it is just a container.
	/// </summary>
	[Serializable]
	public class SubRule : BaseObject<Guid>
	{
	    private int? m_itsId = null;
		private int? m_shiftTypeId = null;
		private string m_filter = string.Empty;
		private bool m_filterEnabled = false;
		private string m_description = string.Empty;
		private SubRuleActionCollection m_collectionActions = new SubRuleActionCollection();

	    /// <summary>
		/// Default constructor.
		/// </summary>
		public SubRule()
			: base()
		{
		}

		/// <summary>
		/// Protected copying constructor. 
		/// </summary>
		/// <param name="subRule">Sub-rule to copy.</param>
		protected SubRule( SubRule subRule )
		{
			if(subRule == null)
			{
				throw new ArgumentNullException( "subRule", Strings.ItemNullExceptionMessage );
			}

			Id = subRule.Id;
			ItsId = subRule.ItsId;
			ShiftTypeId = subRule.ShiftTypeId;
			Filter = subRule.Filter;
			FilterEnabled = subRule.FilterEnabled;
			Description = subRule.Description;
			SubRuleActions = (SubRuleActionCollection)subRule.SubRuleActions.Clone();
		}

	    /// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new SubRule( this );
		}

	    /// <summary>
		/// ITS identifier. It is nullable value. If this value is null that means
		/// that object is not proper initialized.
		/// </summary>
		[XmlElement(IsNullable=true)]
		public int? ItsId
		{
			get { return m_itsId; }
			set { m_itsId = value; }
		}

		/// <summary>
		/// Shift type identifier. It is nullable value. If this value is null that means
		/// that object is not proper initialized.
		/// </summary>
		[XmlElement(IsNullable=true)]
		public int? ShiftTypeId
		{
			get { return m_shiftTypeId; }
			set { m_shiftTypeId = value; }
		}

		/// <summary>
		/// Filter of sub-rule. By default it is empty.
		/// </summary>
		[XmlElement]
		public string Filter
		{
			get { return m_filter; }
			set { m_filter = SchedulingUtilities.ConvertForXml( value ); }
		}

		/// <summary>
		/// Gets or sets a value indicating if the sub-rule filter is enabled. It is false by default.
		/// </summary>
		[XmlElement]
		public bool FilterEnabled
		{
			get { return m_filterEnabled; }
			set { m_filterEnabled = value; }
		}

		/// <summary>
		/// Sub-rule description.
		/// </summary>
		[XmlElement]
		public string Description
		{
			get { return m_description; }
			set { m_description = SchedulingUtilities.ConvertForXml( value ); }
		}

		/// <summary>
		/// Collection of actions which belong to current sub-rule.
		/// </summary>
		[XmlArray]
		public SubRuleActionCollection SubRuleActions
		{
			get { return m_collectionActions; }
			set { m_collectionActions = value; }
		}
	}

	/// <summary>
	/// Represents the collection of sub-rules.
	/// </summary>
	[XmlRoot("SubRules")]
	[Serializable]
	public class SubRuleCollection : BaseCollection<SubRule, Guid>
	{
	    /// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return SchedulingUtilities.CloneBaseCollection<SubRuleCollection, SubRule, Guid>(
				this
			);
		}

	    /// <summary>
		/// Returns new identifier for object. This identifier doesn't exists in this collection.
		/// </summary>
		public override Guid GetNewId()
		{
			return Guid.NewGuid();
		}
	}
}
