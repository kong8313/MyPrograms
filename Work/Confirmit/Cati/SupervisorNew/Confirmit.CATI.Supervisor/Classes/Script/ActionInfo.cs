using Confirmit.CATI.Core.ScheduleDom.Scheduling;

namespace Confirmit.CATI.Supervisor.Classes.Script
{
    public class ActionInfo
    {
        #region Properties

        /// <summary>
        /// Unique identifier. It is nullable value. If this value is null that means
        /// that object is not initialized.
        /// </summary>
        [RowRead( "Id" )]
        public int? Id
        {
            get;
            set;
        }

        /// <summary>
        /// Action identifier. It is nullable value. If this value is null that means
        /// that object is not proper initialized.
        /// </summary>
        [RowRead("ActionId")]
        public int ActionId
        {
            get; set;
        }

        /// <summary>
        /// Filter of rule action. It is empty by default.
        /// </summary>
        [RowRead( "Filter" )]
        public string Filter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if the rule action is enabled. It is true by default.
        /// </summary>
        [RowRead( "Enabled" )]
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// String representation of parameter value of action. It is empty by default.
        /// </summary>
        [RowRead( "Parameter" )]
        public string ParameterValue
        {
            get;
            set;
        }

        /// <summary>
        /// Property shows if parameter value is custom scheduling parameter id.
        /// </summary>
        [RowRead("IsSchedulingParameter")]
        public bool IsSchedulingParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if the action filter is enabled. It is false by default.
        /// </summary>
        [RowRead( "FilterEnabled" )]
        public bool FilterEnabled
        {
            get;
            set;
        }

        #endregion

        #region Methods
        public void FillAction( SubRuleAction subRuleAction )
        {
            subRuleAction.ActionId = ActionId;
            subRuleAction.Enabled = Enabled;
            subRuleAction.Filter = Filter;
            subRuleAction.FilterEnabled = FilterEnabled;
            subRuleAction.Parameter.Value = ParameterValue;
            subRuleAction.Parameter.Type = IsSchedulingParameter
                ? Parameter.ParamType.Parameter
                : Parameter.ParamType.Constant;
        }
        #endregion
    }
}
