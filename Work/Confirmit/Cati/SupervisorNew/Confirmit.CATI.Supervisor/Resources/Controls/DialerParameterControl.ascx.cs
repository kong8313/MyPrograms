using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.DialerSettings;
using Confirmit.CATI.Supervisor.ServerControls;
using DialerCommon.DialerParameters;
using CheckBox = Confirmit.CATI.Supervisor.ServerControls.CheckBox;
using TextBox = Confirmit.CATI.Supervisor.ServerControls.TextBox;

namespace Confirmit.CATI.Supervisor.Resources.Controls
{
    public partial class DialerParameterControl : BaseWUC
    {
        private DialerParameter m_SourceParameter;

        private Control valueControl;

        private static DiallerType DiallerType => SiteService.GetDiallerType();

        /// <summary>
        /// Source parameter for the control.
        /// </summary>
        public DialerParameter SourceParameter
        {
            get => GetDialerParameter();
            set => m_SourceParameter = value;
        }

        /// <summary>
        /// Gets or sets error text for the parameter
        /// </summary>
        public string ErrorMessage
        {
            set => errorMessage.Text = value;
            get => errorMessage.Text;
        }

        /// <summary>
        /// Valid input regex expression
        /// </summary>
        public string ValidInputExpression
        {
            get => (string)(ViewState["ValidInputExpression"] ?? string.Empty);
            set => ViewState["ValidInputExpression"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (m_SourceParameter == null)
                return;

            ParameterName.Text = m_SourceParameter.Name;

            valueControl = CreateParameterValueControl();
            ParameterValue.Controls.Add(valueControl);

            if (m_SourceParameter.Type == typeof(string).FullName)
            {
                ParameterValue.Controls.Add(CreateTextFieldValidationControl(valueControl.ID));
            }

            ibParameterHelp.Title = m_SourceParameter.Name;
            ibParameterHelp.HelpTextId = DialerSettingsParameterHelpManager.GetHelpStringKey(DiallerType, m_SourceParameter.Id);

            if (DiallerType == DiallerType.Generic &&
                DialerSettingsParameterHelpManager.IsSharedParameter(m_SourceParameter.Id) == false)
            {
                if (!string.IsNullOrWhiteSpace(m_SourceParameter.Description))
                {
                    ibParameterHelp.UseSession = true;
                    Session[ibParameterHelp.HelpTextId] = m_SourceParameter.Description;
                }
                else
                {
                    ibParameterHelp.Visible = false;
                }
            } 
        }

        /// <summary>
        /// Creates control to show parameter value.
        /// Checkbox for boolean parameter.
        /// NumericEdit for decimal and integer parameters.
        /// Textbox for others.
        /// </summary>
        /// <returns></returns>
        private Control CreateParameterValueControl()
        {
            if (m_SourceParameter.Type == typeof(Boolean).FullName)
            {
                var checkBox = new CheckBox();
                checkBox.DataBinding += delegate { ((CheckBox)valueControl).Checked = Convert.ToBoolean(m_SourceParameter.Value); };

                return checkBox;
            }

            if (m_SourceParameter.Type == typeof(Decimal).FullName || m_SourceParameter.Type == typeof(Int32).FullName)
            {
                var numericEdit = new NumericEdit
                   {
                       CssClass = "settings-value-numeric",
                       HorizontalAlign = HorizontalAlign.Left
                   };
                numericEdit.DataBinding += delegate { ((NumericEdit)valueControl).Text = Convert.ToString(m_SourceParameter.Value); };
                return numericEdit;
            }

            var textBox = new TextBox
                {
                    ID = "tbValueEditControl"
                };

            textBox.DataBinding += delegate { ((TextBox)valueControl).Text = Convert.ToString(m_SourceParameter.Value); };
            return textBox;
        }

        /// <summary>
        /// Creates validator control to validate text field
        /// </summary>
        /// <param name="controlToValidateId">Id of control that must be validated</param>
        /// <returns>TextFieldValidator control</returns>
        private Control CreateTextFieldValidationControl(string  controlToValidateId)
        {
            var textFieldValidator = new TextFieldValidator
            {
                IsRequired = false,
                Text = "*",
                ControlToValidate = controlToValidateId,
                ValidationErrorMessage = Strings.ErrorIncorrectValue,
                ValidInputExpression = this.ValidInputExpression
            };

            return textFieldValidator;
        }

        /// <summary>
        /// Gets parameter value from the control.
        /// </summary>
        /// <returns></returns>
        private string GetParameterValue()
        {
            if (m_SourceParameter.Type == typeof(Boolean).FullName)
            {
                return ((CheckBox)ParameterValue.Controls[0]).Checked.ToString();
            }

            if (m_SourceParameter.Type == typeof(Decimal).FullName || m_SourceParameter.Type == typeof(Int32).FullName)
            {
                return ((NumericEdit)ParameterValue.Controls[0]).Value.ToString();
            }

            return ((TextBox)ParameterValue.Controls[0]).Text;
        }

        /// <summary>
        /// Gets dialer parameter from the control.
        /// </summary>
        /// <returns></returns>
        internal DialerParameter GetDialerParameter()
        {
            return new DialerParameter
            {
                Id = m_SourceParameter.Id,
                Name = m_SourceParameter.Name,
                Type = m_SourceParameter.Type,
                Value = GetParameterValue()
            };
        }

        /// <summary>
        /// Validates value entered by user
        /// </summary>
        /// <returns>Return true if value is correct otherwise false</returns>
        public bool ValidateAndSetErrorMessage()
        {
            bool result = true;

            if (m_SourceParameter.Type == typeof(Int32).FullName)
            {
                var wne = ((NumericEdit)ParameterValue.Controls[0]);

                if(String.IsNullOrEmpty(wne.Text))
                {
                    result = false;
                    ErrorMessage = Strings.EmptyParameterValue;                    
                }

                else if (m_SourceParameter.Type == typeof(Int32).FullName &&
                    wne.ValueInt != wne.ValueLong)
                {
                    result = false;
                 
                    ErrorMessage = wne.ValueInt > 0
                                       ? string.Format(Strings.ValueIsTooLarge, Int32.MaxValue)
                                       : string.Format(Strings.ValueIsTooSmall, Int32.MinValue);
                }                
            }            

            return result;

        }
    }
}
