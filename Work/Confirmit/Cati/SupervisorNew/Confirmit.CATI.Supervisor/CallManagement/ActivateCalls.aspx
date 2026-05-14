<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ActivateCalls.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.ActivateCalls" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" HideHeader="true" Mode="Modal">
        <OKButton OnClick="SaveButtonClick" Text="Activate" />
        <Content>
            <main class="content-panel flex-panel-column">
                <div class="flex-panel flex-panel-row">
                    <div class="dialog-property">
                        <div class="dialog-property__label">
                            <asp:Label ID="lblPriority" runat="server" Text="<%$CPResource:CallPriority%>" />
                        </div>
                        <div class="dialog-property__edit">
                            <controls:NumericEdit ID="wnePriority" runat="server" Width="173" Nullable="False"
                                ValueText="0" MinValue="1">
                                <Buttons SpinButtonsDisplay="OnRight">
                                </Buttons>
                            </controls:NumericEdit>
                        </div>
                    </div>
                    <div class="dialog-property">
                        <div class="dialog-property__label">
                            <asp:Label ID="lblShiftType" runat="server" Text="<%$CPResource:ShiftTypeName%>" />
                        </div>
                        <div class="dialog-property__edit">
                            <controls:ShiftTypesDropDown ID="ddlShiftType" runat="server" AutoPostBack="false" Style="width: 173px;">
                            </controls:ShiftTypesDropDown>
                        </div>
                    </div>
                </div>
                <div class="flex-panel flex-panel-row">
                    <div class="dialog-property">
                        <div class="dialog-property__label">
                            <asp:Label ID="lblTimeToCall" runat="server" Text="<%$CPResource:TimeToCall%>" />
                            <span class="dialog-property__help">
                                <controls:HelpTextViewer runat="server" ID="helpTimeToCall" HelpTextId="EditTimeToCallHelpText"
                                                     TitleTextId="TimeToCall"></controls:HelpTextViewer>
                            </span>
                        </div>
                        <div class="dialog-property__edit" style="width: 400px;">
                            <controls:DateTimeEdit ID="dteTimeToCall" runat="server" />
                            <controls:CheckBox ID="cbxSetToNow" runat="server" Text="<%$CPResource:SetToNow%>" Checked="false" style="margin-left: 10px;"/>
                        </div>
                    </div>
                </div>
                <div class="flex-panel flex-panel-row">
                    <div class="dialog-property">
                        <div class="dialog-property__label">
                            <asp:Label ID="lblExtendedStatus" runat="server" Text="<%$CPResource:ExtendedStatus%>" />
                        </div>
                        <div class="dialog-property__edit">
                            <controls:ExtendedStatusDropDown ID="ddlExtendedStatus" runat="server" AutoPostBack="false">
                            </controls:ExtendedStatusDropDown>
                        </div>
                    </div>
                    <div class="dialog-property">
                        <div class="dialog-property__label">
                            <asp:Label ID="lblEnableDisabledCalls" runat="server" Text="<%$CPResource:EnableDisabledCalls%>" />
                        </div>
                        <div class="dialog-property__edit">
                            <controls:CheckBox ID="cbEnableDisabledCalls" runat="server" TextAlign="Right" />
                            <controls:HelpTextViewer runat="server" ID="helpQuotaForBalancing" HelpTextId="HelpEnableDisabledCalls"
                                TitleTextId="EnableDisabledCalls"></controls:HelpTextViewer>
                        </div>
                    </div>
                </div>
                <controls:Hint ID="noPersonOrGroupHint" Text="<%$CPResource:NoPersonOrGroupMessage%>" CssClass="attention--no-bottom-margin attention--top-margin" runat="server" />
                <controls:UpdatePanel ID="updatePanel" runat="server" class="flex-panel--all-awailable-space">
                    <ContentTemplate>
                        <controls:PersonsAndGroupsList ID="personsAndGroupsList" ListName="GroupsAndPersons" runat="server" DialTypeVisible="False"></controls:PersonsAndGroupsList>
                    </ContentTemplate>
                </controls:UpdatePanel>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
