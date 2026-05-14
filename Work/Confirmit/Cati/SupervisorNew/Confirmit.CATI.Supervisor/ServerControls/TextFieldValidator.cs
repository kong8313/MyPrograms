using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Universal validator conrol for text field.
    /// </summary>
    public class TextFieldValidator : WebControl
    {
        #region Members

        private const string FieldRequiredValidatorPostfix = "required";
        private const string RegularExpressionValidatorPostfix = "valid";
        private readonly IInputParameterValidator _inputParameterValidator = ServiceLocator.Resolve<IInputParameterValidator>();

        #endregion

        #region Properties

        /// <summary>
        ///  Gets/sets flag indicated is field required or not
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return (bool)(ViewState["IsRequired"] ?? false);
            }
            set
            {
                this.ViewState["IsRequired"] = value;
            }
        }

        /// <summary>
        ///  Gets/sets flag indicated is client validation is enabled or not
        /// </summary>
        public bool IsClientScriptEnabled
        {
            get
            {
                return (bool)(ViewState["IsClientScriptEnabled"] ?? true);
            }
            set
            {
                this.ViewState["IsClientScriptEnabled"] = value;
            }
        }

        /// <summary>
        ///  Control to validate
        /// </summary>
        public string ControlToValidate
        {
            get
            {
                return (string)(ViewState["ControlToValidate"] ?? String.Empty);
            }
            set
            {
                this.ViewState["ControlToValidate"] = value;
            }
        }

        /// <summary>
        ///  Valid input regex expression
        /// </summary>
        public string ValidInputExpression
        {
            get
            {
                return (string)(ViewState["ValidInputExpression"] ?? String.Empty);
            }
            set
            {
                this.ViewState["ValidInputExpression"] = value;
            }
        }

        /// <summary>
        ///  Gets/sets message that will be shown for user if empty value has been provided
        /// </summary>
        public string FieldRequredErrorMessage
        {
            get
            {
                return (string)(ViewState["FieldRequredErrorMessage"] ?? String.Empty);
            }
            set
            {
                this.ViewState["FieldRequredErrorMessage"] = value;
            }
        }

        /// <summary>
        ///  Gets/sets message that will be shown for user if invalid value has been provided
        /// </summary>
        public string ValidationErrorMessage
        {
            get
            {
                return (string)(ViewState["ValidationErrorMessage"] ?? String.Empty);
            }
            set
            {
                this.ViewState["ValidationErrorMessage"] = value;
            }
        }

        /// <summary>
        ///  Valid input regex expression
        /// </summary>
        public string Text
        {
            get
            {
                return (string)(ViewState["Text"] ?? String.Empty);
            }
            set
            {
                this.ViewState["Text"] = value;
            }
        }

        #endregion

        #region Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (IsRequired)
            {
                var requiredFieldValidator = new RequiredFieldValidator()
                {
                    ID = this.ID + FieldRequiredValidatorPostfix,
                    ErrorMessage = ResourceWrapper.Instance.GetString(FieldRequredErrorMessage),
                    ControlToValidate = this.ControlToValidate,
                    Text = this.Text,
                    Display = ValidatorDisplay.Dynamic,
                    SetFocusOnError = true,
                    EnableClientScript = IsClientScriptEnabled,
                };

                Controls.Add(requiredFieldValidator);
            }

            var rxv = new RegularExpressionValidator
            {
                ID = this.ID + RegularExpressionValidatorPostfix,
                ValidationExpression = (this.ValidInputExpression != String.Empty)
                                                         ? this.ValidInputExpression
                                                         : _inputParameterValidator.ValidStringMask,
                ErrorMessage = ResourceWrapper.Instance.GetString(ValidationErrorMessage),
                ControlToValidate = this.ControlToValidate,
                Text = this.Text,
                Display = ValidatorDisplay.Dynamic,
                SetFocusOnError = true,
                EnableClientScript = IsClientScriptEnabled
            };
            Controls.Add(rxv);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            RenderChildren(writer);
        }

        #endregion
    }
}