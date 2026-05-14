<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="DialerProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.DialerProperties" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <script language="javascript" type="text/javascript">
        function OnReconnectionEnabled(checkBox) {
            if (checkBox.checked) {
                document.getElementById("<%=reconnectDurationLabel.ClientID%>").classList.remove("disabled");
                document.getElementById("<%=reconnectDuration.ClientID%>").querySelector('input').disabled = false;
            } else {
                document.getElementById("<%=reconnectDurationLabel.ClientID%>").classList.add("disabled");
                document.getElementById("<%=reconnectDuration.ClientID%>").querySelector('input').disabled = true;
            }
        }

        Y.on('load', function () {
            OnReconnectionEnabled(document.getElementById("<%=reconnectAutomatically.ClientID%>"));
        });
    </script>
    <style>
        .dialer-property-label {
            width: 350px;
        }

        .dialer-property-label--warning {
            color: red;
        }

        .dialer-property-label.dialer-property-label--connection {
            width: 190px;
        }

        .task-choice-boxes {
            margin-right: 20px;
        }
        
        .task-choice-boxes input, .task-choice-boxes label {
                vertical-align: middle;
        }

        .task-choice-container {
            display: flex;
        }

        .disabled {
            opacity: 0.3;
        }

        .dialer-reconnection-label {
            width: 190px;
        }
        .dialer-reconnection-label.disabled {
            opacity: 0.3;
        }
        .dialer-reconnection-settings {
            display: flex;
            column-gap: 9px;
        }

        .dialer-reconnection-duration {
            width: 60px;
        }
    </style>
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="OnClick" CausesValidation="True" />
        <Content>
            <asp:UpdatePanel ID="updatePanel" runat="server" ChildrenAsTriggers="True" UpdateMode="Always" class="content-panel" style="display: block;">
                <ContentTemplate>
                    <div class="flex-panel flex-panel-column dialer-properties">
                        <controls:Hint ID="propertiesHint" runat="server" Text="<%$CPResource:DialerPropertiesCannotDetectHint %>" />
                        <controls:Hint ID="dialerIsActiveHint" runat="server" Text="<%$CPResource:DialerDeleteEditWarning %>" />
                        <table class="settings-table settings-table--default-columns">
                            <tr>
                                <td>
                                    <asp:Label ID="lblDialerType" Text="<%$CPResource:DialerType%>" runat="server" />
                                </td>
                                <td>
                                    <controls:DropDownList ID="ddlDialerType" AutoPostBack="True" CausesValidation="False" OnSelectedIndexChanged="ddlDialerType_OnSelectedIndexChanged" runat="server" Width="100%"></controls:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblDialType" Text="<%$CPResource:DialType%>" runat="server" />
                                </td>
                                <td>
                                    <controls:DropDownList ID="ddlDialType" AutoPostBack="False" CausesValidation="False" runat="server" Width="100%"></controls:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblId" Text="ID" runat="server" />
                                </td>
                                <td>
                                    <controls:TextBox ID="tbId" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblDialerName" Text="<%$CPResource:Name%>" runat="server" />
                                </td>
                                <td>
                                    <controls:TextBox ID="tbName" runat="server"></controls:TextBox>
                                </td>
                            </tr>

                        </table>
                        <hr id="hrBeforeConnectionParams" runat="server" />
                        <table runat="server" id="tbDialerConnectionParameters" class="settings-table settings-table--default-columns">
                        </table>
                        <table class="settings-table settings-table--default-columns">
                            <tr>
                                <td style="width: 190px;">
                                    <asp:Label ID="lblWhitelist" Text="<%$CPResource:Whitelist%>" runat="server" />
                                </td>
                                <td>
                                    <controls:TextBox ID="tbWhitelist" runat="server"></controls:TextBox>
                                </td>
                            </tr>
                        </table>
                        <table class="settings-table settings-table--default-columns">
                            <tr>
                                <td class="dialer-reconnection-label">
                                    <asp:Label ID="reconnectAutomaticallyLabel" Text="<%$CPResource:ReconnectAutomaticallyLabel%>" runat="server" />
                                </td>
                                <td class="dialer-reconnection-settings">
                                    <controls:CheckBox ID="reconnectAutomatically" runat="server" AutoPostBack="false" onclick="OnReconnectionEnabled(this)" />

                                    <asp:Label ID="reconnectDurationLabel" Text="<%$CPResource:ReconnectDurationLabel%>" runat="server" />

                                    <div class="dialer-reconnection-duration">
                                        <controls:DateTimeEditor ID="reconnectDuration" runat="server" HorizontalAlign="Left"
                                            EditModeFormat="HH:mm" Nullable="false" MinimumNum="2">
                                            <Buttons SpinButtonsDisplay="OnRight">
                                            </Buttons>
                                        </controls:DateTimeEditor>
                                    </div>
                                    <controls:HelpTextViewer ID="ReconnectAutomaticallyHelpViewer" runat="server"
                                        HelpTextId="ReconnectAutomaticallyHelpText" TitleTextId="ReconnectAutomaticallyHelpTitle" />
                                </td>
                            </tr>
                        </table>
                        <hr id="hrBeforeConfigParams" runat="server" />
                        <table runat="server" id="tbDialerConfigurationParameters" class="settings-table settings-table--default-columns">
                        </table>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlDialerType" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </Content>
    </controls:Dialog>
    <script type="text/javascript">
        function alertIfServiceAddressChanged(oldServiceAddress) {
            var newServiceAddress = document.getElementsByName('inputForServiceAddress')[0].value;
            if(oldServiceAddress != newServiceAddress)
                alert('It will take up to 5 seconds to apply this change. Please wait a bit before activating this dialer again');
        }
    </script>
</asp:Content>
