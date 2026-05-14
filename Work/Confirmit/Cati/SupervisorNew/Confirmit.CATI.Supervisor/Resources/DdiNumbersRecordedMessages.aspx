<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DdiNumbersRecordedMessages.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.DdiNumbersRecordedMessages"
    MasterPageFile="~/MasterPages/Main.Master" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content">
    <style>
        .repeat-column {
            padding-top: 0.25em;
            padding-bottom: 0.25em;
        }

        .default-repeat-column {
            width: 80px;
            padding-left: 10px;
            padding-right: 5px;
        }
    </style>

    <div class="tab-content">
        <controls:GeneralToolbar runat="server" ID="Toolbar" LeftLabel="<%$CPResource:DdiNumbersRecordedMessagesTitle%>" MakeMarginForExpanCollapseButton="True">
            <RightMenuItems>
                <controls:XpMenuItem ID="ButtonSave" runat="server" ImageName="save" Text="<%$CPResource:Save%>"
                    OnClick="SaveDdiNumberSettings" />
            </RightMenuItems>
        </controls:GeneralToolbar>

        <controls:Hint ID="ParametersHint" Text="<%$CPResource:DdiNumbersDefaultRecordedMessagesHint%>" runat="server" />

        <controls:StateChecker runat="server" ID="stateChecker" AutomaticallySubscribeOnChangeEvents="True" ShowBeforeUnloadWarning="True" />
        <div class="tab-content__wrapper">
            <table cellspacing="0" cellpadding="0" class="settings-table settings-table--dropdown-auto-width settings-table--long-labels" runat="server">
                <tr>
                    <th>
                        <%=Strings.DdiNumbersRecordedMessagesFirstColumnTitle%>
                    </th>
                    <th style="padding-left: 4px;">
                        <%=Strings.DdiNumbersRecordedMessagesRepeatColumnTitle%>
                    </th>
                    <th />
                    <th>
                        <%=Strings.DdiNumbersRecordedMessagesSecondColumnTitle%>
                    </th>
                    <th style="padding-left: 10px;" id="DefaultRepeatColumnTitle" colspan="2">
                        <%=Strings.DdiNumbersRecordedMessagesDefaultColumnTitle%>
                    </th>
                    <th class="helpColumn" />
                </tr>

                <tr>
                    <td>
                        <%=Strings.DdiNumbersRecordedMessagesIncomingCallCompulsoryMessage%>
                    </td>
                    <td class="repeat-column">
                        <controls:DropDownList ID="ddlIncomingCallCompulsoryMessageUrlRepeatCount" runat="server" />
                    </td>
                    <td>
                        <asp:Label runat="server" ID="LabelIncomingCallCompulsoryMessageErrorAsterisk" ForeColor="red">*</asp:Label>
                    </td>
                    <td>
                        <controls:TextBox ID="TextBoxIncomingCallCompulsoryMessageUrl" runat="server" Width="100%" />
                    </td>
                    <td id="DefaultRepeatIncomingCallCompulsoryMessageUrl" class="default-repeat-column">
                        <asp:Label ID="TextBoxDefaultRepeatIncomingCallCompulsoryMessageUrl" runat="server" Enabled="false"/>
                    </td>
                    <td id="DefaultIncomingCallCompulsoryMessageUrl">
                        <controls:TextBox ID="TextBoxDefaultIncomingCallCompulsoryMessageUrl" runat="server" Width="100%" Enabled="false" />
                    </td>
                    <td class="helpColumn">
                        <controls:HelpTextViewer ID="hvIncomingCallCompulsoryMessage" runat="server" 
                                                 HelpTextId="DdiNumbersIncomingCallCompulsoryMessageHelpText" TitleTextId="DdiNumbersIncomingCallCompulsoryMessage" />
                    </td>
                </tr>

                <tr>
                    <td>
                        <%=Strings.DdiNumbersRecordedMessagesIncomingCallMessage%>
                    </td>
                    <td class="repeat-column">
                        <controls:DropDownList ID="ddlIncomingCallUrlRepeatCount" runat="server" />
                    </td>
                    <td>
                        <asp:Label runat="server" ID="LabelIncomingCallErrorAsterisk" ForeColor="red">*</asp:Label>
                    </td>
                    <td>
                        <controls:TextBox ID="TextBoxIncomingCallUrl" runat="server" Width="100%" />
                    </td>
                    <td id="DefaultRepeatIncomingCallUrl" class="default-repeat-column">
                        <asp:Label ID="TextBoxDefaultRepeatIncomingCallUrl" runat="server" Enabled="false" />
                    </td>
                    <td id="DefaultIncomingCallUrl">
                        <controls:TextBox ID="TextBoxDefaultIncomingCallUrl" runat="server" Width="100%" Enabled="false" />
                    </td>
                    <td class="helpColumn">
                        <controls:HelpTextViewer ID="hvIncomingCallMessage" runat="server" HelpTextId="DdiNumbersIncomingCallHelpText" TitleTextId="DdiNumbersIncomingCall" />
                    </td>
                </tr>

                <tr>
                    <td>
                        <%=Strings.DdiNumbersRecordedMessagesSystemFaultMessage%>
                    </td>
                    <td class="repeat-column">
                        <controls:DropDownList ID="ddlSystemFaultUrlRepeatCount" runat="server" />
                    </td>
                    <td>
                        <asp:Label runat="server" ID="LabelSystemFaultAsterisk" ForeColor="red">*</asp:Label>
                    </td>
                    <td>
                        <controls:TextBox ID="TextBoxSystemFaultUrl" runat="server" Width="100%" />
                    </td>
                    <td id="DefaultRepeatSystemFaultUrl" class="default-repeat-column">
                        <asp:Label ID="TextBoxDefaultRepeatSystemFaultUrl" runat="server" Enabled="false" />
                    </td>
                    <td id="DefaultSystemFaultUrl">
                        <controls:TextBox ID="TextBoxDefaultSystemFaultUrl" runat="server" Width="100%" Enabled="false" />
                    </td>
                    <td class="helpColumn">
                        <controls:HelpTextViewer ID="hvSystemFaultMessage" runat="server" HelpTextId="DdiNumbersSystemFaultHelpMessage" TitleTextId="DdiNumbersSystemFault" />
                    </td>
                </tr>

                <tr>
                    <td>
                        <%=Strings.DdiNumbersRecordedMessagesCampaignIsNotAvailableMessage%>
                    </td>
                    <td class="repeat-column">
                        <controls:DropDownList ID="ddlDropCampaignIsNotAvailableRepeatCount" runat="server" />
                    </td>
                    <td>
                        <asp:Label runat="server" ID="LabelDropCampaignIsNotAvailableAsterisk" ForeColor="red">*</asp:Label>
                    </td>
                    <td>
                        <controls:TextBox ID="TextBoxDropCampaignIsNotAvailableUrl" runat="server" Width="100%" />
                    </td>
                    <td id="DefaultRepeatDropCampaignIsNotAvailableUrl" class="default-repeat-column">
                        <asp:Label ID="TextBoxDefaultRepeatDropCampaignIsNotAvailableUrl" runat="server" Enabled="false" />
                    </td>
                    <td id="DefaultDropCampaignIsNotAvailableUrl">
                        <controls:TextBox ID="TextBoxDefaultDropCampaignIsNotAvailableUrl" runat="server" Width="100%" Enabled="false" />
                    </td>
                    <td class="helpColumn">
                        <controls:HelpTextViewer ID="hvCampaignIsNotAvailable" runat="server" HelpTextId="DdiNumbersCampaignIsNotAvailableHelpText" TitleTextId="DdiNumbersCampaignIsNotAvailable" />
                    </td>
                </tr>

                <tr>
                    <td>
                        <%=Strings.DdiNumbersRecordedMessagesDropCallInterviewNotFoundMessage%>
                    </td>
                    <td class="repeat-column">
                        <controls:DropDownList ID="ddlDropCallInterviewNotFoundRepeatCount" runat="server" />
                    </td>
                    <td>
                        <asp:Label runat="server" ID="LabelDropCallInterviewNotFoundAsterisk" ForeColor="red">*</asp:Label>
                    </td>
                    <td>
                        <controls:TextBox ID="TextBoxDropCallInterviewNotFoundUrl" runat="server" Width="100%" />
                    </td>
                    <td id="DefaultRepeatDropCallInterviewNotFoundUrl" class="default-repeat-column">
                        <asp:Label ID="TextBoxDefaultRepeatDropCallInterviewNotFoundUrl" runat="server" Enabled="false" />
                    </td>
                    <td id="DefaultDropCallInterviewNotFoundUrl">
                        <controls:TextBox ID="TextBoxDefaultDropCallInterviewNotFoundUrl" runat="server" Width="100%" Enabled="false" />
                    </td>
                    <td class="helpColumn">
                        <controls:HelpTextViewer ID="hvDropCallInterviewNotFound" runat="server" HelpTextId="DdiNumbersDropCallInterviewNotFoundHelpText" TitleTextId="DdiNumbersDropCallInterviewNotFound" CustomHeight="225" />
                    </td>
                </tr>

                <tr>
                    <td>
                        <%=Strings.DdiNumbersRecordedMessagesDropCallOutOfShiftMessage%>
                    </td>
                    <td class="repeat-column">
                        <controls:DropDownList ID="ddlDropCallOutsideOfOperationHoursRepeatCount" runat="server" />
                    </td>
                    <td>
                        <asp:Label runat="server" ID="LabelDropCallOutsideOfOperationHoursAsterisk" ForeColor="red">*</asp:Label>
                    </td>
                    <td>
                        <controls:TextBox ID="TextBoxDropCallOutsideOfOperationHoursUrl" runat="server" Width="100%" />
                    </td>
                    <td id="DefaultRepeatDropCallOutsideOfOperationHoursUrl" class="default-repeat-column">
                        <asp:Label ID="TextBoxDefaultRepeatDropCallOutsideOfOperationHoursUrl" runat="server" Enabled="false" />
                    </td>
                    <td id="DefaultDropCallOutsideOfOperationHoursUrl">
                        <controls:TextBox ID="TextBoxDefaultDropCallOutsideOfOperationHoursUrl" runat="server" Width="100%" Enabled="false" />
                    </td>
                    <td class="helpColumn">
                        <controls:HelpTextViewer ID="hvDropCallOutsideOfOperationHours" runat="server" HelpTextId="DdiNumbersDropCallOutOfShiftHelpText" TitleTextId="DdiNumbersDropCallOutOfShift" CustomHeight="215" />
                    </td>
                </tr>

            </table>
        </div>
    </div>
</asp:Content>

