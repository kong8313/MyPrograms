<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StatusAlertsList.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.ActivityViews.Controls.StatusAlertsList" %>
    <div class="popup-extender-panel">
        <div class="popup-selector">
            <div class="popup-selector__content">
                <controls:DropDownList ID="ddlAlert" runat="server" />
                <div class="flex-panel flex-panel-row row-with-inputs">
                    <asp:Label ID="lblAmberThreshold" Text="<%$CPResource:AmberThreshold%>" runat="server" />
                    <controls:TextBox ID="tbxAmberThreshold" runat="server" Width="50px" />
                    <asp:Label ID="lblRedThreshold" Text="<%$CPResource:RedThreshold%>" runat="server" />
                    <controls:TextBox ID="tbxRedThreshold" runat="server" Width="50px" />
                    <controls:Button ID="btnSetAlert" runat="server" Text="<%$CPResource:Set%>" OnClick="btnSetAlert_Click" />
                </div>
            </div>
            <asp:UpdatePanel ID="updatePanelStatusAlerts" runat="server" ChildrenAsTriggers="true" class="popup-selector__controls"
                UpdateMode="Always">
                <ContentTemplate>
                    <asp:GridView ID="innerGrid" runat="server" DataKeyNames="ObjectSID,StatusId" AutoGenerateColumns="false" CssClass="generic-grid"
                        Width="100%" AllowSorting="false" OnRowDeleting="innerGrid_RowDeleting">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <controls:ImageButton ID="ibDelete" runat="server" AlternateText=""
                                        CommandName="Delete" CausesValidation="false" ImageName="close" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ObjectSID" Visible="false" />
                            <asp:BoundField DataField="StatusId" Visible="false" />
                            <asp:BoundField DataField="StatusName" HeaderText="<%$CPResource:AlertType%>" />
                            <asp:BoundField DataField="Amber" HeaderText="<%$CPResource:AmberThreshold%>" />
                            <asp:BoundField DataField="Red" HeaderText="<%$CPResource:RedThreshold%>" />
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>
    </div>

