using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.ScheduleDom.Script;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Script
{
    public partial class ScriptRuleProperties : BaseForm
    {
        private readonly IScheduleService _scheduleService;

        private readonly ISchedulingObjectValidator _validator;

        private IActionCollection _actionCollection;

        public ScriptRuleProperties()
        {
            _validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
        }

        [StoreInViewState]
        protected Guid? RuleId;

        [StoreInViewState]
        protected int? ScheduleId;

        protected bool IsNew
        {
            get { return !RuleId.HasValue; }
        }

        public Schedule WorkingSchedule
        {
            get { return (Schedule)Session[$"WorkingSchedule_{ScheduleId}"]; }
        }

        protected RuleCollection Rules
        {
            get { return WorkingSchedule.Rules; }
        }
        
        protected ShiftTypeCollection ShiftTypeCollection
        {
            get { return WorkingSchedule.ShiftTypes; }
        }
        
        protected IActionCollection ActionCollection
        {
            get
            {
                if (_actionCollection == null)
                {
                    _actionCollection = _scheduleService.GetActions();
                }

                return _actionCollection;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["ID"] != null)
            {
                ScheduleId = int.Parse(Request["ID"]);
            }

            if (IsPostBack == false)
            {
                if (Request["RuleId"] != null)
                {
                    RuleId = Guid.Parse(Request["RuleId"]);
                }

                if (!IsNew)
                {
                    BindData();
                    SetTitle();
                }
            }            

            dialog.OKButton.Text = IsNew ? "Add" : "Save";
        }

        private void SetTitle()
        {
            string number = WorkingSchedule.GetNumberByGuid(RuleId.Value);
            SetOverlayTitle(String.Format("Edit '{0}'", string.Format(GetResString("RuleNumber"), number)));
        }

        private void BindData()
        {
            var rule = Rules.GetItemById(RuleId.Value);

            tbxDescripton.Text = rule.Description;
            cbSampleUpdate.Checked = rule.SampleUpdate;
        }        

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                Rule rule;
                ErrorCollection errors;

                if (IsNew == false)
                {
                    rule = (Rule)Rules.GetItemById(RuleId.Value).Clone();
                }
                else
                {
                    rule = new Rule { Id = Rules.GetNewId() };
                }

                rule.Description = tbxDescripton.Text;
                rule.SampleUpdate = cbSampleUpdate.Checked;

                if (_validator.Validate(rule, out errors))
                {
                    if (IsNew)
                    {
                        Rules.Add(rule);
                    }
                    else
                    {
                        int index = Rules.IndexOf(Rules.GetItemById(rule.Id.Value));
                        Rules[index] = rule;
                    }

                    if (rule.SampleUpdate)
                    {
                        CleanSampleUpdateFlagOnOtherRules(rule.Id);
                    }

                    CloseOverlay(true);
                }
                else
                {
                    ShowClientMessage(errors[0].Message);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void CleanSampleUpdateFlagOnOtherRules(Guid? id)
        {
            foreach (var rule in Rules)
            {
                if (rule.Id != id)
                {
                    rule.SampleUpdate = false;
                }
            }
        }
    }
}