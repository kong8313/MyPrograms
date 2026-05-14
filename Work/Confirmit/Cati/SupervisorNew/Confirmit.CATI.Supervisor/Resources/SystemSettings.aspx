<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.master" AutoEventWireup="true"
    CodeBehind="SystemSettings.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.SystemSettings" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Grid ID="settingsGrid" runat="server" HideSelectedColumn="True" OnDblClickCommand="OverrideDefaultValue" GridName="<%$CPResource:SystemSettings%>"
        PrimaryKeyColumn="SystemName" ShowFullToolbarBorders="False" SortedColumnName="Group" SortIndicator="Ascending">

        <Columns>
            <controls:GeneralGridColumn HeaderText="<%$CPResource:SystemName%>" Key="SystemName" SearchColumnType="Text"
                DataFieldName="SystemName" Width="300" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:Group%>" Key="Group" SearchColumnType="TextDropDown"
                DataFieldName="Group" Width="120" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:DisplayName%>" Key="DisplayName" SearchColumnType="Text"
                DataFieldName="DisplayName" Width="300" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:Description%>" Key="Description" SearchColumnType="Text"
                DataFieldName="Description" Width="100%" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:OverriddenValue%>" Key="OverriddenValue" SearchColumnType="Text"
                DataFieldName="OverriddenValue" Width="120" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:DefaultValue%>" Key="DefaultValue" SearchColumnType="Text"
                DataFieldName="DefaultValue" Width="120" />
        </Columns>
        <Commands>
            <controls:OverlayCommand Key="ChangeDefaultValue" RefreshListFrame="true"
                Title="<%$CPResource:ChangeDefaultValue%>" Caption="<%$CPResource:ChangeDefaultValue%>"
                Url="Resources/SystemSettingsEdit.aspx" IDColumnName="SystemName" IDName="SystemName"
                Image="edit" Width="550" Top="200" Height="340" SelectMode="SingleRow" RefreshOwner="True"
                InlineParams="IsDefaultSetting=true"
                Confirmation="cnfr_ChangeDefaultValue" />
            <controls:OverlayCommand Key="OverrideDefaultValue" RefreshListFrame="true"
                Title="<%$CPResource:OverrideDefaultValue%>" Caption="<%$CPResource:OverrideDefaultValue%>"
                Url="Resources/SystemSettingsEdit.aspx" IDColumnName="SystemName" IDName="SystemName"
                Image="edit_outline" Width="550" Top="200" Height="340" SelectMode="SingleRow" RefreshOwner="True"
                InlineParams="IsDefaultSetting=false" />
            <controls:Command Key="DeleteOverriddenValue" Caption="<%$CPResource:DeleteOverriddenValue%>"
                SelectMode="SingleRow" OnServerClick="DeleteOverriddenValue"
                Confirmation="cnfr_DeleteOverriddenValue" Image="delete" />
        </Commands>
        <ToolbarItems>
            <controls:ToolbarCommandButton Key="ChangeDefaultValue" />
            <controls:ToolbarCommandButton Key="OverrideDefaultValue" />
            <controls:ToolbarCommandButton Key="DeleteOverriddenValue" />
        </ToolbarItems>
        <DataMenuItems>
            <controls:DataMenuItem Key="ChangeDefaultValue" />
            <controls:DataMenuItem Key="OverrideDefaultValue" />
            <controls:DataMenuItem Key="DeleteOverriddenValue" />
        </DataMenuItems>
    </controls:Grid>
</asp:Content>
