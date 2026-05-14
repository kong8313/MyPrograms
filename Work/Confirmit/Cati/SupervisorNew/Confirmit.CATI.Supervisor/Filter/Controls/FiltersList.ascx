<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="FiltersList.ascx.cs"
            Inherits="Confirmit.CATI.Supervisor.Filter.Controls.FiltersList" %>

<controls:Grid ID="filtersGrid" runat="server" PrimaryKeyColumn="ID" EnablePaging="false"
    OnDblClickCommand="Properties">
    <Commands>
        <controls:OverlayCommand Key="Add" Caption="AddAdvancedFilter" URL="Filter/FilterAdd.aspx"
            Height="600" Width="800" Top="100" Image="plus" SelectMode="No"
            Title="AddAdvancedFilter" OnServerClick="OnFiltersListChanged" />
        <controls:OverlayCommand Key="Properties" Caption="Properties" IDName="fltID" Url="Filter/FilterAdd.aspx"
           Height="600" Width="800" Top="100" Image="view" SelectMode="SingleRow"
            Title="EditAdvancedFilter" OnServerClick="OnFiltersListChanged" />
        <controls:Command Key="Remove" Caption="Remove filter" Confirmation="DoYouWantToRemoveFilter"
            OnServerClick="DeleteFilter" Image="delete" SelectMode="MultiRow" /> 
        <controls:OverlayCommand Key="Copy" Caption="Copy/move filters from survey..."
            URL="Filter/CopySurveySpecificFilters.aspx" SelectMode="No" OnServerClick="OnFiltersListChanged"
            Height="600" Width="820" Top="100" Image="content_copy" Title="Copy/move filters from survey"  />
    </Commands>
    <ToolbarItems>
        <controls:ToolbarCommandButton Key="Add" />
        <controls:XpMenuItem runat="server" ButtonType="Separator" />
        <controls:ToolbarCommandButton Key="Properties" />
        <controls:XpMenuItem runat="server" ButtonType="Separator" />
        <controls:ToolbarCommandButton Key="Remove" />
        <controls:ToolbarCommandButton Key="Copy" />
        <controls:CheckBox ID="cbShowAllFilters" runat="server" Checked="true" ResName="ShowAllFilters"
            AutoPostBack="True" Width="120px" />
    </ToolbarItems>
    <DataMenuItems>
        <controls:DataMenuItem Key="Add" />
        <controls:DataMenuItem Key="Properties" />
        <controls:DataMenuItem Key="Remove" />
    </DataMenuItems>
    <Columns>
        <controls:GeneralGridColumn HeaderText="ID" Key="ID" SearchColumnType="Number" DataFieldName="SID"
            Width="100px" />
        <controls:GeneralGridColumn HeaderText="Name" Key="Name" SearchColumnType="Text"
            DataFieldName="Name" Width="20%" />
        <controls:GeneralGridColumn HeaderText="Description" Key="Description" SearchColumnType="Text"
            DataFieldName="Description" Width="100%" />
        <controls:UnboundGeneralGridColumn Header-Text="Type" Key="Type" SearchColumnType="DropDown"
            SearchColumnName="SurveySID" Width="100px" />
    </Columns>
</controls:Grid>
