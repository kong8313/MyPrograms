using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Survey.Quota;

namespace Confirmit.CATI.Core.DAL.Generated.Entity.Table
{
    public partial class BvSurveyQuotaCellEntity
    {
        private QuotaCellData _data;

        public QuotaCellData Data
        {
            get
            {
                if (_data == null && !string.IsNullOrWhiteSpace(m_xmldata))
                {
                    _data = XmlSerialization.Deserialize<QuotaCellData>(m_xmldata);
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
                _data = XmlSerialization.Deserialize<QuotaCellData>(m_xmldata);
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
