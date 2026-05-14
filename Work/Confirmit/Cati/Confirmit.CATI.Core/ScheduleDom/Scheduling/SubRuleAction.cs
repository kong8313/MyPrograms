using System;
using System.Xml.Serialization;
using Confirmit.CATI.Core.ScheduleDom.Resources;
using Action = Confirmit.CATI.Core.ScheduleDom.Script.Action;
using System.Reflection;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
    /// <summary>
    /// Represents sub-rule action. Sub-rule action contains identifier, action identifier,
    /// description, filter and parameter value. By default filter and parameter values
    /// are empty. This class does nothing, it is just a container.
    /// </summary>
    [Serializable]
    public class SubRuleAction : BaseObject<int>
    {
        private int? m_actionId = null;
        private string m_filter = string.Empty;
        private bool m_enabled = true;
        private string m_description = string.Empty;
        private Parameter m_parameter = new Parameter();
        private bool m_filterEnabled = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SubRuleAction()
            : base()
        {
        }

        /// <summary>
        /// Protected copying constructor. 
        /// </summary>
        /// <param name="subRuleAction">Sub-rule action to copy.</param>
        protected SubRuleAction(SubRuleAction subRuleAction)
        {
            if (subRuleAction == null)
            {
                throw new ArgumentNullException("subRuleAction", Strings.ItemNullExceptionMessage);
            }

            Id = subRuleAction.Id;
            ActionId = subRuleAction.ActionId;
            Filter = subRuleAction.Filter;
            Description = subRuleAction.Description;
            Enabled = subRuleAction.Enabled;
            FilterEnabled = subRuleAction.FilterEnabled;
            Parameter = new Parameter(subRuleAction.Parameter);
        }

        /// <summary>
        /// Action identifier. It is nullable value. If this value is null that means
        /// that object is not proper initialized.
        /// </summary>
        public int? ActionId
        {
            get { return m_actionId; }
            set { m_actionId = value; }
        }

        /// <summary>
        /// Filter of rule action. It is empty by default.
        /// </summary>
        [XmlElement]
        public string Filter
        {
            get { return m_filter; }
            set { m_filter = SchedulingUtilities.ConvertForXml(value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if the rule action is enabled. It is true by default.
        /// </summary>
        [XmlElement]
        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        /// <summary>
        /// Rule action description.
        /// </summary>
        [XmlElement]
        public string Description
        {
            get { return m_description; }
            set { m_description = SchedulingUtilities.ConvertForXml(value); }
        }

        /// <summary>
        /// String representation of parameter value of action. It is empty by default.
        /// </summary>
        [XmlElement("ParameterValue")]
        public Parameter Parameter
        {
            get { return m_parameter; }
            set { m_parameter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if the action filter is enabled. It is false by default.
        /// </summary>
        [XmlElement]
        public bool FilterEnabled
        {
            get { return m_filterEnabled; }
            set { m_filterEnabled = value; }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance. 
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new SubRuleAction(this);
        }

    }

    /// <summary>
	/// Represents the collection of rule actions.
	/// </summary>
	[XmlRoot("RuleActions")]
	[Serializable]
	public class SubRuleActionCollection : BaseIdInt32Collection<SubRuleAction>
	{
		/// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return SchedulingUtilities.CloneBaseCollection<SubRuleActionCollection, SubRuleAction, int>(
				this
			);
		}
	}
}
