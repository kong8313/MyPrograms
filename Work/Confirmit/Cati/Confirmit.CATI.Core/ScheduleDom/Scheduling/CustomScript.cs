using System;
using System.Xml.Serialization;
using Confirmit.CATI.Core.ScheduleDom.Resources;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents custom script. Custom script has language name and body.
	/// By default the language of script is JScript.Net.
	/// </summary>
	[Serializable]
	public class CustomScript : BaseObject<int>
	{
	    private string m_languageName = "JScript.Net";
		private string m_body = string.Empty;

	    /// <summary>
		/// Default constructor.
		/// </summary>
		public CustomScript()
			: base()
		{
		}

		/// <summary>
		/// Protected copying constructor. 
		/// </summary>
		/// <param name="obj">Object to copy.</param>
		protected CustomScript( CustomScript obj )
		{
			if(obj == null)
			{
				throw new ArgumentNullException( "obj", Strings.ItemNullExceptionMessage );
			}

			Id = obj.Id;
			LanguageName = obj.LanguageName;
			Body = obj.Body;
		}

	    /// <summary>
		/// Name of the language of the custom script.
		/// </summary>
		[XmlElement]
		public string LanguageName
		{
			get { return (m_languageName ?? string.Empty); }
			set { m_languageName = value; }
		}

		/// <summary>
		/// Body of the custom script.
		/// </summary>
		[XmlElement]
		public string Body
		{
			get { return m_body; }
			set { m_body = SchedulingUtilities.ConvertForXml( value ); }
		}

	    /// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new CustomScript( this );
		}
	}
}
