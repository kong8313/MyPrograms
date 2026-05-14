using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
	[Serializable]
	public class RedialInitialStateData : BaseStateData
	{
	    public string DialNumber { get; set; }
	}
}
