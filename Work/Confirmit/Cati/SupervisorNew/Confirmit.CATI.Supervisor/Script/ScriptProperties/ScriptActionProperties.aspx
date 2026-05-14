<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="ScriptActionProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Script.ScriptActionProperties" %>

<%@ Register TagPrefix="asp" Namespace="System.Web.UI.HtmlControls" Assembly="System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>
<%@ Register TagPrefix="Controls" TagName="CodeEditor" Src="~/Script/Controls/CodeEditor.ascx" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClientClick="if(!ValidateChanges()) return false;" OnClick="OKButtonClick" />
        <Content>
            <main class="content-panel" style="overflow: auto">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width settings-table--dropdown-auto-width">

                    <tr>
                        <td style="width: 135px">Action enabled</td>
                        <td>
                            <controls:CheckBox ID="cbActionEnabled" runat="server" Checked="True" /></td>
                    </tr>
                    <tr>
                        <td>Filter enabled</td>
                        <td>
                            <controls:CheckBox ID="cbFilterEnabled" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblSubFilter" Text="Filter" runat="server" />
                        </td>
                        <td style="height: 100px; max-width: 500px" class="textarea-framed">
                            <Controls:CodeEditor runat="server" ID="codeEditorFilter" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblAction" Text="Action" runat="server" />
                        </td>
                        <td>
                            <controls:DropDownList ID="ddlAction" runat="server" columnKey="ActionName" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <textarea class="info-textarea" runat="server" readonly="readonly"
                                id="taActionDescription" columnkey="ActionDescription"></textarea>
                        </td>
                    </tr>
                    <tr id="rblParameters" runat="server">
                        <td colspan="2" style="padding-right: 10px;">
                            <table width="100%" cellspacing="0">
                                <tr>
                                    <td style="width: 135px">
                                        <div class="flex-panel">
                                            <controls:RadioButton ID="rbConst" Checked="true" runat="server" GroupName="ActionParams" CssClass="cati-radio"
                                                Text=" " AutoPostBack="false" />
                                            <label for="<%=rbConst.ClientID %>"><%=Strings.Value %></label>
                                        </div>
                                    </td>
                                    <td>
                                        <controls:TextBox ID="tbConst" runat="server" Width="100%" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="flex-panel">
                                            <controls:RadioButton ID="rbParam" Checked="false" runat="server" GroupName="ActionParams"
                                                Text=" " AutoPostBack="false" />
                                            <label for="<%=rbParam.ClientID %>">Parameter</label>
                                        </div>
                                    </td>
                                    <td>
                                        <controls:DropDownList ID="ddlSchedulingParams" runat="server" Width="100%" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:HtmlInputHidden ID="hdnIsSchedulingParam" columnKey="IsSchedulingParameter"
                                            runat="server" />
                                        <asp:HtmlInputHidden ID="hdnParamValue" columnKey="ParameterValue" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
    <script id="shiftScript" language="javascript" type="text/javascript">

        /*Validation for entered time*/
        function ValidateChanges() {
            return SchedulingActionProperties.validate('<%=ddlAction.ClientID%>', '<%=rbConst.ClientID%>', '<%=tbConst.ClientID%>', '<%=ddlSchedulingParams.ClientID%>', '<%=hdnParamValue.ClientID%>', '<%=hdnIsSchedulingParam.ClientID%>');
        }

    </script>

</asp:Content>
