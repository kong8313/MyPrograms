using System.Collections;
using System.Web.UI;

using Infragistics.Web.UI.NavigationControls;

namespace Confirmit.CATI.Supervisor.ServerControls
{
	/// <summary>
	/// Class of custom grid's band, inherits UltraGridBand,
	/// supports MenuItems collection for a single band.
	/// </summary>
	public class GridBand: Infragistics.Web.UI.GridControls.Band
	{
        private readonly DataMenuItemCollection _mDataMenuItems = new DataMenuItemCollection();

		/// <summary>
		/// Items of the band's context menu.
		/// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public DataMenuItemCollection DataMenuItems
		{
			get
			{
				return _mDataMenuItems;
			}
		}
	}
}
