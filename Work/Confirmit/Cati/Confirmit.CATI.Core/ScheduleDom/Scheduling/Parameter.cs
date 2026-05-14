using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
    [Serializable]
    public class Parameter
    {
        #region Constructors

        public Parameter()
        {
        }

        public Parameter(Parameter copy)
        {
            this.Type = copy.Type;
            this.Value = copy.Value;
        }

        #endregion

        #region Properties

        [XmlText]
        public string Value
        {
            get;
            set;
        }


        [XmlIgnore]
        public string Constant
        {
            get
            {
                if (Type == ParamType.Constant)
                    return Value;
                return null;
            }
            set
            {
                Value = value;
                Type = ParamType.Constant;
            }
        }

        [XmlIgnore]
        public int? ParameterID
        {
            get
            {
                if (Type == ParamType.Parameter)
                    return Int32.Parse(Value);
                return null;
            }
            set
            {
                Value = value.ToString();
                Type = ParamType.Parameter;
            }
        }

        [XmlAttribute("Type")]
        public ParamType Type
        {
            get;
            set;
        }

        #endregion

        #region Enums

        public enum ParamType
        {
            [XmlEnum("Constant")]
            Constant = 0,
            [XmlEnum("Parameter")]
            Parameter = 1
        }

        #endregion

        #region Methods

        public override string  ToString()
        {
            return String.Format("(Type={0}, Value={1})", Type, Value );
        }

        #endregion

    }
}
