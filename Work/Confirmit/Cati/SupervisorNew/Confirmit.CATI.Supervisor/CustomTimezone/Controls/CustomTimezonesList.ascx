<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="CustomTimezonesList.ascx.cs"
            Inherits="Confirmit.CATI.Supervisor.CustomTimezone.Controls.CustomTimezonesList" %>

<controls:Grid ID="customTimezonesGrid" runat="server" PrimaryKeyColumn="ID" EnablePaging="false"
    OnDblClickCommand="Edit" HideResetButton="True" HideSelectedColumn="True">
    <Commands>
        <controls:OverlayCommand Key="Add" Caption="AddCustomTimezone" URL="CustomTimezone/CustomTimezoneAdd.aspx"
            Height="150" Width="400" Image="plus" SelectMode="No" RefreshListFrame="True"
            Title="AddCustomTimezone" OnServerClick="OnCustomTimezonesListChanged" />
        <controls:OverlayCommand Key="Edit" Caption="EditCustomTimezone" IDName="tzID" Url="CustomTimezone/CustomTimezoneAdd.aspx"
                                 Height="150" Width="400" Image="edit" SelectMode="SingleRow"
                                 Title="EditCustomTimezone" OnServerClick="OnCustomTimezonesListChanged" />
        <controls:Command Key="Remove" Caption="RemoveCustomTimeZone" Confirmation="DoYouWantToRemoveCustomTimezone"
                          OnServerClick="DeleteCustomTimezone" Image="delete" SelectMode="MultiRow" />
    </Commands>
    <ToolbarItems>
        <controls:ToolbarCommandButton runat="server" Key="Add" />
        <controls:XpMenuItem runat="server" ButtonType="Separator" />
        <controls:ToolbarCommandButton runat="server" Key="Edit" />
        <controls:XpMenuItem runat="server" ButtonType="Separator" />
        <controls:ToolbarCommandButton runat="server" Key="Remove" />
    </ToolbarItems>
    <DataMenuItems>
        <controls:DataMenuItem Key="Add" />
        <controls:DataMenuItem Key="Edit" />
        <controls:DataMenuItem Key="Remove" />
    </DataMenuItems>
    <Columns>
        <controls:GeneralGridColumn HeaderText="ID" Key="ID" SearchColumnType="Number" DataFieldName="ID"
                                    Width="100px" />
        <controls:GeneralGridColumn HeaderText="Name" Key="Name" SearchColumnType="Text"
                                    DataFieldName="Name" Width="100%" />
    </Columns>
</controls:Grid>
