<%@ Page Language="c#" MasterPageFile="~/MasterPages/Main.Master" CodeBehind="GroupProperties.aspx.cs"
    AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.Persons.GroupProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<%@ Register TagPrefix="Controls" TagName="GroupUserList" Src="~/Persons/Controls/GroupUserList.ascx" %>
<%@ Register TagPrefix="Controls" TagName="PersonSurveyAssignmentList" Src="~/Persons/Controls/PersonSurveyAssignmentList.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">

    <controls:StateChecker runat="server" ID="stateChecker" ShowBeforeUnloadWarning="True" />
    <controls:Dialog runat="server" id="dialog" PutActionButtonsInsideGridIfPossible="False" >
        <okbutton text="Create" />
        <content>
            <controls:Tabs runat="server" ID="tabs" style="height: 100%; width: 100%">
                <Tabs>
                    <controls:TabItem runat="server" TextId="Properties" Key="Properties">
                        <Template>
                            <div class="tab-content">
                            <div class="frame-dialog-header" ID="divHeader" runat="server" Visible="False">
                            <div class="frame-dialog-header__text">
                                <asp:Label runat="server" ID="lbPageInfo"></asp:Label>
                            </div>
                            <div class="frame-dialog-header__controls">
                            <controls:GeneralToolbar runat="server">
                                <controls:XpMenuItem ID="btnSaveProperties" runat="server" ImageName="save" TextId="Save" OnClick = "SaveButtonClick" />
                            </controls:GeneralToolbar>
                            </div>
                            </div>
                            <div class="tab-content__wrapper">
                            <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                                <tr>
                                    <td>
                                        <%=Strings.Name%>
                                        <controls:TextFieldValidator ID="tfvStateGroupName" ControlToValidate="nameInput" IsRequired="true"
                                                                     FieldRequredErrorMessage="Err_EmptyName" ValidationErrorMessage="ErrorIncorrectValue" Text="*" runat="server" />										
                                    </td>
                                    <td>
                                        <Controls:TextBox ID="nameInput" Runat="server" MaxLength="255" onchange="StateChecker.MarkAsChanged()" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <%=Strings.Description%>
                                        <controls:TextFieldValidator ID="tfxvDescription" ControlToValidate="descriptionInput"
                                                                     IsRequired="false" ValidationErrorMessage="ErrorIncorrectValue" Text="*" runat="server" />
                                    </td>
                                    <td>
                                        <Controls:TextBox ID="descriptionInput" Runat="server" MaxLength="255" onchange="StateChecker.MarkAsChanged()"/>
                                    </td>
                                </tr>
                                <tr id="trAllowInboundCallsForOtherSurvey" runat="server">
                                    <td>
                                        <%=Strings.AllowInboundCallsForOtherSurvey%>
                                    </td>
                                    <td>
                                        <Controls:CheckBox ID="cbAllowInboundCallsForOtherSurvey" Runat="server" onchange="StateChecker.MarkAsChanged()"/>
                                    </td>
                                </tr>
                                <tr id="trAllowTransfering" runat="server">
                                    <td>
                                        <%=Strings.AllowTransfering%>
                                    </td>
                                    <td>
                                        <div class="settings-table__with-help">
                                            <Controls:CheckBox ID="cbAllowTransfering" Runat="server" onchange="StateChecker.MarkAsChanged();updateAllowTransferedCallsFromOtherSurvey();"/>
                                            <div class = "divInline">
                                                <controls:HelpTextViewer ID="AllowTransferringHelpTextId" runat="server"
                                                HelpTextId="AllowTransferringHelpText" />
                                            </div>
                                        </div>
                                        
                                    </td>
                                </tr>
                                <tr  id="trAllowTransferedCallsFromOtherSurvey" runat="server">
                                    <td>
                                        <%=Strings.AllowTransferedCallsForOtherSurvey%>
                                    </td>
                                    <td>
                                        <div class="settings-table__with-help">
                                            <Controls:CheckBox ID="cbAllowTransferredCallsFromOtherSurvey" Runat="server" onchange="StateChecker.MarkAsChanged()"/>
                                            <div class = "divInline">
                                                <controls:HelpTextViewer ID="AllowTransferredCallsFromOtherSurveyHelpTextId" runat="server"
                                                HelpTextId="AllowTransferredCallsFromOtherSurveyHelpText" />
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr  id="trIsAdministrative" runat="server">
                                    <td>
                                        <%=Strings.AdministrativeGroup%>
                                    </td>
                                    <td>
                                        <div class="settings-table__with-help">
                                            <Controls:CheckBox ID="cbAdministrativeGroup" Runat="server" onchange="StateChecker.MarkAsChanged();AdministrativeGroupChecked();"/>
                                            <div class = "divInline">
                                                <controls:HelpTextViewer ID="AdministrativeGroupHelpTextId" runat="server"
                                                HelpTextId="AdministrativeGroupHelpText" />
                                            </div>
                                            <div class = "attention attention--warning attention--no-top-margin attention--no-bottom-margin" ID="divAdministrativeGroupWarning" style="visibility: hidden;  display: <%= OldIsAdministrative == null ? "none" : "block" %>;" >
                                                <%=Strings.AdministrativeGroupWarning%>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            </div>
                            </div>
                        </Template>
                    </controls:TabItem>
                    <controls:TabItem runat="server" Text="Interviewers" Key="Interviewers">           
                        <Template>
                            <Controls:GroupUserList id="userList" runat="server"/>
                        </Template>
                    </controls:TabItem>
                    <controls:TabItem runat="server" Text="Assignment" Key="Assignment">           
                        <Template>		
                            <controls:PersonSurveyAssignmentList ID="AssignmentList" runat="server" IsGroup="true" />
                        </Template>
                    </controls:TabItem>
                </Tabs>
            </controls:Tabs>
        </content>
    </controls:Dialog>
    <script type="text/javascript">
        function ValidateParentGroupSelected(source, clientside_arguments) {
            clientside_arguments.IsValid = DoesRightListHaveItems();
        }
        
        function AdministrativeGroupChecked()
        {
            var cbAdministrativeGroup = Y.one("#<%=cbAdministrativeGroup.ClientID%>");
            var checked = cbAdministrativeGroup.get("checked");
            var wasNotAdministrative = <%=(OldIsAdministrative == false).ToString().ToLower()%>;
            var divAdministrativeGroup = document.getElementById("divAdministrativeGroupWarning");
            if(wasNotAdministrative && checked) {
                 divAdministrativeGroup.style.visibility = "visible";
            }else
            {
                divAdministrativeGroup.style.visibility = "hidden";
            }
        }

        function SelectedIndexChanged(sender, args) {
            PageMethods.SetSelectedTab(sender.getTabAt(args.get_tabIndex()).get_key());
        }

        function updateAllowTransferedCallsFromOtherSurvey() {
            var cbAllowTransferring = Y.one("#<%=cbAllowTransfering.ClientID%>");
            var cbAllowTransferredCallsFromOtherSurvey = Y.one("#<%=cbAllowTransferredCallsFromOtherSurvey.ClientID%>");

            if (cbAllowTransferring == null || cbAllowTransferredCallsFromOtherSurvey == null)//feature is disbaled and item isn't rendered in a page
                return;

            var checked = cbAllowTransferring.get("checked");
            cbAllowTransferredCallsFromOtherSurvey.set("disabled", checked ? "" : "disabled");
        }

        updateAllowTransferedCallsFromOtherSurvey();
    </script>
</asp:Content>
