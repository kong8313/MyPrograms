using System;
using System.Xml.Serialization;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using Confirmit.CATI.Core.ScheduleDom.Resources;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents shift type. Shift type contains identifier, name and color.
	/// Color is associated with shift type for representation of shift types 
	/// in GUI.
	/// </summary>
	[Serializable]
    public class ShiftType : BaseObject<int>
	{
	    private const int m_exclusionTypeId = 0;

		private string m_name = string.Empty;
		private Color? m_color = null;

	    /// <summary>
		/// Default constructor.
		/// </summary>
		public ShiftType()
			: base()
		{
		}

		/// <summary>
		/// Protected copying constructor. 
		/// </summary>
		/// <param name="shiftType">Object to copy.</param>
		protected ShiftType( ShiftType shiftType )
		{
			if(shiftType == null)
			{
				throw new ArgumentNullException( "shiftType", Strings.ItemNullExceptionMessage );
			}

			Id = shiftType.Id;
			Name = shiftType.Name;
			Color = shiftType.Color;
		}

	    /// <summary>
		/// Shift type name.
		/// </summary>
		[XmlElement]
		public string Name
		{
			get { return m_name ?? String.Empty; }
			set { m_name = value; }
		}

		/// <summary>
		/// Shift type color.
		/// </summary>
		[XmlIgnore]
		public Color? Color
		{
			get { return m_color; }
			set { m_color = value; }
		}

		/// <summary>
		/// It is surrogate property for serialization of Color property.
		/// Standard XmlSerializer does not serialize Color structure.
		/// See <![CDATA[http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=636862&SiteId=1]]>
		/// for description.
		/// </summary>
		[XmlElement]
		public int? ColorInt
		{
			get
			{
				int? result = null;
				if(m_color.HasValue)
				{
					result = m_color.Value.ToArgb();
				}

				return result;
			}
			set
			{
				if(value.HasValue)
				{
					m_color = System.Drawing.Color.FromArgb( value.Value );
				}
				else
				{
					m_color = null;
				}
			}
		}

		/// <summary>
		/// Returns true, if this shift type is exclusion shift type.
		/// </summary>
		[XmlIgnore]
		public bool IsExclusionType
		{
			get { return (Id.HasValue && Id.Value == m_exclusionTypeId); }
		}

	    /// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new ShiftType( this );
		}

	    /// <summary>
		/// Converts this shift type to exclusion shift type.
		/// </summary>
		public void ConvertToExclusionShiftType()
		{
			Id = m_exclusionTypeId;
		}
	}
}
