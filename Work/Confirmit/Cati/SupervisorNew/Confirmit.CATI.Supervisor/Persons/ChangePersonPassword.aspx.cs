using System;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class ChangePersonPassword : BaseForm
    {
        [StoreInViewState] 
        protected int PersonId;
                
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PersonId =  int.Parse(Request.Params["PersonId"]);                
            }            
        }                    
        
        protected void ChangePassword(object sender, EventArgs e)
        {
            try
            {
                if (!ValidatePassword(tbxChange.Text, tbxConfirmChange.Text))
                {
                    return;
                }

                if (TaskRepository.GetByPerson(PersonId) != null)
                {
                    AddUserMessage(Strings.CouldNotChangePasswordWhileInterviewerIsLoggedIn);
                    return;
                }

                using (var trabsaction = new DatabaseTransactionScope("Supervisor.ChangeInterPass", DeadlockPriority.Supervisor))
                {
                    var evt = new ChangeInterviewerPasswordEvent(PersonId, PersonRepository.GetById(PersonId).Name);
                    
                    ServiceLocator.Resolve<IPasswordSaver>().Save(PersonId, tbxChange.Text.Trim());

                    evt.Finish();
                    trabsaction.Commit();
                }

                CloseOverlay();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private bool ValidatePassword(string password, string confirm)
        {
            if (String.IsNullOrEmpty(password))
            {
                AddUserMessage(Strings.Err_PasswordIsEmpty);
                return false;
            }

            if (password != confirm)
            {
                AddUserMessage(Strings.Err_PasswordsDontMatch);
                return false;
            }

            return true;
        }

    }
}
