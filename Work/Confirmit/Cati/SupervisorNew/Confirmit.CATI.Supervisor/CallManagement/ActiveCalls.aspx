<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="ActiveCalls.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.ActiveCalls" %>

<%@ Register Src="~/Controls/HierarchicalGridEx.ascx" TagName="HierarchicalGridEx" TagPrefix="controls" %>


<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <main class="activity-view-panel">
        <div class="activityViewHeader">
            <div class="activity-view-header">
                <div class="activity-view-header__title">
                    <h2><%=Title %></h2>
                </div>
                <div class="activity-view-header__actions">
                    <controls:XpMenu runat="server">
                        <controls:XpMenuItem ID="btnRefresh" runat="server" AutoPostBack="false" ButtonType="Button"
                            ImageName="refresh" OnClientClick="Common.updatePanel(statusPanelId);" Text="<%$CPResource:Refresh%>"
                            >
                        </controls:XpMenuItem>
                        <controls:XpMenuItem ID="btnClose" runat="server" ButtonType="Button" ImageName="close"
                            OnClientClick="top.overlay.closeLast()" Text="<%$CPResource:Close%>" >
                        </controls:XpMenuItem>
                    </controls:XpMenu>
                </div>
            </div>
        </div>
        <div class="activityViewBody flex-panel--all-awailable-space">

            <asp:UpdatePanel ID="updatePanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Always">

                <ContentTemplate>
                    <controls:ScrollableDiv ID="ScrollableDiv1" runat="server">
                        <controls:HierarchicalGridEx GridLines="Both" ID="m_grid" runat="server" CssClass="gridview"
                            DataKeyNames="Name"
                            HideToggleColumn="true" OnRowDataBound="gridSurveys_OnRowDataBound" AutoGenerateColumns="False" RenderHierarchicalRows="false">
                            <HeaderStyle CssClass="header" Wrap="false" />
                            <RowStyle CssClass="row" />
                            <AlternatingRowStyle CssClass="altrow" />
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="Group/User name" SortExpression="Name" />
                                <asp:TemplateField HeaderText="<%$CPResource:Count%>">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCount" runat="server" ClientIDMode="Static" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </controls:HierarchicalGridEx>
                    </controls:ScrollableDiv>
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>
        <div class="activityViewFooter">
            <asp:UpdatePanel ID="statusBarUpdatePanel" runat="server" ChildrenAsTriggers="true"
                UpdateMode="Always">
                <Triggers>
                </Triggers>
                <ContentTemplate>
                    <div class="ActivityListStatusBar">

                        <div>
                            <asp:Label ID="lblTime" runat="server" />
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </main>
</asp:Content>

