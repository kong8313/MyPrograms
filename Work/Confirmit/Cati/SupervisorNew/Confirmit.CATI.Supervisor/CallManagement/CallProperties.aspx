<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="CallProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.CallProperties" %>

<asp:Content runat="server" ContentPlaceHolderID="Content" ID="Content">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="OKButton_Click" />
        <Content>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td>
                            <asp:Label runat="server" Text="<%$CPResource:InterviewId%>" />
                        </td>
                        <td colspan="2">
                            <controls:NumericEdit ID="wneInterviewID" runat="server" Width="170" Nullable="False"
                                ValueText="1" MinValue="1">
                                <Buttons SpinButtonsDisplay="OnRight">
                                </Buttons>
                            </controls:NumericEdit>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="<%$CPResource:TimeToCall%>" />
                            <span class="settings-table__help">
                                <controls:HelpTextViewer runat="server" ID="helpTimeToCall" HelpTextId="EditTimeToCallHelpText"
                                                         TitleTextId="TimeToCall"></controls:HelpTextViewer>
                            </span>
                        </td>
                        <td>
                            <controls:DateTimeEdit ID="dteTimeToCall" runat="server" />
                        </td>
                        <td>
                            <controls:CheckBox ID="cbxTimeToCall" runat="server" Text="<%$CPResource:SetToNow%>"
                                Checked="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblTimeToExpire" runat="server" Text="<%$CPResource:TimeToExpire%>" />
                            <span class="settings-table__help">
                                <controls:HelpTextViewer runat="server" ID="helpTimeToExpire" HelpTextId="EditTimeToExpireHelpText"
                                                         TitleTextId="TimeToExpire"></controls:HelpTextViewer>
                            </span>
                        </td>
                        <td>
                            <controls:DateTimeEdit ID="dteTimeToExpire" runat="server" />
                        </td>
                        <td>
                            <controls:CheckBox ID="cbxTimeToExpire" runat="server" Text="<%$CPResource:SetToNever%>"
                                Checked="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" Text="Enable" />
                        </td>
                        <td colspan="2">
                            <controls:CheckBox ID="cbxEnable" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblPriority" runat="server" Text="<%$CPResource:CallPriority%>" />
                        </td>
                        <td colspan="2">
                            <controls:NumericEdit ID="wnePriority" runat="server" Width="170" Nullable="False"
                                NullValue="1" MinValue="1">
                                <Buttons SpinButtonsDisplay="OnRight">
                                </Buttons>
                            </controls:NumericEdit>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" Text="<%$CPResource:ShiftTypeName%>" />
                        </td>
                        <td colspan="2">
                            <controls:ShiftTypesDropDown ID="ddlShiftType" runat="server" AutoPostBack="false"
                                Width="170">
                            </controls:ShiftTypesDropDown>
                        </td>
                    </tr>

                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
