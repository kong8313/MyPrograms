using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confirmit.CATI.Common;
using System.Xml.Serialization;
using Confirmit.CATI.Core.ScheduleDom.Resources;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
    /// <summary>
    /// Represents custom parameter. custom parameter contains identifier, name and description,
    /// type and value. 
    /// The custom parameter identifier is Int32 identifier. This class does nothing, it is just a container.
    /// </summary>
    [Serializable]
    public class CustomParameter : BaseObject<int>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CustomParameter()
            : base()
        {
        }

        /// <summary>
        /// Protected copying constructor. 
        /// </summary>
        /// <param name="param">Custom parameter to copy.</param>
        protected CustomParameter(CustomParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param", Strings.ItemNullExceptionMessage);
            }

            Id = param.Id;
            Name = param.Name;
            Description = param.Description;
            Type = param.Type;
            Value = param.Value;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance. 
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new CustomParameter(this);
        }

        /// <summary>
        /// Name identifier. If this value is null or empty that means
        /// that object is not proper initialized.
        /// </summary>
        [XmlElement("Name")]
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Description identifier. By default it is empty
        /// </summary>
        [XmlElement( "Description" )]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Type identifier. It is nullable value. If this value is null that means
		/// that object is not proper initialized.
        /// </summary>
        [XmlElement( "Type", IsNullable=true)]
        public SchedulingParameterType? Type
        {
            get;
            set;
        }

        /// <summary>
        /// Value identifier. It is nullable value. If this value is null that means
        /// that object is not proper initialized.
        /// </summary>
        [XmlElement("Value", IsNullable = true)]
        public int? Value
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the collection of custom parameters.
    /// </summary>
    [XmlRoot("CustomParameters")]
    [Serializable]
    public class CustomParameterCollection : BaseIdInt32Collection<CustomParameter>
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance. 
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return SchedulingUtilities.CloneBaseCollection<CustomParameterCollection, CustomParameter, Int32>(
                this
            );
        }
    }
}
