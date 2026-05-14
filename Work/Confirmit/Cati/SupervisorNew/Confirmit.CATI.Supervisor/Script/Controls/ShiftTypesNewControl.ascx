<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShiftTypesNewControl.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Script.Controls.ShiftTypesNewControl" %>

<Controls:UpdatePanel ID="updatePanel" runat="server" style="height: 100%">
    <contenttemplate>
        <controls:Grid ID="m_grid" runat="server" HideSelectedColumn="true" HideResetButton="True" GridName="Shift types"
                       PrimaryKeyColumn="Id" EnablePaging="False" ShowFullToolbarBorders="False" OnDblClickCommand="Edit" HasMultySelectionCheckBox="False" HideRefreshButton="True" EnableSorting="False" SortedColumnKey="Id">
            <commands>
            
             <Controls:OverlayCommand Key="NewExclusion" Caption="New exclusion" Title="New exclusion" SelectMode="No" Image="add_circle" InlineParams="IsExclusion=true" RefreshOwner="True" Width="330" Height="170" DialogMode="Create" Url="Script/ScriptProperties/ScriptShiftTypeProperties.aspx" OnServerClick="OnChange" />
             <Controls:OverlayCommand Key="New" Caption="New" Title="New Shift Type" SelectMode="No" Image="plus" RefreshOwner="True" Width="330" Height="170" DialogMode="Create" Url="Script/ScriptProperties/ScriptShiftTypeProperties.aspx" OnServerClick="OnChange" />
             <Controls:OverlayCommand Key="Edit" Caption="Edit" Title="Edit" SelectMode="SingleRow" Image="settings" RefreshOwner="True" Width="330" Height="170" DialogMode="ViewEdit" IDName="ShiftTypeId" IDColumnName="Id" Url="Script/ScriptProperties/ScriptShiftTypeProperties.aspx" OnServerClick="OnChange"  />
                
            <Controls:Command Key="Delete" Caption="Delete" SelectMode="SingleRow" Image="delete" OnServerClick="Delete" Confirmation="cnfr_ShiftTypeDelete" />
            <Controls:Command Key="Launch" Caption="SaveAndLaunch" Image="play_circle" OnServerClick = "ScheduleLaunchHandler"  />   	
            <Controls:Command Key="Save" Caption="Save" Image="save" OnServerClick = "ScheduleSaveHandler" />   	
                
        </commands>
        <toolbaritems>
            <Controls:ToolbarCommandButton Key="NewExclusion"  />
            <Controls:ToolbarCommandButton Key="New"  />
            <Controls:ToolbarCommandButton Key="Edit" />
            <Controls:ToolbarCommandButton Key="Delete"  />
            <Controls:XpMenuItem ID="XpMenuItem1" runat="server" ButtonType="Separator"/>
            <controls:ToolbarCommandButton Key="Launch" />
            <Controls:ToolbarCommandButton Key="Save" ID="btnSave" runat="server" /> 	
        </toolbaritems>
        <DataMenuItems>	
            <Controls:DataMenuItem Key="New"/>
            <Controls:DataMenuItem Key="Edit"/>
            <Controls:DataMenuItem Key="Delete"/>
        </DataMenuItems>
		 <Columns>
            <controls:GeneralGridColumn Key="Id" DataFieldName="Id"  Header-Text="<%$CPResource:ID%>" Width="35px"  />
            <controls:GeneralGridColumn Key="Name" DataFieldName="Name" Header-Text="<%$CPResource:Shift Type Name%>" Width = "160px" />
            <controls:GeneralGridColumn Key="IsExclusion" DataFieldName="IsExclusion" Header-Text="" Width="30" Hidden="true"  />
            <controls:GeneralGridColumn Key="ColorName" DataFieldName="ColorName" Header-Text="<%$CPResource:Color Name%>" Width="140px"/>

            <iggrid:TemplateDataField Key="Color" Header-Text="<%$CPResource:Color%>"  Width="100px"  >
				<ItemTemplate>
				  <asp:Panel runat="server" ID="pnlColor" Width="100%" Height="25px" />
				</ItemTemplate>
			</iggrid:TemplateDataField>
            <iggrid:UnboundField Key="Empty" Header-Text="" Width="100%" />
        </Columns>
            
        </controls:Grid>
    </contenttemplate>
</Controls:UpdatePanel>
<asp:PlaceHolder runat="server" ID="placeholder"></asp:PlaceHolder>