<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchedulingRulesNewControl.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Script.Controls.SchedulingRulesNewControl" %>
<%@ Register TagPrefix="Controls" TagName="HierarchicalGrid" Src="~/Controls/HierarchicalGridControl.ascx" %>

<controls:UpdatePanel ID="updatePanel" runat="server" style="height: 100%" UpdateMode="Always">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="m_grid" />
    </Triggers>

    <ContentTemplate>
        <controls:HierarchicalGrid ID="m_grid" runat="server" DataKeyFields="Id" OnDblClickCommand="Edit" ItemCssClass="hierarchical-first-level">
            <commands>
                <Controls:OverlayCommand Key="New" Caption="New" Title="New Rule" SelectMode="No" Image="plus" Width="450" Height="150" DialogMode="Create" Url="Script/ScriptProperties/ScriptRuleProperties.aspx" OnServerClick="OnChange" />
                <Controls:OverlayCommand Key="NewSubrule" Caption="New Subrule" Title="New Subrule" SelectMode="No" Image="plus" RefreshOwner="True" Width="700" Height="450" DialogMode="Create" Url="Script/ScriptProperties/ScriptSubruleProperties.aspx" OnServerClick="OnChange" />
                <Controls:OverlayCommand Key="NewAction" Caption="New Action" Title="New Action" SelectMode="No" Image="plus" RefreshOwner="True" Width="700" Height="400" DialogMode="Create" Url="Script/ScriptProperties/ScriptActionProperties.aspx" OnServerClick="OnChange" />
                <Controls:OverlayCommand Key="EditRule" Caption="Edit" Title="Edit" SelectMode="SingleRow" Image="edit" Width="450" Height="150" DialogMode="ViewEdit" IDName="RuleId" IDColumnName="Id" Url="Script/ScriptProperties/ScriptRuleProperties.aspx"  OnServerClick="OnChange" />
                <Controls:OverlayCommand Key="EditSubRule" Caption="Edit" Title="Edit" SelectMode="SingleRow" Image="edit" RefreshOwner="True" Width="700" Height="450" DialogMode="ViewEdit" IDName="SubRuleId" IDColumnName="Id" Url="Script/ScriptProperties/ScriptSubruleProperties.aspx" OnServerClick="OnChange"  />
                <Controls:OverlayCommand Key="EditAction" Caption="Edit" Title="Edit" SelectMode="SingleRow" Image="edit" RefreshOwner="True" Width="700" Height="400" DialogMode="ViewEdit" IDName="ActionId" IDColumnName="Id" Url="Script/ScriptProperties/ScriptActionProperties.aspx" OnServerClick="OnChange"  />
                <Controls:Command Key="Edit" Caption="Edit" SelectMode="SingleRow" Image="edit" OnClientClick="schedulingRulesController.editRule();" />
                <Controls:Command Key="Copy" Caption="Copy" Image="content_copy" OnClientClick="schedulingRulesController.copyRow();" />
                <Controls:Command Key="Paste" Caption="Paste" Image="paste" OnClientClick="schedulingRulesController.pasteRow();" />
                <Controls:Command Key="Delete" Caption="Delete" SelectMode="SingleRow" Image="delete" OnClientClick="schedulingRulesController.deleteRow();" Confirmation="cnfr_RuleRowDelete" />
                <Controls:Command Key="MoveUp" Caption="MoveUp" Image="expand_less" SelectMode="SingleRow" OnClientClick="schedulingRulesController.moveRow(true);" Confirmation="cnfr_MoveRowUp" />
                <Controls:Command Key="MoveDown" Caption="MoveDown" Image="expand_more" SelectMode="SingleRow" OnClientClick="schedulingRulesController.moveRow(false);" Confirmation="cnfr_MoveRowDown" />
                <Controls:Command Key="ExpandAll" Caption="Expand all" Image="expand_all" OnClientClick="schedulingRulesController.ExpandAll();" />
                <Controls:Command Key="CollapseAll" Caption="Collapse all" Image="collapse_all" OnClientClick="schedulingRulesController.CollapseAll();" />
                <Controls:Command Key="Enable" Caption="Enable" Image="activate" OnClientClick="schedulingRulesController.enableAction(true);" />
                <Controls:Command Key="Disable" Caption="Disable" Image="block" OnClientClick="schedulingRulesController.enableAction(false);" />
                <Controls:Command Key="Export" Caption="Export" Image="export" OnClientClick="schedulingRulesController.ExportClick()" />
                <Controls:Command Key="Launch" Caption="SaveAndLaunch" Image="play_circle" OnServerClick = "ScheduleLaunchHandler"  />   	
                <Controls:Command Key="Save" Caption="Save" Image="save" OnServerClick = "ScheduleSaveHandler" />   
                <Controls:Command Key="Refresh" Caption="Refresh" OnClientClick="window.location.href+='';" Image="refresh" />
                <Controls:Command Key="UpdateAndMarkAsChanged" OnServerClick="OnChange" />
            </commands>
            <toolbaritems>
                <controls:XpMenuItem runat="server" ImageName="find_in_page" ID="btnSearch" Text="Search ..." IsSubmit="False" />
                <Controls:ToolbarCommandButton Key="New" />
                <Controls:ToolbarCommandButton Key="Edit" />
                <Controls:ToolbarCommandButton Key="Paste" />
                <Controls:ToolbarCommandButton Key="Delete" />
                <Controls:XpMenuItem runat="server" ButtonType="Separator" />
                <Controls:ToolbarCommandButton Key="MoveUp" />
                <Controls:ToolbarCommandButton Key="MoveDown" />
                <Controls:ToolbarCommandButton Key="CollapseAll" />
                <Controls:ToolbarCommandButton Key="ExpandAll" />
                <Controls:XpMenuItem runat="server" ButtonType="Separator" />
                <Controls:ToolbarCommandButton Key="Export" />
                <Controls:XpMenuItem runat="server" ButtonType="Separator" />
                <Controls:ToolbarCommandButton Key="Launch" />
                <Controls:ToolbarCommandButton Key="Save" runat="server" ID="btnSave" />
            </toolbaritems>
            <datamenuitems>
                <Controls:DataMenuItem Key="New" />
                <Controls:DataMenuItem Key="NewSubrule" />
                <Controls:DataMenuItem Key="Edit" />
                <Controls:DataMenuItem Key="Delete" />
                <Controls:DataMenuItem Key="Copy" />
                <Controls:DataMenuItem Key="Paste" />
                <Controls:DataMenuItem IsSeparator="true" />
                <Controls:DataMenuItem Key="MoveUp" />
                <Controls:DataMenuItem Key="MoveDown" />
                <Controls:DataMenuItem IsSeparator="true" />
                <Controls:DataMenuItem Key="CollapseAll" />
                <Controls:DataMenuItem Key="ExpandAll" />
            </datamenuitems>
            <columns>
                <controls:GeneralGridColumn Key="Id" DataFieldName="Id" Header-Text="<%$CPResource:ID%>" Hidden="true" />
                <controls:UnboundGeneralGridColumn Key="Number" Header-Text="<%$CPResource:Rules%>" Width="150" />
                <controls:GeneralGridColumn Key="Description" DataFieldName="Description" Header-Text="<%$CPResource:Description%>"
                                            Width="100%" />  
                <controls:GeneralGridColumn Key="SampleUpdate" DataFieldName="SampleUpdate" Header-Text="<%$CPResource:SampleUpdate%>"
                                            Width="210" />  
            </columns>

            <bands>
                <Controls:GridBand Key="SubRules" DataMember="SubRules" DataKeyFields="Id" AutoGenerateColumns="false" ItemCssClass="hierarchical-second-level">
                    <columns>
                        <controls:GeneralGridColumn Key="Id" DataFieldName="Id" Header-Text="<%$CPResource:ID%>" Hidden="true" Width="0px" />
                        <controls:UnboundGeneralGridColumn  Key="Number" Header-Text="<%$CPResource:SubRules%>" Width="70px" />
                        <controls:GeneralGridColumn Key="Filter" DataFieldName="Filter" Header-Text="<%$CPResource:Filter%>" Width="320px" />				
                        <controls:GeneralGridColumn Key="ItsId" DataFieldName="ItsId" Header-Text="<%$CPResource:ExtendedStatusCode%>" Width="140px" />
                        <controls:UnboundGeneralGridColumn Key="ItsName"  Header-Text="<%$CPResource:ExtendedStatusName%>" Width="140px"/>
                        <controls:GeneralGridColumn Key="ShiftTypeId" DataFieldName="ShiftTypeId" Header-Text="<%$CPResource:Shift Type ID%>" Width="100px" />
                        <controls:UnboundGeneralGridColumn Key="ShiftTypeName"  Header-Text="<%$CPResource:ShiftType%>" Width="120px" />
                        <controls:GeneralGridColumn Key="Description" DataFieldName="Description" Header-Text="<%$CPResource:Description%>" Width="100%" />
                        <controls:GeneralGridColumn Key="FilterEnabled" DataFieldName="FilterEnabled" Header-Text="<%$CPResource:FilterEnabled%>" Hidden="true" />
                    </columns>
                    <DataMenuItems>
                        <controls:DataMenuItem Key="NewSubrule" Text="New"/>
                        <controls:DataMenuItem Key="NewAction"/>
                        <controls:DataMenuItem Key="EditSubRule"/>
                        <controls:DataMenuItem Key="Delete"/>
                        <controls:DataMenuItem Key="Copy"/>
                        <controls:DataMenuItem Key="Paste"/>
                        <controls:DataMenuItem IsSeparator="true"/>
                        <controls:DataMenuItem Key="MoveUp"/>
                        <controls:DataMenuItem Key="MoveDown"/>	
                        <controls:DataMenuItem IsSeparator="true"/>
                        <controls:DataMenuItem Key="CollapseAll"/>
                        <controls:DataMenuItem Key="ExpandAll"/>		
                    </DataMenuItems>
                    <bands>
                        <controls:GridBand Key="actions" DataMember="Actions" DataKeyFields="Id" AutoGenerateColumns="false"  ItemCssClass="hierarchical-third-level">
                            <Columns>
                                <controls:GeneralGridColumn Key="Id" DataFieldName="Id" Header-Text="<%$CPResource:Id%>" Hidden="true" />
                                <controls:UnboundGeneralGridColumn  Key="Number" Header-Text="<%$CPResource:Actions%>" Hidden="true" />						
                                <controls:GeneralGridColumn Key="Filter" DataFieldName="Filter" Header-Text="<%$CPResource:Filter%>" Width="320px" />						
                                <controls:UnboundGeneralGridColumn Key="ActionName" Header-Text="<%$CPResource:Action%>" Width="275"/>						
                                <controls:UnboundGeneralGridColumn Key="Parameter" Header-Text="<%$CPResource:Parameter%>" Width="100%"/>
                                <controls:GeneralGridColumn Key="Enabled" DataFieldName="Enabled" Hidden="true" />
                                <controls:GeneralGridColumn Key="FilterEnabled" DataFieldName="FilterEnabled" Hidden="true" />
                                <controls:GeneralGridColumn Key="ActionId" DataFieldName="ActionId" Hidden="true" />
                                <controls:GeneralGridColumn Key="ParameterValue" DataFieldName="ParameterValue" Hidden="true" />
                                <controls:GeneralGridColumn Key="IsSchedulingParameter" DataFieldName="IsSchedulingParameter" Hidden="true" />
                            </Columns>
                            <DataMenuItems>
                                <controls:DataMenuItem Key="NewAction" Text="New"/>
                                <controls:DataMenuItem Key="EditAction"/>
                                <controls:DataMenuItem Key="Delete"/>
                                <controls:DataMenuItem Key="Copy"/>
                                <controls:DataMenuItem Key="Paste"/>
                                <controls:DataMenuItem IsSeparator="true"/>
                                <controls:DataMenuItem Key="MoveUp"/>
                                <controls:DataMenuItem Key="MoveDown"/>						
                                <controls:DataMenuItem IsSeparator="true"/>
                                <controls:DataMenuItem Key="Enable"/>
                                <controls:DataMenuItem Key="Disable"/>
                            </DataMenuItems>
                        </controls:GridBand>
                    </bands>
                </Controls:GridBand>
            </bands>
        </controls:HierarchicalGrid>
        <div style="display: none">
            <asp:Button ID="btnExport" runat="Server" OnClick="ScheduleExport" UseSubmitBehavior="false" />
            <asp:HiddenField runat="server" ID="hfCopiedRowKey" Value="" />
        </div>

    </ContentTemplate>

