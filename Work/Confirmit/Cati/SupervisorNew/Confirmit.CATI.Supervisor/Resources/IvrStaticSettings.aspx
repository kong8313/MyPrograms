<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IvrStaticSettings.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.IvrStaticSettings"
    MasterPageFile="~/MasterPages/Main.Master" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content">
    <div class="tab-content">
        <controls:GeneralToolbar runat="server" ID="Toolbar" LeftLabel="<%$CPResource:IvrStaticSettingsTitle%>" MakeMarginForExpanCollapseButton="True">
            <RightMenuItems>
                <controls:XpMenuItem ID="ButtonSave" runat="server" ImageName="save" Text="<%$CPResource:Save%>"
                    OnClick="SaveIvrStaticSettings" />
            </RightMenuItems>
        </controls:GeneralToolbar>

        <controls:StateChecker runat="server" ID="stateChecker" AutomaticallySubscribeOnChangeEvents="True" ShowBeforeUnloadWarning="True" />
        <div class="tab-content__wrapper">
            <table class="settings-table settings-table--nowrap-labels settings-table--fixed-labels-300px settings-table--default-columns">
                <tr>
                    <td class="firstColumn">
                        <%=Strings.IvrStaticSettingsTermCharMessage%>
                    </td>
                    <td class="errorAsterisk">
                        <asp:Label runat="server" ID="LabelTermCharErrorAsterisk" ForeColor="red">*</asp:Label>
                    </td>
                    <td class="settings-table__value">
                        <div class="settings-table__with-help">
                            <controls:TextBox ID="TextBoxTermChar" runat="server" MaxLength="1" />
                            <div class="divInline">
                                <controls:HelpTextViewer ID="hvTermCharMessage" runat="server" HelpTextId="IvrStaticSettingsTermCharHelpText" TitleTextId="IvrStaticSettingsTermChar" />
                            </div>
                        </div>
                    </td>
                </tr>

                <tr>
                    <td class="firstColumn">
                        <%=Strings.IvrStaticSettingsRecordTypeMessage%>
                    </td>
                    <td />
                    <td class="settings-table__value">
                        <div class="settings-table__with-help">
                            <controls:DropDownList ID="ddlRecordType" runat="server" />
                            <div class="divInline">
                                <controls:HelpTextViewer ID="hvRecordTypeMessage" runat="server" HelpTextId="IvrStaticSettingsRecordTypeHelpText" TitleTextId="IvrStaticSettingsRecordType" />
                            </div>
                        </div>
                    </td>
                </tr>

                <tr>
                    <td class="firstColumn">
                        <%=Strings.IvrStaticSettingsBeepMessage%>
                    </td>
                    <td />
                    <td class="settings-table__value">
                        <div class="settings-table__with-help">
                            <controls:CheckBox ID="CheckBoxBeep" runat="server" />
                            <div class="divInline">
                                <controls:HelpTextViewer ID="hvBeepMessage" runat="server" HelpTextId="IvrStaticSettingsBeepHelpText" TitleTextId="IvrStaticSettingsBeep" />
                            </div>
                        </div>
                    </td>
                </tr>

                <tr>
                    <td class="firstColumn">
                        <%=Strings.IvrStaticSettingsMaxTimeMessage%>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="LabelMaxTimeErrorAsterisk" ForeColor="red">*</asp:Label>
                    </td>
                    <td class="settings-table__value">
                        <div class="settings-table__with-help">
                            <controls:TextBox ID="TextBoxMaxTime" runat="server" />
                            <div class="divInline">
                                <controls:HelpTextViewer ID="hvMaxTimeMessage" runat="server" HelpTextId="IvrStaticSettingsMaxTimeHelpText" TitleTextId="IvrStaticSettingsMaxTime" />
                            </div>
                        </div>
                    </td>
                </tr>

                <tr>
                    <td class="firstColumn">
                        <%=Strings.IvrStaticSettingsFinalSilenceMessage%>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="LabelFinalSilenceErrorAsterisk" ForeColor="red">*</asp:Label>
                    </td>
                    <td class="settings-table__value">
                        <div class="settings-table__with-help">
                            <controls:TextBox ID="TextBoxFinalSilence" runat="server" />
                            <div class="divInline">
                                <controls:HelpTextViewer ID="hvFinalSilenceMessage" runat="server" HelpTextId="IvrStaticSettingsFinalSilenceHelpText" TitleTextId="IvrStaticSettingsFinalSilence" CustomHeight="225" />
                            </div>
                        </div>

                    </td>
                </tr>

                <tr>
                    <td class="firstColumn">
                        <%=Strings.IvrStaticSettingsTransferTimeoutMessage%>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="LabelTransferTimeoutErrorAsterisk" ForeColor="red">*</asp:Label>
                    </td>
                    <td class="settings-table__value">
                        <div class="settings-table__with-help">
                            <controls:TextBox ID="TextBoxTransferTimeout" runat="server" />
                            <div class="divInline">
                                <controls:HelpTextViewer ID="hvTransferTimeout" runat="server" HelpTextId="IvrStaticSettingsTransferTimeoutHelpText" TitleTextId="IvrStaticSettingsTransferTimeoutHelpTitle" CustomHeight="225" />
                            </div>
                        </div>
                    </td>
                </tr>

                <tr>
                    <td class="firstColumn">
                        <%=Strings.IvrStaticSettingsDtmfTermMessage%>
                    </td>
                    <td />
                    <td class="settings-table__value">
                        <div class="settings-table__with-help">
                            <controls:CheckBox ID="CheckBoxDtmfTerm" runat="server" />
                            <div class="divInline">
                                <controls:HelpTextViewer ID="hvDtmfTermMessage" runat="server" HelpTextId="IvrStaticSettingsDtmfTermHelpText" TitleTextId="IvrStaticSettingsDtmfTerm" CustomHeight="215" />
                            </div>
                        </div>
                    </td>
                </tr>

            </table>
        </div>
    </div>
</asp:Content>

