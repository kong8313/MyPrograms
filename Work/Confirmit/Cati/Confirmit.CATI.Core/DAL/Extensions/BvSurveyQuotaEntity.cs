using System.Collections.Generic;
using System.Xml.Serialization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Survey.Quota;

namespace Confirmit.CATI.Core.DAL.Generated.Entity.Table
{
    public partial class BvSurveyQuotaEntity
    {
        private QuotaData _data;

        public QuotaData Data
        {
            get
            {
                if (_data == null && !string.IsNullOrWhiteSpace(m_xmldata))
                {
                    _data = XmlSerialization.Deserialize<QuotaData>(m_xmldata);
                }
                return _data;
            }
            set
            {
                _data = value;
                XmlData = XmlSerialization.Serialize(_data);
            }
        }

        partial void OnBeforeGetXmlData()
        {
            if (_data == null && !string.IsNullOrWhiteSpace(m_xmldata))
            {
                _data = XmlSerialization.Deserialize<QuotaData>(m_xmldata);
            }

            if (_data != null)
            {
                m_xmldata = XmlSerialization.Serialize(_data);
            }

        }

        partial void OnBeforeSetXmlData()
        {
            _data = null;
        }

    }
}
