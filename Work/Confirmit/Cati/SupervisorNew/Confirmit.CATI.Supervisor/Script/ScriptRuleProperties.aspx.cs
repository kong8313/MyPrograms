using System;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Script;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Script
{
    public partial class ScriptRuleProperties : BaseForm
    {
        private ActionCollection m_ActionCollection;

        [StoreInViewState]
        protected Guid? RuleId;

        protected bool IsNew
        {
            get { return !RuleId.HasValue; }
        }

        public Schedule WorkingSchedule
        {
            get { return (Schedule)Session["WorkingSchedule"]; }
        }

        protected RuleCollection Rules
        {
            get { return WorkingSchedule.Rules; }
        }
        
        protected ShiftTypeCollection ShiftTypeCollection
        {
            get { return WorkingSchedule.ShiftTypes; }
        }
        
        protected ActionCollection ActionCollection
        {
            get
            {
                if (m_ActionCollection == null)
                {
                    m_ActionCollection = ScheduleService.GetActions();
                }
                return m_ActionCollection;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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

                if (rule.Validate(out errors))
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
    }
}