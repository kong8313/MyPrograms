using System;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class BreakTypeProperties : BaseForm
    {
        private readonly IBreakTypeRepository _breakTypeRepository;

        [StoreInViewState]
        protected int? BreakId;

        protected bool IsNew
        {
            get { return !BreakId.HasValue; }
        }

        public BreakTypeProperties()
        {
            _breakTypeRepository = ServiceLocator.Resolve<IBreakTypeRepository>();
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlType.Items.Clear();
                ddlType.Items.Add(Strings.Paid);
                ddlType.Items.Add(Strings.Unpaid);

                BreakId = null;
                if (Request["Id"] != null)
                {
                    BreakId = Convert.ToInt32(Request["Id"]);
                }
                
                if (!IsNew)
                {
                    var breakTypeEntity = _breakTypeRepository.TryGetById(BreakId.Value);
                    if (breakTypeEntity != null)
                    {
                        tbName.Text = breakTypeEntity.Name;
                        tbDescription.Text = breakTypeEntity.Description;
                        ddlType.Text = breakTypeEntity.IsPaid ? Strings.Paid : Strings.Unpaid;
                        neYellowThreshold.Value = TimeService.ConvertSecToMin(breakTypeEntity.YellowThreshold);
                        neRedThreshold.Value = TimeService.ConvertSecToMin(breakTypeEntity.RedThreshold);
                    }
                    else
                    {
                        dialog.OKButton.Enabled = false;
                        AddUserMessage(string.Format(Strings.BreakIsNotFound, BreakId.Value));
                    }
                }

                dialog.OKButton.Text = IsNew ? Strings.Add : Strings.Save;
            }
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                var name = tbName.Text.Trim();
                var description = tbDescription.Text.Trim();
                int? yellowThreshold = TimeService.ConvertMinToSec(neYellowThreshold.Value);
                int? redThreshold = TimeService.ConvertMinToSec(neRedThreshold.Value);

                if (string.IsNullOrEmpty(name))
                {
                    AddUserMessage(Strings.PleaseFillBreakName);
                    return;
                }

                if (name.Length > 25)
                {
                    AddUserMessage(Strings.BreakNameLengthError);
                    return;
                }

                if (string.IsNullOrWhiteSpace(description))
                {
                    AddUserMessage(Strings.PleaseFillBreakDescription);
                    return;
                }
                
                if (redThreshold.HasValue && yellowThreshold.HasValue && redThreshold <= yellowThreshold)
                {
                    AddUserMessage(Strings.PleaseFillCorrectRedAlert);
                    return;
                }

                var breakTypeEntity = new BvBreakTypeEntity
                {
                    Name = name,
                    Description = description,
                    IsPaid = ddlType.Text == Strings.Paid,
                    YellowThreshold = yellowThreshold,
                    RedThreshold = redThreshold
                };

                if (IsNew)
                {
                    InsertEntity(breakTypeEntity);
                }
                else
                {
                    UpdateEntity(breakTypeEntity);
                }

                CloseOverlay(true);
            }
            catch(UserMessageException)
            {
                AddUserMessage(Strings.DuplicateBreakTypeError);
                return;
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void InsertEntity(BvBreakTypeEntity breakTypeEntity)
        {
            try
            {
                _breakTypeRepository.Insert(breakTypeEntity);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("constraint 'UC_BvBreakType'"))
                {
                    throw new UserMessageException();
                }

                throw;
            }
        }

        private void UpdateEntity(BvBreakTypeEntity breakTypeEntity)
        {
            try
            {
                breakTypeEntity.Id = Convert.ToInt32(BreakId);

                _breakTypeRepository.Update(breakTypeEntity);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("constraint 'UC_BvBreakType'"))
                {
                    throw new UserMessageException();
                }

                throw;
            }
        }
    }
}