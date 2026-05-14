using System;
using System.Collections;

namespace Confirmit.CATI.Supervisor.Classes
{
	/// <summary>
	/// Summary description for GridListHolder.
	/// </summary>
	[Serializable]
	public class GridListHolder
	{
		private string m_id;
		private ArrayList m_list = null;
		private ArrayList m_addedItems = new ArrayList();
		private ArrayList m_deletedItems = new ArrayList();

		public GridListHolder()
		{
			m_id = Guid.NewGuid().ToString();
		}

		public string ID
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		public ArrayList List
		{
			get
			{
				return m_list;
			}
			set
			{
				m_list = value;
			}
		}

		
		public ArrayList GetPage( int number, int page_size )
		{
			ArrayList page_list = new ArrayList();
			for( int i=(number-1)*page_size; i<number*page_size && i<m_list.Count; i++ )
				page_list.Add( m_list[i] );
			return page_list;
		}

		public bool AddItem( object item )
		{
			if( m_list.IndexOf(item) >= 0 )
				return false;
			m_list.Add( item );
			m_addedItems.Add( item );
			m_deletedItems.Remove( item );
			return true;
		}

		public void RemoveItem( object item )
		{
			m_list.Remove( item );
			m_deletedItems.Add( item );
			m_addedItems.Remove( item );
		}

		public ArrayList AddedItems
		{
			get
			{
				return m_addedItems;
			}
		}

		public ArrayList DeletedItems
		{
			get
			{
				return m_deletedItems;
			}
		}

		public void Clear()
		{
			m_list = null;
			m_addedItems.Clear();
			m_deletedItems.Clear();
		}
	}
}
