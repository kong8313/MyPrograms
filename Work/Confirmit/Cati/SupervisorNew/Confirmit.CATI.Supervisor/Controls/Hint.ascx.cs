using System;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class Hint : BaseWUC
    {
        /// <summary>
        /// Hint text
        /// </summary>
        public string Text
        {
            get
            {
                return lblHint.Text;
            }
            set
            {
                lblHint.Text = value;
            }
        }

        [StoreInViewState] 
        public HintType HintType;

        public string HintClientId => lblHint.ClientID;

        public string CssClass
        {
            get => ViewState["CssClass"]?.ToString();
            set => ViewState["CssClass"] = value;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            switch (HintType)
            {
                case HintType.Info:
                    break;
                case HintType.Warning:
                    CssClass = "attention--warning";
                    attentionIcon.ImageName = "warning";
                    break;
                case HintType.Success:
                    CssClass = "attention--success";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum HintType    
    {
        Info = 0,
        Warning,
        Success
    }
}