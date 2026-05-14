<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SurveysAssignments.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.SurveysAssignments" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <script type="text/javascript">
        function ShowDiv(GridName) {
            $get("divSurveysGrid").style.display = "none";
            $get("divUsersGrid").style.display = "none";
            if (GridName == 'SurveysGrid')
                $get("divSurveysGrid").style.display = "";
            if (GridName == 'UsersGrid')
                $get("divUsersGrid").style.display = "";
        }

        function surveysTree_DragStart(oTree, oNode, oDataTransfer, oEvent) {
            oNode.setChecked(true, false);
            oDataTransfer.dataTransfer.setData("Text", oNode.getDataKey());
            oDataTransfer.dataTransfer.effectAllowed = "move";
        }

        function personsTree_DragStart(oTree, oNode, oDataTransfer, oEvent) {
            oNode.setChecked(true, false);
            oDataTransfer.dataTransfer.setData("Text", oNode.getDataPath());
            oDataTransfer.dataTransfer.effectAllowed = "move";
        }

        function showHelp() {
            window.open('<%=BaseRelativePath("HelpPages/SurveysAssignments.html") %>', '1', 'toolbar=no,status=no,resizable,width=600,height=600');
        }

        function resizeTreeScript(clientPersonTreeId, clientSurveyTreeId, gridId) {
            var script = function () {
                var el = document.getElementById(clientPersonTreeId);
                el.style.height = (document.body.clientHeight - Y.one('#' + clientPersonTreeId).getY() - 40) + "px";

                el = document.getElementById(clientSurveyTreeId);
                el.style.height = (document.body.clientHeight - Y.one('#' + clientSurveyTreeId).getY() - 40) + "px";

                el = document.getElementById(gridId);
                el.style.height = (document.body.clientHeight - Y.one('#' + gridId).getY() - 40) + "px";
            };
            window.onresize = script;
            script();
        }
    </script>
    <div class="main-wrapper">
        <main class="content-panel">
            <controls:UpdatePanel ID="updatePanelMenu" runat="server" Width="100%" Height="100%">
                <ContentTemplate>
                    <div class="leftMenu">
                        <controls:XpMenu ID="menu" runat="server" CssClass="leftMenu">
                            <controls:XpMenuItem ID="btnToolBarAssign" runat="server" ButtonType="Button" ImageName="enable.gif"
                                OnClick="btnAssign_Click" Text="<%$CPResource:Assign%>" TextAndImage="true">
                            </controls:XpMenuItem>
                            <controls:XpMenuItem ID="btnToolBarDeassign" runat="server" ButtonType="Button" ImageName="disable.gif"
                                OnClick="btnDeassign_Click" Text="<%$CPResource:Deassign%>" TextAndImage="true">
                            </controls:XpMenuItem>
                            <controls:XpMenuItem ID="btnToolBarReset" runat="server" ButtonType="Button" ImageName="reset"
                                OnClick="btnReset_Click" Text="<%$CPResource:Reset%>" TextAndImage="true">
                            </controls:XpMenuItem>
                            <controls:XpMenuItem ID="btnToolBarRefresh" runat="server" ButtonType="Button" ImageName="refresh"
                                OnClientClick="window.location.href+='';" Text="<%$CPResource:Refresh%>" TextAndImage="true">
                            </controls:XpMenuItem>
                            <controls:XpMenuItem ID="btnToolBarHelp" runat="server" ButtonType="Button" ImageName="help"
                                OnClientClick="showHelp();" Text="<%$CPResource:Help%>" TextAndImage="true">
                            </controls:XpMenuItem>
                            <controls:XpMenuItem ID="btnToolBarClose" runat="server" ButtonType="Button" ImageName="close_win.gif"
                                OnClientClick="window.close();" Text="<%$CPResource:Close%>" TextAndImage="true">
                            </controls:XpMenuItem>
                        </controls:XpMenu>
                    </div>
                </ContentTemplate>
            </controls:UpdatePanel>
            <table id="Table1" cellspacing="0" cellpadding="0" width="100%" style="table-layout: fixed;"
                runat="server">
                <tr>
                    <td style="width: 30%;">
                        <h3>
                            <asp:Literal ID="ltrUsers" runat="server" Text="<%$CPResource:Interviewers%>" /></h3>
                    </td>
                    <td>
                        <controls:UpdatePanel ID="warpLabel" runat="server" Height="100%">
                            <ContentTemplate>
                                <h3>
                                    <asp:Literal ID="lbGridName" runat="server" Text="<%$CPResource:InterviewersSurveys%>"></asp:Literal></h3>
                            </ContentTemplate>
                        </controls:UpdatePanel>
                    </td>
                    <td style="width: 30%;">
                        <h3>
                            <asp:Literal ID="ltrSurveys" runat="server" Text="<%$CPResource:Surveys%>" /></h3>
                    </td>
                </tr>
                <tr>
                    <td valign="top" align="left" style="width: 30%;">
                        <controls:UpdatePanel runat="server" ID="updatePanelPersons" Width="100%" Height="100%">
                            <ContentTemplate>
                                <controls:PersonsTreeWithAssignments ID="personsTree" runat="server" Height="400px" />
                            </ContentTemplate>
                        </controls:UpdatePanel>
                    </td>
                    <td valign="top" style="width: 100%; height: 100%">
                        <controls:UpdatePanel runat="server" ID="updatePanelGrid" style="position: relative; height: 100%">
                            <ContentTemplate>
                                <div class="flex-panel flex-panel-row flex-panel-row--justify filter-controls--with-margin">
                                    <controls:Button ID="btnAddUsers" runat="server" Text="  >>  " OnClick="btnAddUsers_Click" Style="position: absolute; left: 0px; top: 0px" />
                                    <div class="flex-panel flex-panel-row--justify filter-controls--with-padding">
                                        <controls:Button ID="btnAssign" runat="server" Text="Assign" OnClick="btnAssign_Click" />
                                        <controls:Button ID="btnDeassign" runat="server" Text="Deassign" OnClick="btnDeassign_Click" />
                                        <controls:Button ID="btnReset" runat="server" Text="Reset" OnClick="btnReset_Click" />
                                    </div>
                                    <controls:Button ID="btnAddSurveys" runat="server" Text=" << " OnClick="btnAddSurveys_Click" Style="position: absolute; right: 0px; top: 0px" />
                                </div>
                                <div style="position: absolute; top: 40px; bottom: 0px; width: 100%">
                                    <div id="divUsersGrid" style="display: block; height: 100%">
                                        <controls:Grid ID="UsersGrid" Visible="true" runat="server" PrimaryKeyColumn="SID"
                                            GridName="UsersList" EnablePaging="false" HideToolBar="true"
                                            HideSelectedColumn="true">
                                            <Commands>
                                                <controls:Command Key="Delete" Caption="DeleteSelected" Image="delete" OnServerClick="btnUserGridDeleteRows_Click"
                                                    Confirmation="DoYouWantToDeleteSelectedRow" SelectMode="SingleRow" />
                                            </Commands>
                                            <DataMenuItems>
                                                <controls:DataMenuItem Key="Delete" />
                                            </DataMenuItems>
                                            <Columns>
                                                <controls:GeneralGridColumn HeaderText="Id" Key="SID" Hidden="True" DataFieldName="Id"
                                                    Width="60" />
                                                <controls:GeneralGridColumn HeaderText="Name" Key="Name" DataFieldName="Name"
                                                    Width="200" />
                                                <controls:GeneralGridColumn HeaderText="Description" Key="Description"
                                                    DataFieldName="Description" Width="100%" />
                                            </Columns>
                                        </controls:Grid>
                                    </div>
                                    <div id="divSurveysGrid" style="display: none; height: 100%">
                                        <controls:Grid ID="SurveysGrid" Visible="true" runat="server" PrimaryKeyColumn="Id"
                                            GridName="SurveysList" EnablePaging="false" HideToolBar="true"
                                            HideSelectedColumn="true">
                                            <Commands>
                                                <controls:Command Key="Delete" Caption="DeleteSelected" Image="delete" OnServerClick="btnSurveyGridDeleteRows_Click"
                                                    Confirmation="DoYouWantToDeleteSelectedRow" SelectMode="SingleRow" />
                                            </Commands>
                                            <DataMenuItems>
                                                <controls:DataMenuItem Key="Delete" />
                                            </DataMenuItems>
                                            <Columns>
                                                <controls:GeneralGridColumn HeaderText="Id" Key="Id" Hidden="True" DataFieldName="Id"
                                                    Width="60" />
                                                <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectId%>" Key="ConfirmitID"
                                                    DataFieldName="ConfirmitID" />
                                                <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectName%>" Key="Name"
                                                    DataFieldName="Name" Width="200" />
                                            </Columns>
                                        </controls:Grid>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </controls:UpdatePanel>
                    </td>
                    <td valign="top" align="left" style="border-top: solid 1px #a0a0a0; border-left: solid 1px #a0a0a0; padding-top: 5px; width: 30%">
                        <controls:UpdatePanel runat="server" ID="updatePanelSurveys" Width="100%" Height="100%">
                            <ContentTemplate>
                                <controls:SurveysTreeWithAssignments ID="surveysTree" runat="server" Height="400px" />
                            </ContentTemplate>
                        </controls:UpdatePanel>
                    </td>
                </tr>
            </table>
        </main>
    </div>
</asp:Content>
