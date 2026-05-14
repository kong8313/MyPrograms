using System.Xml.Serialization;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.ScheduleDom.Script
{
    /// <summary>
    /// Represents action. Action contains identifier, name and parameter definition properties.
    /// Each action may contains 1 or zero parameters. Parameter has type and description.
    /// </summary>
    public class Action
    {
        private string _name = string.Empty;
        private string _parameterTypeName = string.Empty;
        private string _parameterDescription = string.Empty;

        public Action()
        {
            Id = null;
            HasParameter = false;
        }

        /// <summary>
        /// Unique identifier. It is nullable value. If this value is null that means
        /// that object is not initialized.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public int? Id { get; set; }

        /// <summary>
        /// Action name.
        /// </summary>
        [XmlElement]
        public string Name
        {
            get { return _name ?? string.Empty; }
            set { _name = value; }
        }

        /// <summary>
        /// Indicates that action has parameter. Action has no parameter by default.
        /// </summary>
        [XmlElement]
        public bool HasParameter { get; set; }

        /// <summary>
        /// Parameter description.
        /// </summary>
        [XmlElement]
        public string ParameterDescription
        {
            get { return _parameterDescription ?? string.Empty; }
            set { _parameterDescription = value; }
        }

        /// <summary>
        /// .Net type name of action parameter. For example 'System.Int32'.
        /// </summary>
        [XmlElement]
        public string ParameterTypeName
        {
            get { return _parameterTypeName ?? string.Empty; }
            set { _parameterTypeName = value; }
        }

        [XmlElement(IsNullable = true)]
        public SchedulingParameterType? ParameterType { get; set; }
    }
}
