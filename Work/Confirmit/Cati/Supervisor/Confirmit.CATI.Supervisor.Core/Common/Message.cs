using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace Confirmit.CATI.Supervisor.Core.Common
{	
	/// <summary>
	/// Enum describes available message types.
	/// </summary>
	public enum MessageTypeEnum
	{
		Error = 0,
		Warning = 1,
		Info = 2
	}
	/// <summary>
	/// Class describes custom application message.
	/// </summary>
	public class Message
	{
		private MessageTypeEnum m_Type = MessageTypeEnum.Error;
		private string m_Description = null;
		private string m_Details = null;
		/// <summary>
		/// Gets or sets message type
		/// </summary>
		public MessageTypeEnum Type
		{
			get { return m_Type; }
			set { m_Type = value; }
		}
		/// <summary>
		/// Gets or sets message description
		/// </summary>
		public string Description
		{
			get { return m_Description; }
			set { m_Description = value; }
		}
		/// <summary>
		/// Gets or sets message details
		/// </summary>
		public string Details
		{
			get { return m_Details; }
			set { m_Details = value; }
		}
		/// <summary>
		/// Default empty constructor.
		/// </summary>
		public Message()
		{
		}
		/// <summary>
		/// Constructor with params - class fields.
		/// </summary>
		/// <param name="type">Message type</param>
		/// <param name="desc">Message text string</param>
		/// <param name="details">Detailed message string</param>
		public Message(MessageTypeEnum type, string desc, string details)
		{
			Type = type;
			Description = desc;
			Details = details;
		}
		/// <summary>
		/// Constructor 'transforms' System.Exception to class object according to existent format rules.
		/// </summary>
		/// <param name="ex"></param>
		public Message(Exception ex)
		{
			Type = MessageTypeEnum.Error;
			Description = ex.Message;
			if (ex.InnerException != null)
			{
				if (!String.IsNullOrEmpty(ex.InnerException.Message))
					Description += "\n" + "Description: " + ex.InnerException.Message;
				if (!String.IsNullOrEmpty(ex.InnerException.Source))
					Description += "\n" + "Source: " + ex.InnerException.Source;
				System.Runtime.InteropServices.COMException comex = ex.InnerException as System.Runtime.InteropServices.COMException;
				if (comex != null)
					Description += "\n" + "Code: " + comex.ErrorCode;
				if (!String.IsNullOrEmpty(ex.InnerException.StackTrace))
					Details += "Stack trace: " + ex.InnerException.StackTrace;
			}
			else
			{
				if (!String.IsNullOrEmpty(ex.Source))
					Details += "\n" + "Source: " + ex.Source;
				System.Runtime.InteropServices.COMException comex = ex as System.Runtime.InteropServices.COMException;
				if (comex != null)
					Details += "\n" + "Code: " + comex.ErrorCode;
				if(!String.IsNullOrEmpty(ex.StackTrace))
					Details = "Stack trace: " + ex.StackTrace;
			}
		}
	}
}
