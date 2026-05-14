using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.BlackList;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class BlacklistNumberProperties : BaseForm
    {
        [StoreInViewState]
        protected string TelephoneNumber;

        private readonly IBlackListService _blackListService = ServiceLocator.Resolve<IBlackListService>();
        private readonly ITelephoneBlacklistRepository _telephoneBlacklistRepository= ServiceLocator.Resolve<ITelephoneBlacklistRepository>();
        
        protected bool IsNew => string.IsNullOrEmpty(TelephoneNumber);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TelephoneNumber = Request["TelephoneNumber"];

                if (!IsNew)
                {
                    tbTelephoneNumber.Text = TelephoneNumber;
                    var entity = _telephoneBlacklistRepository.GetByNumber(TelephoneNumber);
                    tbComment.Text = entity.Comment;
                }
            }

            dialog.OKButton.Text = IsNew ? "Add" : "Save";
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                var entity = new BvTelephoneBlacklistEntity
                {
                    TelephoneNumber = tbTelephoneNumber.Text.Trim(),
                    Comment = tbComment.Text.Trim()
                };
                
                if (IsNew)
                {
                    
                    _blackListService.AddNumber(entity);
                }
                else
                {
                    _blackListService.UpdateNumber(TelephoneNumber, entity);
                }

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}