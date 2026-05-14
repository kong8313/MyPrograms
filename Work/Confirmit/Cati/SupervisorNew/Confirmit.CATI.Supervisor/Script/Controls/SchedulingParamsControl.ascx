<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchedulingParamsControl.ascx.cs"
	Inherits="Confirmit.CATI.Supervisor.Script.Controls.SchedulingParamsControl" %>

<Controls:UpdatePanel ID="updatePanel" runat="server" style="height: 100%">
    <contenttemplate>
        <controls:Grid ID="m_grid" runat="server" HideSelectedColumn="true" HideResetButton="True" SortedColumnName="Id" GridName="Parameters"
                       PrimaryKeyColumn="Id" EnablePaging="False" ShowFullToolbarBorders="False" OnDblClickCommand="Edit" HasMultySelectionCheckBox="False" HideRefreshButton="True" EnableSorting="False" >
            <commands>
                <Controls:OverlayCommand Key="New" Caption="New" Title="New Parameter" SelectMode="No" Image="plus" RefreshOwner="True" Width="430" Height="270" DialogMode="Create" Url="Script/ScriptProperties/ScriptParameterProperties.aspx" OnServerClick="OnChange" />
                <Controls:OverlayCommand Key="Edit" Caption="Edit" Title="Edit Parameter" SelectMode="SingleRow" Image="settings" RefreshOwner="True" Width="430" Height="270" DialogMode="ViewEdit" IDName="ParameterId" IDColumnName="Id" Url="Script/ScriptProperties/ScriptParameterProperties.aspx" OnServerClick="OnChange"  />
                <Controls:Command Key="Delete" Caption="Delete" SelectMode="SingleRow" Image="delete" Confirmation="cnfr_ParamDelete" OnServerClick = "Delete"/>
                <Controls:Command Key="Launch" Caption="SaveAndLaunch" Image="play_circle" OnServerClick = "ScheduleLaunchHandler"  />   	
                <Controls:Command Key="Save" Caption="Save" Image="save" OnServerClick = "ScheduleSaveHandler" />   	
            </commands>
            <toolbaritems>
                <Controls:ToolbarCommandButton Key="New"  />
                <Controls:ToolbarCommandButton Key="Edit" />
                <Controls:ToolbarCommandButton Key="Delete"  />
                <controls:XpMenuItem runat="server" ButtonType="Separator"/>
                <controls:ToolbarCommandButton Key="Launch" />
                <Controls:ToolbarCommandButton Key="Save" ID="btnSave" runat="server" /> 	
            </toolbaritems>
   
            <DataMenuItems>	
                <Controls:DataMenuItem Key="New"/>
                <Controls:DataMenuItem Key="Edit"/>
                <Controls:DataMenuItem Key="Delete"/>
            </DataMenuItems>
            <Columns>
                <controls:GeneralGridColumn DataFieldName="Id" HeaderTextId="ID" Key="Id" Width="35px" />
                <controls:GeneralGridColumn DataFieldName="Name" HeaderTextId="Name" Key="Name" Width="160px" />
                <controls:UnboundGeneralGridColumn  Header-Text="<%$CPResource:ParamType%>" Key="TypeName" Width="160px"/>
                <controls:GeneralGridColumn DataFieldName="Type" Key="Type" Hidden="True" />
                <controls:GeneralGridColumn DataFieldName="DefaultValue" HeaderTextId="DefaultValue" Key="DefaultValue" Width="160px" />
                <controls:GeneralGridColumn DataFieldName="Description" HeaderTextId="Description" Key="Description" Width="100%" />
            </Columns>
        </controls:Grid>
    </contenttemplate>
</Controls:UpdatePanel>
<asp:PlaceHolder runat="server" ID="placeholder"></asp:PlaceHolder>
