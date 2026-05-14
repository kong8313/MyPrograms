using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Confirmit.CATI.Core.ScheduleDom.Resources;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents rule. Rule contains identifier, description, sample update flag and collection of sub-rules.
	/// The rule identifier is Guid identifier. This class does nothing, it is just a container.
	/// </summary>
	[Serializable]
	public class Rule : BaseObject<Guid>
	{
	    private string _description = string.Empty;
        private bool _sampleUpdate;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Rule()
		{
			SubRules = new SubRuleCollection();
		}

		/// <summary>
		/// Protected copying constructor. 
		/// </summary>
		/// <param name="rule">Object to copy.</param>
		protected Rule( Rule rule )
		{
			if(rule == null)
			{
				throw new ArgumentNullException( "rule", Strings.ItemNullExceptionMessage );
			}

			Id = rule.Id;
			Description = rule.Description;
            SampleUpdate = rule.SampleUpdate;
			SubRules = (SubRuleCollection)rule.SubRules.Clone();
		}

	    /// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new Rule( this );
		}

	    /// <summary>
		/// Rule description.
		/// </summary>
		[XmlElement]
		public string Description
		{
			get => _description;
            set => _description = SchedulingUtilities.ConvertForXml( value );
        }

		/// <summary>
		/// Execute during sample update
		/// </summary>
		[DefaultValue(false), XmlAttribute("SampleUpdate")]
		public bool SampleUpdate
		{
            get => _sampleUpdate;
            set => _sampleUpdate = value;
        }

		/// <summary>
		/// Collection of sub-rules which belong to current rule.
		/// </summary>
		[XmlArray]
	    public SubRuleCollection SubRules { get; set; }
	}

	/// <summary>
	/// Represents the collection of rules.
	/// </summary>
	[XmlRoot("Rules")]
	[Serializable]
	public class RuleCollection : BaseCollection<Rule, Guid>
	{
	    /// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return SchedulingUtilities.CloneBaseCollection<RuleCollection, Rule, Guid>(
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
