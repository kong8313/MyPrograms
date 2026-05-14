using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.ServerControls;
using ConfirmitDialerInterface;
using Infragistics.Web.UI.GridControls;
using Newtonsoft.Json;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class ExternalTransferPage : BaseForm
    {
        private IExternalTransferTelephoneNumberRepository _externalTransferTelephoneNumberRepository =
            ServiceLocator.Resolve<IExternalTransferTelephoneNumberRepository>();

        private IExternalTransferTelephoneNumberService _externalTransferTelephoneNumberService =
            ServiceLocator.Resolve<IExternalTransferTelephoneNumberService>();

        public override string TopTitle => Strings.ExternalTransfer;

        protected void Page_Load(object sender, EventArgs e)
        {
            grid.HintText = Strings.ExternalTransferHint;
            grid.GetPage += GetPage;
            grid.InitializeRow += Grid_InitializeRow;

            FillHiddenSearchFilter();
        }

        private object GetPage(out int totalCount)
        {
            var numbers = _externalTransferTelephoneNumberRepository.GetAll();

            return BaseMethods.GetPage(numbers, grid.PageArguments, out totalCount);
        }

        /// <summary>
        /// Used to fill row's cells by some values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            var entry = (BvSpTransfer_GetExternalListEntity)e.Row.DataItem;
            var hidden = entry.Hidden.GetValueOrDefault();
            var hiddenItem = e.Row.Items.FindItemByKey("Hidden");

            hiddenItem.Text = hidden ? Strings.Yes : Strings.No;
        }

        protected void Delete(object sender, EventArgs e)
        {
            _externalTransferTelephoneNumberService.DeleteNumbers(grid.SelectedKeysInt.ToArray());
            
            grid.BindData();
        }

        private void FillHiddenSearchFilter()
        {
            if (!(grid.Columns.FromKey("Hidden") is GeneralGridColumn hiddenColumn))
            {
                return;
            }

            hiddenColumn.Items.Add(new ListItem(Strings.Yes, "1"));
            hiddenColumn.Items.Add(new ListItem(Strings.No, "0"));
        }
    }
}
