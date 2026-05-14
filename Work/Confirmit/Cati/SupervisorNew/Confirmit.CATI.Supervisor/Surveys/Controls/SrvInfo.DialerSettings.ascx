<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SrvInfo.DialerSettings.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Surveys.SrvInfoDialerSettings" %>
<controls:StateChecker runat="server" ID="stateChecker" AutomaticallySubscribeOnChangeEvents="True" ShowBeforeUnloadWarning="True" />
<div class="tab-content">
    <controls:GeneralToolbar runat="server" ID="toolbar">
        <RightMenuItems>
            <controls:XpMenuItem ID="btnRefresh" runat="server" ImageName="refresh" Text="<%$CPResource:Refresh%>"
                OnClick="Refresh" />
            <controls:XpMenuItem ID="btnSave" runat="server" ImageName="save" Text="<%$CPResource:Save%>"
                OnClick="SaveDialerSettings" />
            <controls:XpMenuItem ID="btnReset" runat="server" ImageName="reset" Text="<%$CPResource:ResetToDefaultValues%>"
                OnClick="ResetParams" />
        </RightMenuItems>
    </controls:GeneralToolbar>
    <controls:Hint ID="surevyDialerSettingsHint" runat="server" Text="<%$CPResource:SurveyDialerSettingsHint%>" />
    <div class="tab-content__wrapper">
        <div class="settings-table settings-table--nowrap-labels">
            <controls:DialerParameters ID="ParametersArea" runat="server" />
            <% if (ToggleSettings.EnableInbound)
                { %>
            <div class="settings-table__row">
                <div class="settings-table__label">
                    <asp:Label ID="lblDdiNumbers" Text="<%$CPResource:InboundDdiNumber%>" runat="server" />
                </div>
                <div class="settings-table__value">
                    <asp:Label ID="lblDdiNumbersValues" runat="server" />
                </div>
            </div>
            <% } //foreach %>
        </div>
    </div>
</div>
