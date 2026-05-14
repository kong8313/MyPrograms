<%@ Page Language="C#" MasterPageFile="~/MasterPages/RightFrameWithInfo.master" AutoEventWireup="true" CodeBehind="TimezonesList.aspx.cs" 
Inherits="Confirmit.CATI.Supervisor.Resources.TimezonesList" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        .LocalTimezone {
            font-weight: 600;
        }
    </style>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="RightFrameContent" runat="server">
    <controls:Grid ID="grid" runat="server" HideSelectedColumn="true" GridName="<%$CPResource:Timezones%>"
                   PrimaryKeyColumn="Id" EnablePaging="false" OnDblClickCommand="ShowCustomTimezones" ShowFullToolbarBorders="False">
        <commands>
		    <controls:Command Key="Activate" SelectMode="SingleRow" Caption="Activate" Image="checked_green" OnServerClick="ActivateTimezone" />
            
            <controls:Command Key="Deactivate" SelectMode="SingleRow" Caption="Deactivate" Image="block" OnServerClick="DeactivateTimezone" />
            <controls:Command Key="DeactivateUnused" SelectMode="No" Caption="DeactivateUnused" Image="delete_outline" OnServerClick="DeactivateUnused" />
            <controls:Command Key="SetAsLocal" SelectMode="SingleRow" Caption="SetAsLocal" Image="activate" OnServerClick="SetLocalTimezone" />
            <controls:ViewCommand Key="ShowCustomTimezones" Caption="ShowCustomTimezones" IDColumnName="Id"
                                  IDName="Id" URL="Resources/CustomTimezonesView.aspx" Image="view" />
	    </commands>
        <toolbaritems>
            <controls:ToolbarCommandButton Key="Activate" />
            
            <controls:ToolbarCommandButton Key="Deactivate" />
            <controls:ToolbarCommandButton Key="DeactivateUnused" />
            <controls:ToolbarCommandButton Key="SetAsLocal" />
            <controls:ToolbarCommandButton Key="ShowCustomTimezones" />
	    </toolbaritems>
        <DataMenuItems>
		    <controls:DataMenuItem Key="Activate" />
            
            <controls:DataMenuItem Key="Deactivate" />
            <controls:DataMenuItem Key="DeactivateUnused" />
            <controls:DataMenuItem Key="SetAsLocal" />
            <controls:DataMenuItem Key="ShowCustomTimezones" />
	    </DataMenuItems>
        <Columns>
            <controls:GeneralGridColumn HeaderText="<%$CPResource:ID%>" Key="Id" DataFieldName="ID" Width="40" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:IsActive%>" Key="IsActive" DataFieldName="IsActive" SearchColumnType="TextDropDown" Width="100" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:Name%>" Key="Name" DataFieldName="Name" SearchColumnType="Text" Width="50%" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:DaylightName%>" Key="DaylightName" SearchColumnType="Text" DataFieldName="DaylightName" Width="50%" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:BiasHHMMSS%>" Key="Bias" DataFieldName="Bias" Width="100" />
            
            <controls:GeneralGridColumn HeaderText="<%$CPResource:DaylightBias%>" Key="DaylightBias" DataFieldName="DaylightBias" Width="100" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:DaylightSavingStartDate%>" Key="DaylightSavingStartDate" DataFieldName="DaylightSavingStartDate" Width="150" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:DaylightSavingEndDate%>" Key="DaylightSavingEndDate" DataFieldName="DaylightSavingEndDate" Width="150" />
            <controls:GeneralGridColumn Key="IsDaylightSavingTimeNow" DataFieldName="IsDaylightSavingTimeNow" Width="40" Hidden="True" />
        </Columns>
    </controls:Grid>
</asp:Content>