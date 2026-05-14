<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="MoveCalls.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.MoveCalls" %>

<%@ Register TagPrefix="controls" TagName="AppointmentProperties" Src="~/CallManagement/Controls/AppointmentProperties.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OkButtonClick" />
        <Content>
            <main class="content-panel">
                <div class="flex-panel flex-panel-row flex-panel-row--justify">
                    <asp:Label ID="lblSelectITS" Text="<%$CPResource:SelectITS%>" runat="server" />
                    <controls:DropDownList ID="ddlITS" runat="server" OnSelectedIndexChanged="SelectedStatusChanged" AutoPostBack="true" />
                </div>
                <asp:Panel ID="appointmentPropertiesPanel" runat="server" Visible="false">
                    <controls:Hint ID="CreateAppointmentWhenMoveAndRescheduleHint" Text="<%$CPResource:CreateAppointmentWhenMoveAndRescheduleHint%>" runat="server" />
                    <controls:AppointmentProperties ID="AppointmentProperties" runat="server" />
                </asp:Panel>
            </main>
        </Content>
    </controls:Dialog>
    <script>
        function confirmationPrcocessingLimit(selectedCount, limit, message) {
            if (selectedCount > limit) {
                return confirm(message);
            }

            return true;
        }
    </script>
</asp:Content>
