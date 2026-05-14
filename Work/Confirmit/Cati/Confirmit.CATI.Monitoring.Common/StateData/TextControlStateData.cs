using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
	/// <summary>
	/// Represents state data of text control. Contains single field - control text.
	/// </summary>
	[Serializable]
	public class TextControlStateData : BaseStateData
	{
		#region Constructors

		/// <summary>
		/// Initializes new instance of TextControlStateData class.
		/// </summary>
		public TextControlStateData()
			: base()
		{
		}

		/// <summary>
		/// Initializes new instance of TextControlStateData class and fills it with given data.
		/// </summary>
		/// <param name="controlName">Control name.</param>
		/// <param name="text">Control text.</param>
		public TextControlStateData(string controlName, string text)
			: base(controlName)
		{
			Text = text;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Control text.
		/// </summary>
		public string Text
		{
			get;
			set;
		}

		#endregion
	}
}
