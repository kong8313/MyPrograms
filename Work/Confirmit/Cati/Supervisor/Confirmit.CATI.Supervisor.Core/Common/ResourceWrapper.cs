using System;
using System.Reflection;
using System.Resources;
using System.IO;

namespace Confirmit.CATI.Supervisor.Core.Common
{
	public class ResourceWrapper : IResourceWrapper
	{
		private ResourceManager m_RM = new ResourceManager(
            "Confirmit.CATI.Supervisor.Resources.Strings",
			Assembly.Load( "Confirmit.CATI.Supervisor.Resources" ) );
		private static ResourceWrapper m_Instance = new ResourceWrapper();

		//---------------------------------------------------------------------------
		private ResourceWrapper()
		{
		}

		//---------------------------------------------------------------------------
		public static ResourceWrapper Instance
		{
			get{ return( m_Instance ); }
		}

		//---------------------------------------------------------------------------
		public string this[ string sResItemName ]
		{
			get{ return( GetString( sResItemName ) ); }
		}
		
		//---------------------------------------------------------------------------
		public string GetString( string sResItemName )
		{
			string sRes = m_RM.GetString( sResItemName );
			
			if ( ( sRes == null ) || ( sRes == "" ) )
			{			    
				return sResItemName;
			}
			else
			{
				return sRes;
			}
		}		
	}
}
