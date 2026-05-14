<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecuritySettingsControl.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Resources.Controls.Settings.SecuritySettingsControl" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Panel ID="Panel1" runat="server" DefaultButton="btnDefault">

    <controls:GeneralToolbar runat="server" ID="toolbar"  LeftLabel="<%$CPResource:SecuritySiteSettingsHint%>">
        <RightMenuItems>
            <controls:XpMenuItem ID="btnSaveProperties" runat="server" ImageName="save" Text="<%$CPResource:Save%>" />
        </RightMenuItems>
    </controls:GeneralToolbar>
    <section class="content-panel">
        <div class="hidden">
            <asp:Button ID="btnDefault" runat="server" />
        </div>

        <%--<controls:Hint ID="Hint1" Text="<%$CPResource:SecuritySiteSettingsHint%>" runat="server" />--%>
        <table class="settings-table settings-table--default-columns settings-table--no-min-width">
            <tr>
                <td nowrap="nowrap">
                    <%=Strings.EnableInterviewerAccountLockout%>
                </td>
                <td>
                    <table class="settings-table__complex-row">
                        <tr>
                            <td>
                                <controls:CheckBox ID="cbAccountLockingEnabled" runat="server" AutoPostBack="false"
                                    onclick="OnAccountLockingEnabled();" />
                            </td>
                            <td id="tdAfter"><%=Strings.After%></td>
                            <td>
                                <controls:NumericEdit runat="server" ID="neNumberOfAttempts" MinValue="1" ValueText="3"
                                    MaxValue="99" Nullable="False">
                                    <Buttons SpinButtonsDisplay="OnRight"></Buttons>
                                    <ClientEvents Initialize="OnAccountLockingEnabled" TextChanged="OnAccountLockingValueChanged" />
                                </controls:NumericEdit>
                            </td>
                            <td id="tdAttemps"><%=Strings.Attempts%>.</td>
                            <td>
                                <controls:HelpTextViewer ID="hvAccountLockingEnabled" runat="server" HelpTextId="EnableInterviewerAccountLockoutHelpText" TitleTextId="EnableInterviewerAccountLockout" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <tr>
                <td nowrap="nowrap"><%=Strings.EnableInterviewerPasswordExpiration%></td>
                <td>
                    <table class="settings-table__complex-row">
                        <tr>
                            <td>
                                <controls:CheckBox ID="cbPasswordExpirationEnabled" runat="server" AutoPostBack="false" onclick="OnPasswordExpirationEnabled();" />
                            </td>
                            <td id="tdExpireAfter" nowrap="nowrap"><%=Strings.ExpireAfter%></td>
                            <td>
                                <controls:NumericEdit runat="server" ID="neExpireAfterNumber" MinValue="1"
                                    MaxValue="999" Nullable="False">
                                    <Buttons SpinButtonsDisplay="OnRight">
                                    </Buttons>
                                    <ClientEvents Initialize="OnPasswordExpirationEnabled" TextChanged="OnPasswordExpirationValueChanged" />
                                </controls:NumericEdit>
                            </td>
                            <td id="tdDays"><%=Strings.DaysSmall%>.</td>
                            <td>
                                <controls:HelpTextViewer ID="hvPasswordExpirationEnabled" runat="server" HelpTextId="EnablePasswordExpirationHelpText" TitleTextId="EnablePasswordExpiration" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <tr>
                <td nowrap="nowrap">
                    <%=Strings.EnforceMinimumPasswordLength%>
                </td>
                <td>
                    <table class="settings-table__complex-row">
                        <tr>
                            <td>
                                <controls:CheckBox ID="cbEnforceMinimumPasswordLengthEnabled" runat="server" AutoPostBack="false"
                                    onclick="OnEnforceMinimumPasswordLengtEnabled();" />
                            </td>
                            <td id="tdAtLeast" nowrap="nowrap">
                                <%=Strings.AtLeast%>
                            </td>
                            <td>
                                <controls:NumericEdit runat="server" ID="nePasswordLength" MinValue="1" MaxValue="99" Nullable="False">
                                    <Buttons SpinButtonsDisplay="OnRight">
                                    </Buttons>
                                    <ClientEvents Initialize="OnEnforceMinimumPasswordLengtEnabled" TextChanged="OnPasswordLengthValueChanged" />
                                </controls:NumericEdit>
                            </td>
                            <td id="tdCharacters"><%=Strings.Characters%>.</td>
                            <td>
                                <controls:HelpTextViewer ID="hvEnforceMinimumPasswordLengthEnabled" runat="server"
                                    HelpTextId="EnableEnforceMinimumPasswordLengthHelpText" TitleTextId="EnableEnforceMinimumPasswordLength" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <tr>
                <td nowrap="nowrap"><%=Strings.EnforceComplexPasswords%></td>
                <td>
                    <table class="settings-table__complex-row">
                        <tr>
                            <td>
                                <controls:CheckBox ID="cbEnforceComplexPasswordsEnabled" runat="server" AutoPostBack="false" />
                            </td>
                            <td>
                                <controls:HelpTextViewer ID="hvEnforceComplexPasswordsEnabled" runat="server"
                                    HelpTextId="EnableEnforceComplexPasswordsHelpText" TitleTextId="EnableEnforceComplexPasswords" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <tr>
                <td nowrap="nowrap"><%=Strings.AlwaysEncryptFiles%></td>
                <td>
                    <table class="settings-table__complex-row">
                        <tr>
                            <td>
                                <controls:CheckBox ID="cbAlwaysEncryptFiles" runat="server" AutoPostBack="false" onclick="OnAlwaysEncryptFiles()" />
                            </td>
                            <td>
                                <controls:HelpTextViewer ID="hvAlwaysEncryptFilesEnabled" runat="server"
                                    HelpTextId="AlwaysEncryptFilesHelpText" TitleTextId="EnableAlwaysEncryptFiles" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="settings-table__label-with-indent">
                    <%=Strings.UserForEncryption%>
                </td>
                <td>
                    <table class="settings-table__complex-row">
                        <tr>
                            <td>
                                <controls:TextBox ID="UserForEncryptionTextBox" runat="server" AutoPostBack="False" />
                            </td>
                            <td>
                                <controls:HelpTextViewer ID="UserForEncryptionHelpViewer" runat="server" HelpTextId="UserForEncryptionHelpText"
                                    TitleTextId="UserForEncryptionHelpTitle" />
                            </td>
                            <td>
                                <asp:CustomValidator ID="cvUserForEncryption" ControlToValidate="UserForEncryptionTextBox" Display="Dynamic" CssClass="validation-error"
                                    ValidateEmptyText="True" OnServerValidate="ValidateUserForEncryption" runat="server" ErrorMessage="<%$CPResource:UserForEncryptionInvalidFormatMessage%>" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td nowrap="nowrap"><%=Strings.IsChangeAfterFirstLoginRequired%></td>
                <td>
                    <table class="settings-table__complex-row">
                        <tr>
                            <td>
                                <controls:CheckBox ID="cbIsChangeAfterFirstLoginRequired" runat="server" AutoPostBack="false" />
                            </td>
                            <td>
                                <controls:HelpTextViewer ID="IsChangeAfterFirstLoginRequiredHelpViewer" runat="server"
                                                         HelpTextId="IsChangeAfterFirstLoginRequiredHelpText" TitleTextId="IsChangeAfterFirstLoginRequired" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

        </table>
    </section>
