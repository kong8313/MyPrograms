using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Supervisor.Classes;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Supervisor.Resources.Controls
{
    /// <summary>
    /// Control represents a set of dialer parameter controls.
    /// </summary>
    public partial class DialerParameters : BaseWUC
    {
        private IEnumerable<DialerParameter> m_ParametersCollection;

        /// <summary>
        /// Source parameters collection to show in the control.
        /// </summary>
        public IEnumerable<DialerParameter> ParametersCollection
        {
            get
            {
                return GetDialerParameters();
            }
            set
            {
                m_ParametersCollection = value;
            }
        }

        /// <summary>
        /// Dynamically created dialer parameter controls collection.
        /// </summary>
        private List<DialerParameterControl> m_ControlsCollection;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_ControlsCollection = new List<DialerParameterControl>();
            foreach (var parameter in m_ParametersCollection)
            {
                if (parameter == null)
                    return;
                var parameterControl = (DialerParameterControl)Page.LoadControl(@"~/Resources/Controls/DialerParameterControl.ascx");
                parameterControl.SourceParameter = parameter;
                divParameters.Controls.Add(parameterControl);
                m_ControlsCollection.Add(parameterControl);
            }

            this.DataBinding += this.DialerParameters_DataBinding;
        }

        protected void DialerParameters_DataBinding(object sender, EventArgs e)
        {
            for (int i = 0; i < this.m_ControlsCollection.Count; i++)
            {
                var control = this.m_ControlsCollection[i];
                control.SourceParameter = m_ParametersCollection.ElementAt(i);
            }
        }

        /// <summary>
        /// Gets collection of dialer parameters from the control.
        /// </summary>
        /// <returns></returns>
        public List<DialerParameter> GetDialerParameters()
        {
            return m_ControlsCollection.Select(control => control.GetDialerParameter()).ToList();
        }

        /// <summary>
        /// Makes control to display error message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        public void SetError(string id, string description)
        {            
            var control = m_ControlsCollection.Single(x => x.GetDialerParameter().Id == id);

            if (String.IsNullOrEmpty(control.ErrorMessage))
            {
                control.ErrorMessage = description;
            }
        }

        /// <summary>
        /// Validates parameters
        /// </summary>
        /// <returns>True if all parameters are correct otherwise false</returns>
        internal bool ValidateParametersAndSetErrorMessages()
        {
            bool result = true;

            foreach (var dialerParameterControl in m_ControlsCollection)
            {
                result =  dialerParameterControl.ValidateAndSetErrorMessage() && result;
            }

            return result;
        }
    }
}