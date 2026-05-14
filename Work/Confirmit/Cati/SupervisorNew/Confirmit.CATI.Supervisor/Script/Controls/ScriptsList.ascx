<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="ScriptsList.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Script.Controls.ScriptsList" %>

<script type="text/javascript">
    function openScriptInfoFrame(id) {
        Y.on("domready", function () {
            openAndSetInfoFrame('<%=BaseRelativePath("Script/ScriptView.aspx")%>?ID=' + id);
        });
    }
    function showSchedulingErrorsList() {
        var gridController = <%=Scripts.ClientControllerName%>;
            var row = gridController.GetSelectedRow();

            if (row) {
                var scriptId = row.get_cellByColumnKey('ID').get_value();
                top.showSchedulingErrorsList(scriptId);
            }
        }
</script>

<controls:Grid ID="Scripts" runat="server" OnDblClickCommand="Properties" PrimaryKeyColumn="ID" ShowFullToolbarBorders="False" GridName="Scheduling scripts">
    <Commands>
        <controls:OverlayCommand Key="New" Caption="NewScript" Title="NewScript" Url="Script/ScriptProperties.aspx"
            Image="plus" RefreshListFrame="true" Height="150" Width="450" DialogMode="Create" RefreshOwner="True"
            SelectMode="No" />
        <controls:OverlayCommand Key="Properties" Caption="Properties" Url="Script/ScriptProperties.aspx" Image="spanner" RefreshInfoFrame="True"
            DialogMode="ViewEdit" IDName="ID" IDColumnName="ID" RefreshOwner="True" RefreshListFrame="true" Title="ScriptProperties" Height="150" Width="450" />
        <controls:ViewCommand Key="View" Caption="View" URL="Script/ScriptView.aspx" Image="view" />
        <controls:Command Key="Duplicate" Caption="Duplicate" Image="content_copy" OnServerClick="DuplicateScript" />
        <controls:Command Key="CopySchedulingScriptToDefault" Caption="CopyToDefaultSchedulingScriptCaption" SelectMode="SingleRow" Image="file-replace-outline" OnServerClick="CopySchedulingScriptToDefault"/>
        <controls:Command Key="Delete" Caption="Delete" SelectMode="MultiRow" OnServerClick="DeleteScript" Image="delete" Confirmation="cnfr_DelScript" />
        <controls:Command Key="Export" Caption="Export" SelectMode="MultiRow" OnServerClick="ExportScript" Image="export" />
        <controls:Command Key="ErrorsList" Caption="ErrorsList" SelectMode="SingleRow" Image="error" OnClientClick="showSchedulingErrorsList()"/>

    </Commands>
    <ToolbarItems>
        <controls:ToolbarCommandButton Key="New" />
        <controls:ToolbarCommandButton Key="Properties" />
        <controls:ToolbarCommandButton Key="Delete" />
        <controls:ToolbarCommandButton Key="View" />
        <controls:XpMenuItem runat="server" ButtonType="Separator" />
        <controls:ToolbarCommandButton Key="Export" />
        <controls:XpMenuItem runat="server" ImageName="publish" ToolTip="Import" OnClientClick="return false;" ID="btnImport" />

    </ToolbarItems>
    <DataMenuItems>
        <controls:DataMenuItem Key="New" />
        <controls:DataMenuItem Key="Duplicate" />
        <Controls:DataMenuItem Key="CopySchedulingScriptToDefault"/>
        <controls:DataMenuItem Key="Delete" />
        <controls:DataMenuItem Key="View" />
        <controls:DataMenuItem Key="Properties" />
        <controls:DataMenuItem Key="ErrorsList" />
        <controls:DataMenuItem IsSeparator="True" />
        <controls:DataMenuItem Key="Export" />
       
    </DataMenuItems>
    <Columns>
        <controls:GeneralGridColumn HeaderText="ID" SearchColumnType="Number" Key="ID" DataFieldName="SID"
            Width="50" Hidden="True" />
        <controls:GeneralGridColumn HeaderText="Name" SearchColumnType="Text" Key="Name" DataFieldName="Name"
            Width="70%" />
        <controls:GeneralGridColumn HeaderTextId="ScriptsList_ExtendedStatusGroupColumn_Caption" SearchColumnType="Text" Key="DesignStateGroupName" SearchColumnName="DesignStateGroupName"
            Width="30%" />
        <controls:UnboundGeneralGridColumn Header-Text="State" SearchColumnType="DropDown" Key="State" SearchColumnName="State"
            Width="145" />
        <controls:GeneralGridColumn HeaderText="Created" SearchColumnType="DateTime" Key="CreateDate" DataFieldName="CreateDate"
            Width="150" />
        <controls:GeneralGridColumn HeaderText="Modified" SearchColumnType="DateTime" Key="ModifyDate" DataFieldName="ModifyDate"
            Width="150" />
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
                        <controls:Button ID="btnFileLoad" runat="server" IsSubmit="true" OnClick="ImportScript"
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
