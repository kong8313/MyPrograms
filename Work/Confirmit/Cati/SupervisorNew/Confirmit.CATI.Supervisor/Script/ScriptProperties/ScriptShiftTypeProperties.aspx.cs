using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Script
{
    public partial class ScriptShiftTypeProperties : BaseForm
    {
        public ScriptShiftTypeProperties()
        {
            _validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
        }

        [StoreInViewState]
        protected int? ShiftTypeId;

        [StoreInViewState] 
        protected bool IsExclusion;

        [StoreInViewState]
        protected int ScheduleId;

        private readonly ISchedulingObjectValidator _validator;

        protected bool IsNew
        {
            get { return !ShiftTypeId.HasValue; }
        }

        public Schedule WorkingSchedule
        {
            get { return (Schedule)Session[$"WorkingSchedule_{ScheduleId}"]; }
        }

        protected ShiftTypeCollection ShiftTypeCollection
        {
            get
            {
                return WorkingSchedule.ShiftTypes;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {            
            BindColors();
            ddlColor.Attributes["onChange"] = String.Format("ChangeColor('{0}', '{1}')", ddlColor.ClientID, divColor.ClientID);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["ID"] != null)
            {
                ScheduleId = int.Parse(Request["ID"]);
            }

            if (IsPostBack == false)
            {
                if (Request["ShiftTypeId"] != null)
                {
                    ShiftTypeId = Int32.Parse(Request["ShiftTypeId"]);
                }

                if (Request["IsExclusion"] != null)
                {
                    IsExclusion = bool.Parse(Request["IsExclusion"]);
                }

                if (IsNew == false)
                {
                    BindData();
                }
            }

            dialog.OKButton.Text = IsNew ? "Add" : "Save";
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterStartupScript(ddlColor.Attributes["onChange"]);
        }

        private void BindData()
        {
            var shiftType = ShiftTypeCollection.GetItemById(ShiftTypeId.Value);

            tbShiftTypeName.Text = shiftType.Name;

            if (shiftType.Color != null)
            {
                var colorName = GetColorName(shiftType.Color.Value);
                ddlColor.SelectedValue = colorName;
            }
        }

        private static String GetColorName(Color color)
        {
            var colors =
               typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public)
                   .ToDictionary(p => p.Name, p => (Color)p.GetValue(null, null));
            return colors.FirstOrDefault(c => c.Value.A == color.A && c.Value.R == color.R && c.Value.G == color.G && c.Value.B == color.B).Key;
        }

        private void BindColors()
        {
            Array colors = Enum.GetValues(typeof(KnownColor));

            for (int i = 0; i < colors.Length; i++)
            {
                Color color = Color.FromKnownColor((KnownColor)colors.GetValue(i));
                if (!color.IsSystemColor)

                    /* exclude Aqua color because it duplicate Cyan color
                       exclude Magenta color because it duplicate Fuchsia color
                       exclude LightGray color because IE knows only LightGrey */

                    if (!(color.Name == "Aqua") && !(color.Name == "Magenta") && !(color.Name == "LightGray"))
                        ddlColor.Items.Add(colors.GetValue(i).ToString());
            }
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                ShiftType shiftType;
                ErrorCollection errors;
                ErrorCollection collectionBasedErrors = null;

                if (IsNew == false)
                {
                    shiftType = (ShiftType)ShiftTypeCollection.GetItemById(ShiftTypeId.Value).Clone();
                }
                else
                {
                    shiftType = new ShiftType();

                    if (IsExclusion)
                    {
                        shiftType.ConvertToExclusionShiftType();
                    }
                    else
                    {
                        shiftType.Id = ShiftTypeCollection.GetNewId();
                    }
                }

                shiftType.Name = tbShiftTypeName.Text;
                shiftType.Color = ColorTranslator.FromHtml(ddlColor.SelectedValue);

                if (_validator.Validate(shiftType, out errors) && _validator.ValidateWithCollection(ShiftTypeCollection,shiftType, out collectionBasedErrors))
                {
                    if (IsNew)
                    {
                        ShiftTypeCollection.Add(shiftType);
                    }
                    else
                    {
                        int index = ShiftTypeCollection.IndexOf(ShiftTypeCollection.GetItemById(shiftType.Id.Value));
                        ShiftTypeCollection[index] = shiftType;
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