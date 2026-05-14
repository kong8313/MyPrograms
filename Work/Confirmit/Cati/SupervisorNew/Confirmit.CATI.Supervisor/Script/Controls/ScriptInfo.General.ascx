<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="ScriptInfo.General.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Script.Controls.ScriptInfo_General" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<table class="settings-table settings-table--default-columns settings-table--no-min-width">
    <tr>
        <td>
            <%=Strings.Name%>
            <controls:TextFieldValidator ID="tfxvScriptName" ControlToValidate="tbScriptName"
                IsRequired="true" FieldRequredErrorMessage="Err_EmptyName" ValidationErrorMessage="ErrorIncorrectValue"
                Text="*" runat="server" />
        </td>
        <td>
            <controls:TextBox ID="tbScriptName" runat="server" Width="100%" MaxLength="255" DisableSubmitOnEnter="True" ></controls:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <%=Strings.DesignStateGroup%>
        </td>
        <td>
            <controls:DropDownList runat="server" ID="ddlStatesList" Width="100%"></controls:DropDownList>
        </td>
    </tr>
</table>