</controls:UpdatePanel>

<asp:PlaceHolder runat="server" ID="placeholder"></asp:PlaceHolder>

<controls:PopupExtender ID="peSearch" MasterID="btnSearch" SlaveID="pnlSearch" runat="server" />
<asp:Panel ID="pnlSearch" runat="server" CssClass="popup-extender-container">
    <div class="popup-selector">
        <div class="popup-selector__content">
            <table cellspacing="5" cellpadding="0">
                <tr align="left">
                    <td nowrap width="90">
                        <asp:Label ID="lblSearchType" Text="<%$CPResource:Search type:%>" runat="server"
                            Font-Bold="true" />
                    </td>
                    <td>
                        <controls:DropDownList ID="ddlSearch" runat="server" Width="140px">
                            <asp:ListItem Text="<%$CPResource:Any%>" Value="" />
                            <asp:ListItem Text="<%$CPResource:Filter%>" Value="Filter" />
                            <asp:ListItem Text="<%$CPResource:ExtendedStatusCode%>" Value="ItsId" />
                            <asp:ListItem Text="<%$CPResource:ExtendedStatusName%>" Value="ItsName" />
                            <asp:ListItem Text="<%$CPResource:ShiftType%>" Value="ShiftTypeName" />
                            <asp:ListItem Text="<%$CPResource:Description%>" Value="Description" />
                            <asp:ListItem Text="<%$CPResource:Action%>" Value="ActionName" />
                            <asp:ListItem Text="<%$CPResource:Parameter%>" Value="ParameterValue" />
                        </controls:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSearchText" runat="server" Text="Search text:" Font-Bold="true" />
                    </td>
                    <td>
                        <controls:TextBox ID="tbSearch" runat="server" CssClass="plain_textbox" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="popup-selector__controls">
            <input class="plain_button button-cancel" type="button" id="btnCancel" value="Cancel" onclick="hidePopup();" />
            <input class="plain_button" type="button" id="btnFindNext" value="Find next" onclick="schedulingRulesController.findNext();" />

        </div>
    </div>
</asp:Panel>