</asp:Panel>
<script type="text/javascript">
    function OnAccountLockingEnabled() {
        var checkBoxControl = document.getElementById("<%=cbAccountLockingEnabled.ClientID%>");
        var enabled = checkBoxControl.checked;

        window.$IG.WebTextEditor.find("<%=neNumberOfAttempts.ClientID%>").set_enabled(enabled);
        document.getElementById("tdAfter").disabled = !enabled;
        document.getElementById("tdAttemps").disabled = !enabled;
    }

    function OnAlwaysEncryptFiles() {
        var checkBoxControl = document.getElementById("<%=cbAlwaysEncryptFiles.ClientID%>");
        var enabled = checkBoxControl.checked;

        document.getElementById("<%=UserForEncryptionTextBox.ClientID%>").disabled = !enabled;
    }

    function OnPasswordExpirationEnabled() {
        var checkBoxControl = document.getElementById("<%=cbPasswordExpirationEnabled.ClientID%>");
        var enabled = checkBoxControl.checked;

        window.$IG.WebTextEditor.find("<%=neExpireAfterNumber.ClientID%>").set_enabled(enabled);

        document.getElementById("tdExpireAfter").disabled =
            document.getElementById("tdDays").disabled = !enabled;
    }

    function OnEnforceMinimumPasswordLengtEnabled() {
        var checkBoxControl = document.getElementById("<%=cbEnforceMinimumPasswordLengthEnabled.ClientID%>");
        var enabled = checkBoxControl.checked;

        window.$IG.WebTextEditor.find("<%=nePasswordLength.ClientID%>").set_enabled(enabled);
        document.getElementById("tdAtLeast").disabled =
            document.getElementById("tdCharacters").disabled = !enabled;
    }

    function OnAccountLockingValueChanged() {
        window.StateChecker.MarkAsChanged();
    }

    function OnPasswordExpirationValueChanged() {
        window.StateChecker.MarkAsChanged();
    }

    function OnPasswordLengthValueChanged() {
        window.StateChecker.MarkAsChanged();
    }

    Y.on('load', function () {
        OnAlwaysEncryptFiles();
    });
</script>
