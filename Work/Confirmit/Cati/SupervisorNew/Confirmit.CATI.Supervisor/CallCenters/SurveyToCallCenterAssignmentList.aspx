<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.master"
    CodeBehind="SurveyToCallCenterAssignmentList.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallCenters.SurveyToCallCenterAssignmentList" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="Content">
    <controls:Grid ID="_assignmentsGrid" runat="server" EnablePaging="True" SortedColumnName="SupervisorName"
        PrimaryKeyColumn="SurveyId" OnDblClickCommand="Activate" GridName="Call Centers - Surveys"
        HideSelectedColumn="True">
        <Commands>
            <controls:OverlayCommand Key="Activate" Caption="ActivateSurveyInCallCenters" IDName="ID"
                IDColumnName="SurveyId" Image="activate" Url="CallCenters/ActivateSurveyInCallCenters.aspx"
                DialogMode="ViewEdit" SelectMode="SingleRow" Title="ActivateSurveyInCallCenters"
                Height="550" Width="900" Top="250" RefreshOwner="True" />
        </Commands>
        <ToolbarItems>
            <controls:ToolbarCommandButton Key="Activate" />
        </ToolbarItems>
        <DataMenuItems>
            <controls:DataMenuItem Key="Activate" />
        </DataMenuItems>
        <Columns>
            <controls:GeneralGridColumn Key="SurveyId" Hidden="True" DataFieldName="SurveyId" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectId%>" Key="ProjectId"
                Width="150px" DataFieldName="ProjectId" SearchColumnType="Text" SortIndicator="Ascending" />
            <controls:GeneralGridColumn Key="SurveyName" HeaderText="<%$CPResource:ProjectName%>"
                DataFieldName="SurveyName" SearchColumnType="Text" Width="350px" />
            <controls:GeneralGridColumn Key="CallCenters" HeaderText="<%$CPResource:CallCenterNameColumn%>"
                DataFieldName="CallCenterNames" Width="100%" SearchColumnName="CallCenterId"
                SearchColumnType="DropDown" EnableSorting="False" />
        </Columns>
    </controls:Grid>
</asp:Content>
