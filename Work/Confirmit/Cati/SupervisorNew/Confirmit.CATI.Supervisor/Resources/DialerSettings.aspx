<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="DialerSettings.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.DialerSettings" %>

<%@ Register TagPrefix="controls" Src="~/Resources/Controls/DialerParameterControl.ascx"
    TagName="DialerParameterControl" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <div class="tab-content">
        <controls:StateChecker runat="server" ID="stateChecker" AutomaticallySubscribeOnChangeEvents="True" ShowBeforeUnloadWarning="True" />
        <controls:GeneralToolbar runat="server" ID="toolbar" LeftLabel="<%$CPResource:GlobalDialerSettings%>" MakeMarginForExpanCollapseButton="True">
            <RightMenuItems>
                <controls:XpMenuItem ID="btnSave" runat="server" ImageName="save" Text="<%$CPResource:Save%>"
                    OnClick="SaveDialerSettings" />
            </RightMenuItems>
        </controls:GeneralToolbar>
        <controls:Hint ID="ParametersHint" Text="<%$CPResource:DialerParametersHint%>" runat="server" />
        <div class="tab-content__wrapper">
            <div class="settings-table settings-table--nowrap-labels">
                <controls:DialerParameters ID="ParametersArea" runat="server" />
                <controls:DialerParameterControl ID="RespondentVariablesParameter" runat="server" />
                <controls:DialerParameterControl ID="EmailParameter" ValidInputExpression="^[^&lt;&gt;&amp;'\x00-\x1F\x7F-\x9F]*$" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>
