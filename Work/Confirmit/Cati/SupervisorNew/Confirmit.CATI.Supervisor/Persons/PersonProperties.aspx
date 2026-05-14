<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master"
    AutoEventWireup="true" CodeBehind="PersonProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Persons.PersonProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Classes" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<%@ Register TagPrefix="Controls" TagName="PersonSurveyAssignmentList" Src="~/Persons/Controls/PersonSurveyAssignmentList.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <script type="text/javascript">
        function SelectedIndexChanged(sender, args) {
            PageMethods.SetSelectedTab(sender.getTabAt(args.get_tabIndex()).get_key());
        }
    </script>
    <controls:StateChecker runat="server" ID="stateChecker" ShowBeforeUnloadWarning="True" />
    <controls:Dialog ID="dialogControl" runat="server" HideButtons="True" PutActionButtonsInsideGridIfPossible="False" HideHeader="True">
        <okbutton visible="True" text="Create" />
        <savebutton visible="False" />
        <content>		
            <input type="hidden" runat="server" id="m_AutoSurveyId" name="m_AutoSurveyId"/>			
            
            <controls:Tabs runat="server" ID="tabs" style="height: 100%; width: 100%">
                <Tabs>
                    <controls:TabItem runat="server" TextId="Properties" Key="Properties">
                        <Template>
                            <div class="tab-content">
                                <div class="frame-dialog-header <%=IsNewPerson?"hidden":""%>">
                                    <div class="frame-dialog-header__text">
                                        <asp:Label runat="server" ID="lbPageInfo"></asp:Label>
                                    </div>
                                    <div class="frame-dialog-header__controls">
                                        <controls:GeneralToolbar ID="toolbar" runat="server">
                                            <controls:XpMenuItem ID="btnSaveProperties" runat="server" ImageName="save" TextId="Save" OnClick="SaveHandler" />
                                        </controls:GeneralToolbar>
                                    </div>
                                </div>
                                <div class="tab-content__wrapper  flex-panel" style="flex-wrap: wrap;">
                                    <table style="width: auto; margin-right: 30px;" class="settings-table settings-table--default-columns settings-table--no-min-width">
                                        <tr ID="trIdRow" runat="server">
                                            <td><%=Strings.ID%></td>
                                            <td><asp:Label ID="lblID" runat="server"/></td>
                                        </tr>
                                        <tr>
                                            <td><%=Strings.Login%>				                    
                                                <controls:TextFieldValidator ID="tfxvLogin" ControlToValidate="tbxLogin"
                                                                             IsRequired="true" FieldRequredErrorMessage="Err_EmptyLogin"	ValidationErrorMessage="ErrorIncorrectValue" Text="*" runat="server" />
                                            </td>				                    
                                            <td><Controls:TextBox ID="tbxLogin" runat="server" MaxLength="255" onchange="StateChecker.MarkAsChanged()"/></td>
                                        </tr>
                                        <tr ID="pnlNewPassword" runat="server">
                                            <td>
                                                <%=Strings.Password%>
                                                <asp:RequiredFieldValidator runat="server" ID="tfxvPassword" ControlToValidate="tbxPassword" Text="*" ErrorMessage="<%$CPResource:Err_PasswordIsEmpty%>"/>
                                            </td>
                                            <td>
                                                <Controls:TextBox ID="tbxPassword" runat="server" TextMode="Password"/>
                                            </td>
                                        </tr>
                                        <tr ID="trConfirmPassword" runat="server">
                                            <td>
                                                <%=Strings.ConfirmPassword%>
                                                <asp:RequiredFieldValidator runat="server" ID="tbxvConfirm" ControlToValidate="tbxConfirm" Text="*" ErrorMessage="<%$CPResource:Err_PasswordIsEmpty%>"/>
                                            </td>
                                            <td><Controls:TextBox ID="tbxConfirm" runat="server" TextMode="Password"/></td>
                                        </tr>
                                        <tr runat="server" ID="pnlChangePassword" Visible="False">
                                            <td><%=Strings.Password%></td>
                                            <td>
                                                <asp:LinkButton runat="server" ID="lbtnChangePassword" Text="<%$CPResource:Change%>" />							
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <%=Strings.PersonPropertiesDisplayName%>
                                                <controls:TextFieldValidator ID="tfxvDisplayName" ControlToValidate="tbxDisplayName"
                                                                             IsRequired="false" ValidationErrorMessage="ErrorIncorrectValue" Text="*" runat="server" />
                                            </td>
                                            <td>
                                                <div class="settings-table__with-help">
                                                    <Controls:TextBox ID="tbxDisplayName" runat="server" MaxLength="255" onchange="StateChecker.MarkAsChanged()"/>
                                                    <div class = "divInline">
                                                        <controls:HelpTextViewer ID="HelpTextInterviewerDisplayName" runat="server"
                                                            HelpTextId="InterviewerDisplayNameHelpText" />
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <%=Strings.Description%>
                                                <controls:TextFieldValidator ID="tfxvDescription" ControlToValidate="tbxDescription"
                                                                             IsRequired="false" ValidationErrorMessage="ErrorIncorrectValue" Text="*" runat="server" />
                                            </td>
                                            <td>
                                                <Controls:TextBox ID="tbxDescription" runat="server" MaxLength="255" onchange="StateChecker.MarkAsChanged()"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <%=Strings.Location%>
                                                <controls:TextFieldValidator ID="tfxvLocation" ControlToValidate="tbLocation"
                                                    IsRequired="false"
                                                    ValidationErrorMessage="ErrorIncorrectValue" Text="*" runat="server" />
                                            </td>
                                            <td>
                                                <div class="settings-table__with-help">
                                                    <controls:TextBox ID="tbLocation" onchange="StateChecker.MarkAsChanged()" MaxLength="255" runat="server"/>
                                                    <div class = "divInline">
                                                        <controls:HelpTextViewer ID="HelpTextInterviewerLocation" runat="server"
                                                            HelpTextId="InterviewerLocationHelpText" TitleTextId="InterviewerLocationTitleId" />
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr ID="pnlLockedDate" runat="server" Visible="false">
                                            <td><%=Strings.LockedDate%></td>
                                            <td>
                                                <asp:Label ID="lblLockedDate" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <table style="width: auto;" class="settings-table settings-table--default-columns settings-table--no-min-width">
                                        <tr>
                                            <td><%=Strings.PersonAssignmentListMode%></td>
                                            <td>
                                                <controls:DropDownList ID="ddlAssignmentListMode" onchange="StateChecker.MarkAsChanged()" runat="server"/>
                                            </td>
                                        </tr>

                                        <tr runat="server" id="trCallGroup" Visible="False">
                                            <td>
                                                <%=Strings.CallGroup%>
                                            </td>
                                            <td>          
                                                <div class="settings-table__with-help">
                                                 <controls:DropDownList ID="ddlCallGroups" onchange="StateChecker.MarkAsChanged()" runat="server">
                                                 </controls:DropDownList>
                                                 <div class = "divInline">
                                                     <controls:HelpTextViewer ID="hvCallGroups" runat="server" HelpTextId="PersonViewCallGroupsHelpText"
                                                           TitleTextId="CallGroups" />
                                                    </div>
                                                 <asp:Panel runat="server" ID="pnlCallGroupWarning" CssClass="divInline" >
                                                    <asp:Label ID="lbCallGroupsWarning" Font-Bold="True" ForeColor="Red" runat="server" Text="<%$CPResource:Warning%>" 
                                                    ToolTip="<%$CPResource:CallGroupsCanOnlyBeUsedInSurveySelectionTaskChoice%>" />  
                                                 </asp:Panel>       </div>                                                                                                                                                                                           
                                            </td>
                                        </tr>
                                        <tr>
                                            <td><%=Strings.PersonProperties_TaskChoice%></td>
                                            <td>
                                                <controls:TaskChoiceDropDownList ID="ddlTaskChoice" runat="server"/>
                                            </td>
                                        </tr>								
                                        <tr ID="pnlTaskChoicePermissions" Visible="false" runat="server">
                                            <td colspan="2">
                                                <controls:SelectTaskChoicePermissions ID="m_SelectTaskChoicePermissions" runat="server"	/>						
                                            </td>
                                        </tr>																									
                                        <tr runat="server" id = "rowAutomaticSurvey" visible="false">
                                            <td><%=Strings.AutomaticSurvey%></td>
                                            <td>
                                                <table border="0" cellpadding="0" cellspacing="0">
                                                    <tr>							                    
                                                        <td>
                                                            <asp:LinkButton runat="server" ID="lbtnChangeAutoSurvey" Text="<%$CPResource:Change%>" />													             
                                                        </td>								                    
                                                        <td style="padding-left:3px;">
                                                            <asp:Label ID="lblAutoSurveyName" runat="server" />
                                                        </td>
                                                        <td style="padding-left:3px;">
                                                            <input type="image" src="~/images/icon-delete.gif" ID="btnClearAutomaticSurvey" runat="server" style="cursor: pointer;"
                                                                   title="<%$CPResource:ClearAutomaticSurvey%>" onclick="clearAutomaticSurvey(); return false;" />

                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr runat="server" ID="dialTypeRow" visible="false">
                                            <td nowrap>
                                                <%=Strings.PersonPropertiesDialType%>
                                            </td>
                                            <td>
                                                <controls:DialTypeDropDownList ID="ddlDialType" runat="server" AddNoChangeOption="False" onchange="StateChecker.MarkAsChanged()"/>
                                            </td>
                                        </tr>
                                        <tr runat="server" ID="typeOfDialerSSO" visible="false">
                                            <td nowrap>
                                                <%=Strings.SSOIntegration%>
                                            </td>
                                            <td>
                                                <div class="settings-table__with-help">
                                                    <controls:DropDownList ID="ddlSSOIntegration" runat="server">
                                                        <asp:ListItem Text="<%$CPResource:NoSSO%>" />
                                                        <asp:ListItem Selected="true" Text="<%$CPResource:DefaultSSO%>" />
                                                    </controls:DropDownList>
                                                    <div class = "divInline">
                                                        <controls:HelpTextViewer ID="ChangeSSOHelpTextId" runat="server"
                                                        HelpTextId="ChangeSSOHelpText" TitleTextId="SSOIntegrationTitleId" />
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr runat="server" ID="statusRow">
                                            <td><%=Strings.Status%></td>
                                            <td><asp:Label ID="lblStatus" runat="server"></asp:Label></td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </Template>
                    </controls:TabItem>
                    <controls:TabItem runat="server" TextId="Attributes" Key="Attributes" >
                        <Template>
                            <div class="tab-content">
                                <div class="frame-dialog-header <%=IsNewPerson?"hidden":""%>">
                                    <div class="frame-dialog-header__text">
                                        <asp:Label runat="server" ID="lblAttributes"></asp:Label>
                                    </div>
                                    <div class="frame-dialog-header__controls">
                                        <controls:GeneralToolbar runat="server">
                                            <controls:XpMenuItem ID="btnSaveAttributes" runat="server" ImageName="save" TextId="Save" OnClick="SaveHandler" />
                                        </controls:GeneralToolbar>
                                    </div>
                                </div>
                                <controls:Hint ID="attributesHint" runat="server" Text="<%$CPResource:PersonAttributesHint%>" />
                                <div class="tab-content__wrapper  flex-panel" style="flex-wrap: wrap;">
                                    <table style="width: auto; margin-right: 30px;" class="settings-table settings-table--default-columns settings-table--no-min-width">
                                        <tr>
                                            <td runat="server" ID="lblAttribute1">
                                                Attribute1
                                            </td>
                                            <td>
                                                <Controls:TextBox ID="tbxAttribute1" runat="server" MaxLength="50" onchange="StateChecker.MarkAsChanged()"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td runat="server" ID="lblAttribute2">
                                                Attribute2
                                            </td>
                                            <td>
                                                <Controls:TextBox ID="tbxAttribute2" runat="server" MaxLength="50" onchange="StateChecker.MarkAsChanged()"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td runat="server" ID="lblAttribute3">
                                                Attribute3
                                            </td>
                                            <td>
                                                <Controls:TextBox ID="tbxAttribute3" runat="server" MaxLength="50" onchange="StateChecker.MarkAsChanged()"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td runat="server" ID="lblAttribute4">
                                                Attribute4
                                            </td>
                                            <td>
                                                <Controls:TextBox ID="tbxAttribute4" runat="server" MaxLength="50" onchange="StateChecker.MarkAsChanged()"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td runat="server" ID="lblAttribute5">
                                                Attribute5
                                            </td>
                                            <td>
                                                <Controls:TextBox ID="tbxAttribute5" runat="server" MaxLength="50" onchange="StateChecker.MarkAsChanged()"/>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </Template>
                    </controls:TabItem>
                    <controls:TabItem runat="server" TextId="Membership" Key="Membership" >
                        <Template>
                            <div class="tab-content">
                                <div class="frame-dialog-header <%=IsNewPerson?"hidden":""%>">
                                    <div class="frame-dialog-header__text">
                                        <asp:Label runat="server" ID="lblMembership"></asp:Label>
                                    </div>
                                    <div class="frame-dialog-header__controls">
                                        <controls:GeneralToolbar runat="server">
                                            <controls:XpMenuItem ID="btnSaveMembership" runat="server" ImageName="save" TextId="Save" OnClick = "SaveHandler" />
                                        </controls:GeneralToolbar>
                                    </div>
                                </div>
                                <div class="tab-content__wrapper">
                                    <Controls:DoubleListBox id="membershipLists" runat="server" Rows="10" />
                                    <asp:CustomValidator ID="GroupSelectedValidator" ClientValidationFunction="ValidateParentGroupSelected" CssClass="validation-error" runat="server" Text="*" ErrorMessage="<%$CPResource:UnableToAddPersonOutOfGroup%>"/>
                                </div>
                            </div>
                        </Template>
                    </controls:TabItem>
                    <controls:TabItem runat="server" TextId="Assignment" Key="Assignment" >
                        <Template>
                            <controls:PersonSurveyAssignmentList ID="AssignmentList" runat="server" IsGroup="false" />
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

        function taskChoiceChanged() {
            var ddlTaskChoice = document.getElementById("<%=ddlTaskChoice.ClientID%>");
            var panel = document.getElementById("<%=pnlTaskChoicePermissions.ClientID%>");

            if (ddlTaskChoice && panel) {
                if (ddlTaskChoice.value == "<%=(int)ConfirmitDialerInterface.AgentTaskChoiceMode.Choice%>") {
                    panel.style.display = "";
                }
                else {
                    panel.style.display = "none";
                }
                showCallGroupWarning(ddlTaskChoice.value != "<%=(int)ConfirmitDialerInterface.AgentTaskChoiceMode.CampaignAssignment%>");
            }
        }

        function showCallGroupWarning(visible) {

            var panel = Y.one("#<%=pnlCallGroupWarning.ClientID%>");
            var ddlCallGroups = document.getElementById("<%=ddlCallGroups.ClientID%>");

            if (ddlCallGroups) {
                if (visible && ddlCallGroups.selectedIndex > 0) {
                    panel.show();
                } else {
                    panel.hide();
                }
            }
        }

        //Occurs when task choice 'Choice' is selected and 'SurveySelection' checkbox is checked or unchecked
        function taskChoicePermissionChanged(choice, isChecked, skipMarkAsChanged) {
            if (!skipMarkAsChanged) {
                StateChecker.MarkAsChanged();
            }
            if (choice == "<%=(int)Confirmit.CATI.Common.TaskChoicePermissions.SurveyAssignment%>") {
                var rowAutomaticSurvey = document.getElementById("<%=rowAutomaticSurvey.ClientID%>");

                if (isChecked) {
                    rowAutomaticSurvey.style.display = "";
                }
                else {
                    rowAutomaticSurvey.style.display = "none";
                }

                showCallGroupWarning(!isChecked);
            }
        }

        function showSelectAutomaticSurveyDialog(personId, title, width, height) {

            var settings = { height: height + "px", width: width + "px" };

            top.overlay.overlayClosedEvent.on(function (args) {
                if (args.result !== true)
                    return;

                var returnValue = args.data;
                if (returnValue) {
                    var returnArray = returnValue.split(",");
                    setAutomaticSurvey(returnArray[0], returnArray[1], false);
                }
            });

            top.overlay.show(title, "Persons/SelectAutomaticSurveyDialog.aspx?PersonId=" + personId, null, settings, null);
        }

        function clearAutomaticSurvey() {
            setAutomaticSurvey('', '', true);
        }

        function setAutomaticSurvey(surveyId, surveyName, hideClearButton) {
            StateChecker.MarkAsChanged();
            var btnClearAutomaticSurvey = document.getElementById("<%=btnClearAutomaticSurvey.ClientID%>");
            var m_AutoSurveyId = document.getElementById("<%=m_AutoSurveyId.ClientID%>");
            var labelAutoSurvey = document.getElementById("<%=lblAutoSurveyName.ClientID%>");

            if (hideClearButton)
                btnClearAutomaticSurvey.style.visibility = "hidden";
            else
                btnClearAutomaticSurvey.style.visibility = "visible";

            m_AutoSurveyId.value = surveyId;
            labelAutoSurvey.innerHTML = surveyName;
        }

        function changePasswordDialog(personId) {

            var settings = { height: "180px", width: "520px", top: "50px", calledWindow: window };

            var params = { PersonId: personId };

            top.overlay.show("Change Password", "Persons/ChangePersonPassword.aspx", params, settings, null);

            top.overlay.overlayClosedEvent.on(function (args) {
                Common._setProcessingState(false);
            });

            return overlay;

        }

    </script>
</asp:Content>
