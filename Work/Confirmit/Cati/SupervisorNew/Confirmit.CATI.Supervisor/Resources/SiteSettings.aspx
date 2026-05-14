<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SiteSettings.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.SiteSettings" %>

<%@ Register TagPrefix="settings" tagName="GeneralSettingsControl" src="Controls/Settings/GeneralSettingsControl.ascx" %>
<%@ Register TagPrefix="settings" tagName="InterviewerConsoleSettingsControl" src="Controls/Settings/InterviewerConsoleSettingsControl.ascx" %>
<%@ Register TagPrefix="settings" tagName="MonitoringConsoleSettingsControl" src="Controls/Settings/MonitoringConsoleSettingsControl.ascx" %>
<%@ Register TagPrefix="settings" tagName="SecuritySettingsControl" src="Controls/Settings/SecuritySettingsControl.ascx" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <script type="text/javascript">
        function SelectedIndexChanged(sender, args) {
            window.PageMethods.SetSelectedTab(sender.getTabAt(args.get_tabIndex()).get_key());
        }

        function EnableAllControls() {
            Y.all("input[type='text'], input[type='checkbox'], select").set("disabled", false);
        }
    </script>

    <Controls:StateChecker runat="server" ID="stateChecker" ShowBeforeUnloadWarning="True" AutomaticallySubscribeOnChangeEvents="True"/>
    <controls:Tabs runat="server" ID="tabs" Style="height: 100%; width: 100%">
        <Tabs>

            <controls:TabItem runat="server" TextId="SettingsTabs_General" Key="General">
                <Template>
                    <settings:GeneralSettingsControl ID="generalSettings" runat="server" />
                </Template>
            </controls:TabItem>

            <controls:TabItem runat="server" TextId="SettingsTabs_InterviewerConsole" Key="InterviewerConsole">
                <Template>
                    <settings:InterviewerConsoleSettingsControl ID="interviewerConsoleSettings" runat="server"/>
                </Template>
             </controls:TabItem>

            <controls:TabItem runat="server" TextId="SettingsTabs_MonitoringConsole" Key="MonitoringConsole">
                <Template>
                    <settings:MonitoringConsoleSettingsControl ID="monitoringConsoleSettings" runat="server"/>
                </Template>
             </controls:TabItem>

            <controls:TabItem runat="server" TextId="SettingsTabs_Security" Key="Security" >
                <Template>
                    <settings:SecuritySettingsControl Id="securitySettings" runat="server" />
                </Template>
            </controls:TabItem>

        </Tabs>
    </controls:Tabs>

</asp:Content>
