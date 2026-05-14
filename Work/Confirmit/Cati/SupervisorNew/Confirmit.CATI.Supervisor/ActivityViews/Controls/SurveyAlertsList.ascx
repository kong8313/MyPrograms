<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyAlertsList.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.ActivityViews.Controls.SurveyAlertsList" %>
<div class="popup-extender-panel">
    <div class="popup-selector">
        <div class="popup-selector__content">
            <controls:DropDownList ID="ddlAlert" runat="server" />
            <div class="flex-panel flex-panel-row">
                <div class="flex-panel flex-panel-row row-with-inputs">
                    <asp:Label ID="lblAmberThreshold" Text="<%$CPResource:AmberThresholdSec%>" runat="server" />
                    <controls:TextBox ID="tbxAmberThreshold" runat="server" Width="30px" />
                    <asp:Label ID="lblRedThreshold" Text="<%$CPResource:RedThresholdSec%>" runat="server" />
                    <controls:TextBox ID="tbxRedThreshold" runat="server" Width="30px" />
                    <controls:Button ID="btnSetAlert" runat="server" Text="<%$CPResource:Set%>" OnClick="btnSetAlert_Click" />
                </div>
            </div>
        </div>
        <asp:UpdatePanel ID="updatePanelAlerts" runat="server" ChildrenAsTriggers="true" class="popup-selector__controls"
            UpdateMode="Always">
            <ContentTemplate>
                <asp:GridView ID="innerGrid" runat="server" DataKeyNames="ObjectSID,ThresholdsTypeId"  CssClass="generic-grid"
                    AutoGenerateColumns="false" Width="100%" AllowSorting="false" OnRowDeleting="innerGrid_RowDeleting">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <controls:ImageButton ID="ibDelete" runat="server" Text=""
                                    CommandName="Delete" CausesValidation="false" ImageName="close" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ObjectSID" Visible="false" />
                        <asp:BoundField DataField="ThresholdsTypeId" Visible="false" />
                        <asp:BoundField DataField="ColumnName" HeaderText="<%$CPResource:AlertType%>" />
                        <asp:BoundField DataField="Amber" HeaderText="<%$CPResource:AmberThreshold%>" />
                        <asp:BoundField DataField="Red" HeaderText="<%$CPResource:RedThreshold%>" />
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>

