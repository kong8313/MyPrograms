﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.ServerControls.Commands
{
    public class BaseOverlayCommand : Command
    {
        public bool RefreshListFrame { get; set; }

        public bool RefreshInfoFrame { get; set; }

        public bool RefreshOwner { get; set; }

        public string GetAfterOverlayClosedJobScript()
        {
            var closingScript = new StringBuilder();

            if (RefreshOwner && Owner is GeneralGrid)
            {
                ServerClick += (Owner as GeneralGrid).RefreshHandler;
            }

            if (ServerClickEventHandler != null)
            {
                var postbackReference =
                    ((Page)HttpContext.Current.Handler).ClientScript.GetPostBackEventReference(
                        new PostBackOptions(Owner, Key, "", false, false, true, true, true, ""));
                //changes quotes to apostrophes to correct java-script
                postbackReference = Regex.Replace(postbackReference, "\"", "'");

                closingScript.AppendLine(postbackReference);
            }

            if (RefreshListFrame)
            {
                closingScript.AppendLine("Common.refreshListFrame();");
            }
            if (RefreshInfoFrame)
            {
                closingScript.AppendLine("Common.refreshInfoFrame();");
            }

            return closingScript.ToString();
        }

        public override string GetClientEventJavaScript(Page page, Control baseControl)
        {
            string command = string.Format("var overlay = {0}; overlay.overlayClosedEvent.on(function(args){{if(args.result !== true) return; {1}}}, this);",
                                            OnClientClick, GetAfterOverlayClosedJobScript());

            if (String.IsNullOrEmpty(Confirmation) == false)
            {
                command = string.Format("if(confirm('{0}')){{{1}}};", ResourceWrapper.Instance.GetString(Confirmation), command);
            }

            if (Owner is GeneralGrid)
            {
                var grid = Owner as GeneralGrid;

                if (SelectMode == CommandGridSelectMode.SingleRow || SelectMode == CommandGridSelectMode.MultiRow)
                {
                    var selectionCondition = grid.ClientGetCurrentRow() + "== null";
                    var warningMessage = (SelectMode == CommandGridSelectMode.MultiRow)
                                             ? Strings.NoRowsSelected
                                             : Strings.NoRowchosen;
                    if (SelectMode == CommandGridSelectMode.MultiRow) selectionCondition += " && !" + grid.ClientGetIsRowsSelected();


                    command = String.Format("if({0}){{window.alert('{1}')}} else {{{2}}};", selectionCondition, warningMessage, command);

                }
            }
            //needed to support context menu references using pseudo-protocol javascript:
            command = "(function(){" + command + "})();";

            return command;
        }
    }
}