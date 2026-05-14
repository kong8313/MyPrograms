<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master"
    AutoEventWireup="true" CodeBehind="IvrAgentProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Persons.AgentProperties" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<%@ Register TagPrefix="Controls" TagName="PersonSurveyAssignmentList" Src="~/Persons/Controls/PersonSurveyAssignmentList.ascx" %>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <style type="text/css">
        .igtab_THContent {
            overflow: auto !important;
        }
    </style>
    <script type="text/javascript">
        function SelectedIndexChanged(sender, args) {
            PageMethods.SetSelectedTab(sender.getTabAt(args.get_tabIndex()).get_key());
        }
    </script>
    <controls:StateChecker runat="server" ID="stateChecker" ShowBeforeUnloadWarning="True" />
    <controls:Dialog ID="dialogControl" runat="server" HideButtons="true" PutActionButtonsInsideGridIfPossible="False" HideHeader="True">
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
                                </div>
                            </div>
                            <div class="tab-content__wrapper  flex-panel" style="flex-wrap: wrap;">
                            <table class="settings-table settings-table--default-columns settings-table--no-min-width">
                                <tr>
                                    <td><%=Strings.AgentNamePrefix%>				                    
                                        <controls:TextFieldValidator ID="tfxvAgentNamePrefix" ControlToValidate="tbxAgentNamePrefix"
                                                                     IsRequired="true" FieldRequredErrorMessage="Err_AgentNamePrefix"
                                            ValidationErrorMessage="ErrorIncorrectValue" Text="*" runat="server" />
                                    </td>				                    
                                    <td>
                                        <Controls:TextBox ID="tbxAgentNamePrefix" runat="server" MaxLength="255" />
                                    </td>
                                </tr>
                                <tr runat="server" ID="dialTypeRow" visible="false">
                                    <td nowrap>
                                        <%=Strings.DialTypeName%>
                                    </td>
                                    <td>
                                        <controls:DialTypeDropDownList ID="ddlDialType" runat="server" AddNoChangeOption="False"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <%=Strings.NumberOfAgentsToCreate%>				                    
                                    </td>				                    
                                    <td>
                                        <controls:NumericEdit runat="server" ID="neNumberOfAgentsToCreate" MinValue="1" ValueText="1"
                                                              Nullable="False">
                                        </controls:NumericEdit>
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
                            <asp:CustomValidator ID="GroupSelectedValidator" ClientValidationFunction="ValidateParentGroupSelected" runat="server" 
                                                 CssClass="validation-error" Text="*" ErrorMessage="<%$CPResource:UnableToAddPersonOutOfGroup%>"/>
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
    </script>
</asp:Content>
