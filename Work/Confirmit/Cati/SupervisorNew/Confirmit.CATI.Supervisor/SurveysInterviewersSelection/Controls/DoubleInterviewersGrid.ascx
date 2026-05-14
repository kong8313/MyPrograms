<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DoubleInterviewersGrid.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Controls.DoubleInterviewersGrid" %>
<controls:DoubleGrid runat="server">
    <AddButton OnClick="Add"/>
    <RemoveButton OnClick = "Remove"/>
    <RemoveAllButton OnClick = "RemoveAll"/>
    <LeftGridContent>
        <controls:Grid ID="allGrid" runat="server" PrimaryKeyColumn="Id" GridName="Available Interviewers/Groups" EnablePaging="False"
            GridNameWidth="100%" PageSize="50" OnDblClickCommand="Add" SortedColumnName="Name">
            <Commands>
                <controls:Command Key="Add" SelectMode="SingleRow" OnServerClick="Add"
                    IDColumnName="Id" />
            </Commands>
            <Columns>                
                <controls:GeneralGridColumn HeaderText="<%$CPResource:ID%>" Key="Id" SearchColumnType="Number"
                    DataFieldName="Id" Width="50px" Hidden="True" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:Name%>" Key="PersonName" SearchColumnType="Text"
                    DataFieldName="Name" Width="100%" SortIndicator="Ascending" />   
                <controls:GeneralGridColumn HeaderText="<%$CPResource:ObjType%>" Key="IsGroup" SearchColumnType="DropDown"
                    DataFieldName="IsGroup" Width="75px" EnableSorting="False" />  
                <controls:UnboundGeneralGridColumn Key="Id_IsGroup" Hidden="True" />           
            </Columns>
        </controls:Grid>
    </LeftGridContent>
    <RightGridContent>
        <controls:Grid ID="selectedGrid" runat="server" GridName="Selected Interviewers" EnablePaging="False"
            PrimaryKeyColumn="Id" OnDblClickCommand="Remove" PageSize="50" SortedColumnName="Name">
            <Commands>
                <controls:Command Key="Remove" SelectMode="SingleRow" OnServerClick="Remove"
                    IDColumnName="Id" />
            </Commands>
            <Columns>                
                <controls:GeneralGridColumn HeaderText="<%$CPResource:ID%>" Key="Id" SearchColumnType="Number"
                    DataFieldName="Id" Width="50px" Hidden="True" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:Name%>" Key="PersonName" SearchColumnType="Text"
                    DataFieldName="Name" Width="100%" SortIndicator="Ascending" />                
            </Columns>
        </controls:Grid>
    </RightGridContent>
</controls:DoubleGrid>
