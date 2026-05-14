<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="SelectShiftForReport.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Reports.SelectShiftForReport" %>

<%@ Register TagPrefix="controls" TagName="Dte" Src="../Controls/DateTimeEdit.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="Content">

    <controls:Dialog ID="dialogControl" runat="server" EnableViewState="true" HideHeader="True" Mode="Modal">
        <OKButton Text="Save" OnClick="SaveSelected"></OKButton>
        <Content>
            <controls:Hint runat="server" Text="<%$CPResource:SelectShiftForReport%>" />
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td nowrap>
                            <asp:Label ID="lblShiftStartTime" runat="server" Text="<%$CPResource:StartShiftReport%>" />
                        </td>
                        <td>
                            <controls:Dte ID="dteShiftStartTime" ShowDate="False" runat="server" AutoPostBack="false" />
                        </td>
                    </tr>
                    <tr>
                        <td nowrap>
                            <asp:Label ID="lblShiftEndTime" runat="server" Text="<%$CPResource:EndShiftReport%>" />
                        </td>
                        <td>
                            <controls:Dte ID="dteShiftEndTime" ShowDate="false" runat="server" AutoPostBack="false" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <controls:CheckBox ID="cbxResetShift" runat="server" Text="<%$CPResource:ResetShiftReport%>"
                                Checked="false" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
