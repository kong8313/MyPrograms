using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace Confirmit.CATI.Core.Schedules2007.BvSchScriptGen
{
    [Serializable]
    public class CustomCodeDescription
    {        
        #region Properties

        public bool IsFilterDescription
        {
            get;
            set;            
        }

        public bool IsCustomCodeDescription
        {
            get;
            set;            
        }

        public Guid? RuleId
        {
            get;
            set;            
        }

        public Guid? SubRuleId
        {
            get;
            set;            
        }

        public int? ActionId
        {
            get;
            set;            
        }

        #endregion

        #region Constructors

        private CustomCodeDescription()
        { 
        
        }
        
        internal CustomCodeDescription(Rule rule, SubRule subRule, SubRuleAction action)
            : this(rule, subRule)
        {            
            ActionId = action.Id;
        }

        internal CustomCodeDescription(Rule rule, SubRule subRule )
        {
            RuleId = rule.Id;
            SubRuleId = subRule.Id;
            IsFilterDescription = true;            
        }

        internal CustomCodeDescription(CustomScript customScript)
        {
            IsCustomCodeDescription = true;
        }

        #endregion

        #region Methods

        internal string Serialize()
        {
            StringBuilder builder = new StringBuilder();
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                NewLineHandling = NewLineHandling.None
            };

            XmlWriter writer = XmlWriter.Create(builder, settings);

            serializer.Serialize(writer, this);

            return builder.ToString();
        }

        public static CustomCodeDescription Deserialize(string serializedObject)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CustomCodeDescription));

            TextReader tr = new StringReader(serializedObject);
            XmlReader xmlReader = XmlReader.Create(tr);

            if (serializer.CanDeserialize(xmlReader))
            {
                return (CustomCodeDescription)serializer.Deserialize(xmlReader);
            }

            return null;
        }

        #endregion        
    }
}
