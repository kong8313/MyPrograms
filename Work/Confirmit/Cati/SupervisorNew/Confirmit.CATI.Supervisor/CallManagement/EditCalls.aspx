<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="EditCalls.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.EditCalls" %>

<asp:Content runat="server" ContentPlaceHolderID="Content" ID="Content">
    <style type="text/css">
        .toggle-label {
            vertical-align: top;
            padding-left: 10px;
            font-size: 15px;
        }

        .row-table {
            height: 50px;
        }

        .button-switch{
            width: 85px;
            height: 40px;
            outline: none;
            cursor: pointer;
            font-size: 14px;
            background: none;
            border: 1px solid #E3E4E5;
            color: #A6A9AD;
        }

        .button-switch__selected {
            background: #E1F0F6;
            border: 1px solid #007EB9;
            color: #158CC1;
        }

        .button-switch__deselected{
            color: #2D333C;
        }
    </style>
    <script>
        function disableCheckBoxes() {
            Y.all('#<%=cbxTimeToCall.ClientID%>, #<%=cbxTimeToExpire.ClientID%>').set("disabled", true);
        }

        // Function for switching toggles on/off //
        function toggleTimeToCall(enabled) {
            Y.one('#<%=cbxTimeToCall.ClientID%>').set("disabled", !enabled);

            if (!<%=dteTimeToCall.ClientControllerName%>) {
                return;
            }

            var setToNowCheckBox = Y.one('#<%=cbxTimeToCall.ClientID%>')._node;
            <%=dteTimeToCall.ClientControllerName%>.setEnabled(enabled && !setToNowCheckBox.checked);
        }

        function toggleTimeToExpire(enabled) {
            Y.one('#<%=cbxTimeToExpire.ClientID%>').set("disabled", !enabled);

            if (!<%=dteTimeToExpire.ClientControllerName%>) {
                return;
            }

            var setToNeverCheckBox = Y.one('#<%=cbxTimeToExpire.ClientID%>')._node;
            <%=dteTimeToExpire.ClientControllerName%>.setEnabled(enabled && !setToNeverCheckBox.checked);
        }

        function toggleCallState(enabled) {
            Y.all('#btnCallStateEnabled, #btnCallStateDisabled').set("disabled", !enabled);

            if (!enabled) {
                Y.all('#btnCallStateEnabled, #btnCallStateDisabled').set("className", "button-switch");
            } else {
                onChangeCallStateSelectedValue();
            }
        }

        function toggleCallPriority(enabled) {
            if (!<%=wnePriority.ClientID%>) {
                return;
            }

            Y.one('#<%=wnePriority.ClientID%> input').set("disabled", !enabled);
        }

        function toggleShiftTypeName(enabled) {
            Y.one('#<%=ddlShiftType.ClientID%>').set("disabled", !enabled);
        }

        function toggleExtendedStatus(enabled) {
            Y.one('#<%=ddlExtendedStatus.ClientID%>').set("disabled", !enabled);
        }

        function toggleDialingMode(enabled) {
            Y.all('#btnDialingModeDefault, #btnDialingModePreview, #btnDialingModeSpecial').set("disabled", !enabled);

            if (!enabled) {
                Y.all('#btnDialingModeDefault, #btnDialingModePreview, #btnDialingModeSpecial').set("className", "button-switch");
            } else {
                onChangeDialingModeSelectedValue();
            }
        }

        // Function for selecting enabled/disabled call state //
        function onChangeCallStateSelectedValue() {
            var input = Y.one('#<%=callStateSelectedValue.ClientID%>')._node;
            var isEnabled = input.value === "True";

            Y.one('#btnCallStateEnabled').toggleClass("button-switch__selected", isEnabled).toggleClass("button-switch__deselected", !isEnabled);
            Y.one('#btnCallStateDisabled').toggleClass("button-switch__selected", !isEnabled).toggleClass("button-switch__deselected", isEnabled);
        }
        
        function selectEnabledCallState() {
            Y.one('#<%=callStateSelectedValue.ClientID%>').set("value", "True");
            Y.one('#<%=callStateSelectedValue.ClientID%>')._node.onchange();
        }

        function selectDisabledCallState() {
            Y.one('#<%=callStateSelectedValue.ClientID%>').set("value", "False");
            Y.one('#<%=callStateSelectedValue.ClientID%>')._node.onchange();
        }

        // Function for selecting default/preview/special dialing mode //
        function onChangeDialingModeSelectedValue() {
            var input = Y.one('#<%=dialingModeSelectedValue.ClientID%>')._node;
            var isDefault = input.value === "Default";
            var isPreview = input.value === "Preview";
            var isSpecial = input.value === "Special";

            Y.one('#btnDialingModeDefault').toggleClass("button-switch__selected", isDefault).toggleClass("button-switch__deselected", !isDefault);
            Y.one('#btnDialingModePreview').toggleClass("button-switch__selected", isPreview).toggleClass("button-switch__deselected", !isPreview);
            Y.one('#btnDialingModeSpecial').toggleClass("button-switch__selected", isSpecial).toggleClass("button-switch__deselected", !isSpecial);
        }

        function selectDefaultDialingMode() {
            Y.one('#<%=dialingModeSelectedValue.ClientID%>').set("value", "Default");
            Y.one('#<%=dialingModeSelectedValue.ClientID%>')._node.onchange();
        }

        function selectPreviewDialingMode() {
            Y.one('#<%=dialingModeSelectedValue.ClientID%>').set("value", "Preview");
            Y.one('#<%=dialingModeSelectedValue.ClientID%>')._node.onchange();
        }

        function selectSpecialDialingMode() {
            Y.one('#<%=dialingModeSelectedValue.ClientID%>').set("value", "Special");
            Y.one('#<%=dialingModeSelectedValue.ClientID%>')._node.onchange();
        }
    </script>

    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="OKButton_Click" />
        <Content>
            <asp:Label ID="lblInfo" Text="<%$CPResource:EditCallsGeneralInfo%>" runat="server" style="padding-left: 20px; padding-bottom: 10px;" />
            <main class="content-panel" style="padding-left: 30px">
                <table class="settings-table--no-min-width" >
                    <tr class="row-table">
                        <td style="width:185px">
                            <controls:ToggleSelector ID="timeToCallToggle" runat="server" OnToggle="toggleTimeToCall" Text="<%$CPResource:TimeToCall%>" 
                                                     HelpTextId="EditTimeToCallHelpText" TitleTextId="EditTimeToCallTitleId" />
                        </td>
                        <td style="width:230px">
                            <controls:DateTimeEdit ID="dteTimeToCall" runat="server" />
                        </td>
                        <td style="padding-left: 10px">
                            <controls:CheckBox ID="cbxTimeToCall" runat="server" Text="<%$CPResource:SetToNow%>" />
                        </td>
                    </tr>
                    <tr class="row-table">
                        <td>
                            <controls:ToggleSelector ID="timeToExpireToggle" runat="server" OnToggle="toggleTimeToExpire" Text="<%$CPResource:TimeToExpire%>" 
                                                     HelpTextId="EditTimeToExpireHelpText" TitleTextId="EditTimeToExpireTitleId" />
                        </td>
                        <td>
                            <controls:DateTimeEdit ID="dteTimeToExpire" runat="server" />
                        </td>
                        <td style="padding-left: 10px">
                            <controls:CheckBox ID="cbxTimeToExpire" runat="server" Text="<%$CPResource:SetToNever%>" />
                        </td>
                    </tr>
                    <tr class="row-table">
                        <td>
                            <controls:ToggleSelector ID="callStateToggle" runat="server" OnToggle="toggleCallState" Text="<%$CPResource:CallState%>" />
                        </td>
                        <td colspan="2">
                            <button ID="btnCallStateEnabled" class="button-switch" type="button" disabled onclick="selectEnabledCallState()" >Enabled</button>
                            <button ID="btnCallStateDisabled" class="button-switch" type="button" disabled onclick="selectDisabledCallState()" >Disabled</button>
                            <input type="text" style="display: none;" runat="server" ID="callStateSelectedValue" onchange="onChangeCallStateSelectedValue()" value="False" />
                        </td>
                    </tr>
                    <tr class="row-table">
                        <td>
                            <controls:ToggleSelector ID="callPriorityToggle" runat="server" OnToggle="toggleCallPriority" Text="<%$CPResource:CallPriority%>" />
                        </td>
                        <td colspan="2">
                            <controls:NumericEdit ID="wnePriority" runat="server" SelectionOnFocus="NotSet" Enabled="False" Width="120" Nullable="False" NullValue="1" MinValue="1">
                                <Buttons SpinButtonsDisplay="OnRight" />
                            </controls:NumericEdit>
                        </td>
                    </tr>
                    <tr class="row-table">
                        <td>
                            <controls:ToggleSelector ID="shiftTypeNameToggle" runat="server" OnToggle="toggleShiftTypeName" Text="<%$CPResource:ShiftTypeName%>" />
                        </td>
                        <td>
                            <controls:ShiftTypesDropDown ID="ddlShiftType" runat="server" Enabled="False" AutoPostBack="false" Width="230" />
                        </td>
                        <td/>
                    </tr>
                    <tr class="row-table">
                        <td>
                            <controls:ToggleSelector ID="extendedStatusToggle" runat="server" OnToggle="toggleExtendedStatus" Text="<%$CPResource:ExtendedStatus%>" 
                                                     HelpTextId="EditExtendedStatusHelpText" TitleTextId="EditExtendedStatusTitleId" />
                        </td>
                        <td colspan="2">
                            <controls:DropDownList ID="ddlExtendedStatus" runat="server" AutoPostBack="false" Enabled="False" />
                        </td>
                    </tr>
                    <tr id="dialingModeInfo" class="row-table" runat="server">
                        <td>
                            <controls:ToggleSelector ID="dialingModeToggle" runat="server" OnToggle="toggleDialingMode" Text="<%$CPResource:DialingModeColumnText%>" />
                        </td>
                        <td colspan="2">
                            <button ID="btnDialingModeDefault" class="button-switch" type="button" disabled onclick="selectDefaultDialingMode()">Default</button>
                            <button ID="btnDialingModePreview" class="button-switch" type="button" disabled onclick="selectPreviewDialingMode()">Preview</button>
                            <button ID="btnDialingModeSpecial" class="button-switch" type="button" disabled onclick="selectSpecialDialingMode()">Special</button>
                            <input type="text" style="display: none;" runat="server" ID="dialingModeSelectedValue" onchange="onChangeDialingModeSelectedValue()" value="Default" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
