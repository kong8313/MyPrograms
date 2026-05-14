<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DoubleSurveysGrid.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Controls.DoubleSurveysGrid" %>
<controls:DoubleGrid runat="server">
    <AddButton OnClick="AddSurveys" />
    <RemoveButton OnClick="RemoveSurveys" />
    <RemoveAllButton OnClick = "RemoveAll"/>
    <LeftGridContent>
        <controls:Grid ID="allSurveysGrid" runat="server" GridName="Available Surveys" PrimaryKeyColumn="Id" EnablePaging="False" 
            OnDblClickCommand="AddSurveys">
            <Commands>
                <controls:Command Key="AddSurveys" SelectMode="SingleRow" OnServerClick="AddSurveys" IDColumnName="Id" />
            </Commands>
            <ToolbarItems>
                <controls:XpMenuItem ID="miAddOpenSurveys" runat="server" Text="<%$CPResource:AddOpenSurveys%>" OnClick="AddAllOpenSurveys" ImageName="assignment_turned_in" />
            </ToolbarItems>
            <Columns>
                <controls:GeneralGridColumn HeaderText="ID" Key="Id" SearchColumnType="Number" DataFieldName="Id"
                    Width="50" Hidden="True" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectId%>" Key="ConfirmitID"
                    SearchColumnType="Text" DataFieldName="ConfirmitID" Width="100px" SortIndicator="Ascending" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectName%>" Key="Name" SearchColumnType="Text"
                    DataFieldName="Name" Width="100%" />
            </Columns>
        </controls:Grid>
    </LeftGridContent>
    <RightGridContent>
        <controls:Grid ID="selectedSurveysGrid" runat="server" GridName="Selected Surveys" EnablePaging="False"
            PrimaryKeyColumn="Id" OnDblClickCommand="RemoveSurveys">
            <Commands>
                <controls:Command Key="RemoveSurveys" SelectMode="SingleRow" OnServerClick="RemoveSurveys"
                    IDColumnName="Id" />
            </Commands>
            <Columns>
                <controls:GeneralGridColumn HeaderText="ID" Key="Id" SearchColumnType="Number" DataFieldName="Id"
                    Width="50" Hidden="True" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectId%>" Key="ConfirmitID"
                    SearchColumnType="Text" DataFieldName="ConfirmitID" Width="100px" SortIndicator="Ascending" />
                <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectName%>" Key="Name" SearchColumnType="Text"
                    DataFieldName="Name" Width="100%" />
            </Columns>
        </controls:Grid>
    </RightGridContent>
</controls:DoubleGrid>
