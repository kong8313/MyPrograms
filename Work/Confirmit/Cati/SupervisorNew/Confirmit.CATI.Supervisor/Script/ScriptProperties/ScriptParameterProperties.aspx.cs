using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Script
{
    public partial class ScriptParameterProperties : BaseForm
    {
        private readonly ISchedulingScriptSettings _schedulingScriptSettings;
        private readonly IScheduleService _scheduleService;

        private readonly ISchedulingObjectValidator _validator;

        public ScriptParameterProperties()
        {
            _validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
            _schedulingScriptSettings = ServiceLocator.Resolve<ISchedulingScriptSettings>();
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
        }

        [StoreInViewState]
        protected int? ParameterId;

        [StoreInViewState]
        protected int? ScheduleId;

        protected bool IsNew
        {
            get { return !ParameterId.HasValue; }
        }

        public Schedule WorkingSchedule
        {
            get { return (Schedule)Session[$"WorkingSchedule_{ScheduleId}"]; }
        }

        /// <summary>
        /// Contains all parameters
        /// </summary>
        public CustomParameterCollection ParametersCollection
        {
            get
            {
                return WorkingSchedule.CustomParameters;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["ID"] != null)
            {
                ScheduleId = int.Parse(Request["ID"]);
            }

            if (!IsPostBack)
            {
                ddlType.Items.Clear();
                foreach (SchedulingParameterType type in Enum.GetValues(typeof(SchedulingParameterType)))
                {
                    ddlType.Items.Add(new ListItem(StringHelper.GetStringForEnum(type), ((int)type).ToString(CultureInfo.InvariantCulture)));
                }

                if (Request["ParameterId"] != null)
                {
                    ParameterId = Int32.Parse(Request["ParameterId"]);
                }

                if (!IsNew)
                {
                    BindData();
                }
            }

            dialog.OKButton.Text = IsNew ? "Add" : "Save";
        }

        private void BindData()
        {
            var parameter = ParametersCollection.GetItemById(ParameterId.Value);

            tbParamName.Text = parameter.Name;
            tbDescription.Text = parameter.Description;
            ddlType.SelectedValue = ((int)parameter.Type.Value).ToString();
            neDefaultValue.Value = parameter.Value.Value;
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                CustomParameter param;
                ErrorCollection errors;
                ErrorCollection collectionBasedErrors = null;

                if (IsNew == false)
                {
                    param = (CustomParameter)ParametersCollection.GetItemById(ParameterId.Value).Clone();
                }
                else
                {
                    param = new CustomParameter { Id = ParametersCollection.GetNewId() };
                }

                param.Name = tbParamName.Text;
                param.Description = tbDescription.Text;
                param.Type = (SchedulingParameterType?)ddlType.SelectedIndex;
                param.Value = neDefaultValue.ValueInt;

                if (WorkingSchedule.CustomParameters.Count == _schedulingScriptSettings.MaxParameters && IsNew)
                {
                    throw new UserMessageException(String.Format(Strings.YouCannotCreateMoreThanNParameters, _schedulingScriptSettings.MaxParameters));
                }

                string reason;
                if (!_scheduleService.CheckParamValue(WorkingSchedule, 0, param.Type.Value, param.Value.Value, out reason))
                {
                    throw new UserMessageException(String.IsNullOrEmpty(reason) ? Strings.InvalidParameterValue : reason);
                }

                if (_validator.Validate(param, out errors) && _validator.ValidateWithCollection(ParametersCollection,param, out collectionBasedErrors))
                {
                    if (IsNew)
                    {
                        ParametersCollection.Add(param);
                    }
                    else
                    {
                        int index = ParametersCollection.IndexOf(ParametersCollection.GetItemById(param.Id.Value));
                        ParametersCollection[index] = param;
                    }

                    CloseOverlay(true);
                }
                else
                {
                    //notify user about validation errors
                    if (collectionBasedErrors != null)
                    {
                        errors.AddRange(collectionBasedErrors);    
                    }

                    ShowClientMessage(errors[0].Message);
                }            
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}