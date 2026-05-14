<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DoubleGroupsInterviewersGrid.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Controls.DoubleGroupsInterviewersGrid" %>
<controls:DoubleGrid runat="server">
    <AddButton OnClick="Add"/>
    <RemoveButton OnClick = "Remove"/>
    <RemoveAllButton OnClick = "RemoveAll"/>
    <LeftGridContent>
        <controls:Grid ID="allGrid" runat="server" PrimaryKeyColumn="Id_IsGroup" GridName="Available Interviewers"
            GridNameWidth="100%" PageSize="50" OnDblClickCommand="Add">
            <Commands>
                <controls:Command Key="Add" SelectMode="SingleRow" OnServerClick="Add"
                    IDColumnName="Id" />
            </Commands>
            <Columns>
                <controls:UnboundGeneralGridColumn Key="Id_IsGroup" Hidden="True" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:ID%>" Key="Id" SearchColumnType="Number"
                    DataFieldName="Id" Width="50px" Hidden="True" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:Name%>" Key="PersonName" SearchColumnType="Text"
                    DataFieldName="Name" Width="100%" SortIndicator="Ascending" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:ObjType%>" Key="IsGroup" EnableSorting="False" SearchColumnType="DropDown"
                    DataFieldName="IsGroup" Width="120px" />                
            </Columns>
        </controls:Grid>
    </LeftGridContent>
    <RightGridContent>
        <controls:Grid ID="selectedGrid" runat="server" GridName="Selected Interviewers"
            PrimaryKeyColumn="Id_IsGroup" OnDblClickCommand="Remove" PageSize="50">
            <Commands>
                <controls:Command Key="Remove" SelectMode="SingleRow" OnServerClick="Remove"
                    IDColumnName="Id" />
            </Commands>
            <Columns>
                <controls:UnboundGeneralGridColumn Key="Id_IsGroup" Hidden="True" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:ID%>" Key="Id" SearchColumnType="Number"
                    DataFieldName="Id" Width="50px" Hidden="True" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:Name%>" Key="PersonName" SearchColumnType="Text"
                    DataFieldName="Name" Width="100%" SortIndicator="Ascending" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:ObjType%>" Key="IsGroup" SearchColumnType="DropDown"
                    DataFieldName="IsGroup" Width="120px" EnableSorting="False" />                
            </Columns>
        </controls:Grid>
    </RightGridContent>
</controls:DoubleGrid>
