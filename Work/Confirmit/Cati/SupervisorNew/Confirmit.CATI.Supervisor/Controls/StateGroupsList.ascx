<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StateGroupsList.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Controls.StateGroupsList" %>
<%@ Register TagPrefix="controls" TagName="Grid" Src="~/Controls/GeneralGrid.ascx" %>

<script type="text/javascript">
    function openScriptInfoFrame(id) {
        Y.on("domready", function () {
            openAndSetInfoFrame('<%=BaseRelativePath("Resources/States.aspx")%>?ID=' + id);
        });
    }
</script>

<controls:Grid ID="grid" runat="server" HideSelectedColumn="true" GridName="<%$CPResource:ExtendedStatusCodes%>"
    OnDblClickCommand="View" PrimaryKeyColumn="ID" AutoGenerateColumns="false" ShowFullToolbarBorders="False">
    <Commands>
        <controls:ViewCommand Key="View" Caption="View" URL="Resources/States.aspx" Image="view" />
        <controls:OverlayCommand Key="New" RefreshListFrame="true" SelectMode="No" Caption="New" Title="NewStateGroup" IDColumnName="ID" Url="Resources/StateGroupProperties.aspx" Height="150" Width="400" Image="plus" RefreshOwner="True"/>
        <controls:OverlayCommand Key="Duplicate" RefreshListFrame="true" RefreshOwner="True" SelectMode="SingleRow" Caption="Duplicate" Title="DuplicateStateGroup" IDName="CopyID" IDColumnName="ID" Url="Resources/StateGroupProperties.aspx" Height="150" Width="400" Image="content_copy" />
        <controls:Command Key="CopyToDefaultGroup" SelectMode="SingleRow" Caption="CopyToDefaultStateGroupCaption" OnServerClick="CopyCustomGroupToDefault" Image="file-replace-outline"/>
        <controls:Command Key="Delete" SelectMode="SingleRow" Caption="Delete" OnServerClick="DeleteStateGroup" Confirmation="cnfr_DelStateGroup" Image="delete" />
        <controls:Command Key="Export" SelectMode="SingleRow" Caption="Export" OnServerClick="ExportStateGroup" Image="export" />
    </Commands>
    <ToolbarItems>
        <controls:ToolbarCommandButton Key="View" />
        <controls:ToolbarCommandButton Key="New" />
        <controls:ToolbarCommandButton Key="Duplicate" />
        <controls:ToolbarCommandButton Key="Delete" />
        <controls:XpMenuItem runat="server" ButtonType="Separator" />
        <controls:ToolbarCommandButton Key="Export" />
        <controls:XpMenuItem runat="server" ImageName="publish" ToolTip="Import" OnClientClick="return false;" ID="btnImport" />
    </ToolbarItems>
    <DataMenuItems>
        <controls:DataMenuItem Key="View" />
        <controls:DataMenuItem Key="New" />
        <controls:DataMenuItem Key="Duplicate" />
        <controls:DataMenuItem Key="CopyToDefaultGroup" />
        <controls:DataMenuItem Key="Delete" />
        <controls:DataMenuItem IsSeparator="True" />
        <controls:DataMenuItem Key="Export" />
    </DataMenuItems>
    <Columns>
        <controls:GeneralGridColumn HeaderText="ID" Key="ID" DataFieldName="ID" Width="75px" SearchColumnType="Number" />
        <controls:GeneralGridColumn HeaderText="Name" Key="Name" DataFieldName="Name" SearchColumnType="Text"
            Width="100%" />
    </Columns>
</controls:Grid>
<controls:PopupExtender ID="peImport" runat="server" MasterID="btnImport" SlaveID="pnlFileLoad" />
<asp:Panel ID="pnlFileLoad" runat="server" CssClass="popup-extender-container">
    <div class="popup-extender-panel">
        <div class="popup-selector">
            <div class="popup-selector__content flex-panel flex-panel-column">
                <asp:Label ID="Label2" runat="server" Text="<%$CPResource:SelectFileForImport%>">
                </asp:Label>
                <asp:FileUpload CssClass="plain_textbox" ID="FileLoad" runat="server" Width="280px" />
            </div>
            <div class="popup-selector__controls">
                <controls:UpdatePanel ID="upFileLoad" runat="server">
                    <%-- FileLoad must trigger full postback of MasterPage update panel  --%>
                    <ContentTemplate>
                        <controls:Button ID="btnFileLoad" runat="server" IsSubmit="true" OnClick="ImportStateGroup"
                            Text="Load" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnFileLoad" runat="server" />
                    </Triggers>
                </controls:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Panel>
