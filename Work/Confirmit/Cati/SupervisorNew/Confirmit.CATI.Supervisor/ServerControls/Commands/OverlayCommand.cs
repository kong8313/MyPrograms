using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;

namespace Confirmit.CATI.Supervisor.ServerControls.Commands
{

    public class OverlayCommand : BaseOverlayCommand
    {
        private string m_title;
        private string m_idName = string.Empty;
        private DialogMode m_dialogMode = DialogMode.ViewEdit;
        private Dictionary<string, string> m_externalParams = new Dictionary<string, string>();
        private Dictionary<string, string> _dynamicClientParams = new Dictionary<string, string>();

        public OverlayCommand()
        {
            Height = 400;
            Width = 600;
            SelectMode = CommandGridSelectMode.SingleRow;
        }

        public string GetClientFunctionName(string ownerName)
        {
            var name = new StringBuilder();
            name.Append("f_");
            name.Append(ownerName);
            name.Append("_");
            name.Append(Key);
            return name.ToString();
        }

        /// <summary>
        /// Adds a dynamic parameter to the overlay command. The value of the parameter is evaluated each time when the command's script is invoked.
        /// </summary>
        /// <param name="parameterName">The name of the parameter. It will be the name of the request parameter in the overlay.</param>
        /// <param name="script">JavaScript code that returnes the parameter value.</param>
        public void AddDynamicClientParameter(string parameterName, string script)
        {
            _dynamicClientParams.Add(parameterName, script);
        }

        public DialogMode DialogMode
        {
            get { return m_dialogMode; }
            set { m_dialogMode = value; }
        }

        public string IDName
        {
            get { return m_idName; }
            set { m_idName = value; }
        }

        public string Url { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int? Top { get; set; }

        public bool ShowInCurrentFrame { get; set; }

        public string InlineParams { get; set; }
       
        public string Title
        {
            get { return m_title; }
            set { m_title = GetResString(value); }
        }

        public Dictionary<string, string> ExternalDynamicParams
        {
            get { return m_externalParams; }
            set { m_externalParams = value; }
        }

        public override string OnClientClick
        {
            get
            {
                if (Owner is GeneralGrid == false && Owner is HierarchicalGridControl == false)            
                {
                    throw new NotSupportedException((string.Format("Command '{0}': not supported owner", Key)));
                }

                var grid = (GridBaseControl)Owner;

                var dynamicClientParameters = new Dictionary<string, string>(_dynamicClientParams);

                if (DialogMode == DialogMode.ViewEdit && SelectMode != CommandGridSelectMode.No)
                {
                    var sb = new StringBuilder();

                    switch (SelectMode)
                    {
                        case CommandGridSelectMode.MultiRow:
                            sb.AppendLine("var ids = " + grid.ClientGetSelectedRows() + ";");
                            sb.AppendLine("if(ids == \'\')");
                            sb.AppendLine("{");
                            sb.Append("var row = " + grid.ClientGetCurrentRow() + ";");
                            sb.Append(string.Format(@"ids = row.get_cellByColumnKey('{0}').get_value();", IDColumnName));
                            sb.AppendLine("}");
                            sb.AppendLine("return ids;");
                            break;
                        case CommandGridSelectMode.SingleRow:
                            sb.AppendLine("var row = " + grid.ClientGetCurrentRow() + ";");
                            sb.AppendLine(string.Format(@"return row.get_cellByColumnKey('{0}').get_value();",
                                                        IDColumnName));
                            break;
                    }

                    dynamicClientParameters.Add(IDName, String.Format(@"(function(){{{0}}}())", sb));
                }

                var scriptGenerator = new OverlayCommandClientFunctionGenerator(GetClientFunctionName(grid.ClientID),
                                                                                BaseForm.BaseRelativePath(Url),
                                                                                Title,
                                                                                HttpContext.Current.Request.QueryString,
                                                                                InlineParams,
                                                                                ExternalDynamicParams,
                                                                                dynamicClientParameters,
                                                                                Height, 
                                                                                Width,
                                                                                Top,
                                                                                ShowInCurrentFrame);

                grid.Page.RegisterScriptBlock(scriptGenerator.GetFunctionBody(), scriptGenerator.FunctionName);

                return scriptGenerator.FunctionName + "(" + scriptGenerator.GetParametersAsJsonObject() + ");";
            }
        }
    }
}