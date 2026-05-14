<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonSurveyAssignmentList.ascx.cs"
            Inherits="Confirmit.CATI.Supervisor.Persons.Controls.PersonSurveyAssignmentList" %>
    <style type="text/css">
        tr.AutoSurveyRow td {
            background-color: rgb(211, 249, 188) !important;
        }
    </style>
<controls:Grid ID="m_grid" runat="server" PrimaryKeyColumn="SID_Calls">
    <commands>
        <Controls:OverlayCommand Key="New" Caption="New" DialogMode="ViewEdit" SelectMode="No"
            Width="750" Height="700" Top="100" Title="SelectSurveysToAssign" OnServerClick="RefreshHandler"
            URL="Persons/AddOrReplacePersonSurveyAssignment.aspx" Image="plus" IDColumnName="SID_Calls"/>
       <Controls:OverlayCommand Key="Replace" Caption="Replace" OnServerClick="RefreshHandler" IDColumnName="SID_Calls"
            DialogMode="ViewEdit" Width="750" Height="700" Top="100" Title="SelectSurveysToAssign"
            URL="Persons/AddOrReplacePersonSurveyAssignment.aspx" Image="persons" SelectMode="No"/>
        <Controls:Command Key="Deassign" Caption="Deassign survey" SelectMode="MultiRow" OnServerClick="DeassignSurveys" 
            Image="block" Confirmation="cnfr_deassign"/>
        <Controls:Command Key="SetAutomaticSurvey" Caption="Set automatic survey" Image="set_default" SelectMode = "SingleRow" OnServerClick="SetAutomaticSurvey" />
	</commands>
    <toolbaritems>
		<Controls:ToolbarCommandButton Key="New"/>
		<Controls:ToolbarCommandButton Key="Replace"/>
		<Controls:ToolbarCommandButton Key="Deassign"/>
		<Controls:ToolbarCommandButton Key="SetAutomaticSurvey" />
	</toolbaritems>
    <DataMenuItems> 
		<Controls:DataMenuItem Key="New"/>
		<Controls:DataMenuItem Key="Deassign"/>
		<Controls:DataMenuItem Key="SetAutomaticSurvey" />
	</DataMenuItems>
    <Columns>
        <controls:GeneralGridColumn Key="SurveySID" DataFieldName="SurveySID" SearchColumnType="Number"
            HeaderText="ID" Width="50px" Hidden="true" />
        <controls:GeneralGridColumn Key="ProjectId" DataFieldName="ProjectID" SearchColumnType="Text"
            HeaderText="<%$CPResource:ProjectId%>" Width="100px" />
        <controls:GeneralGridColumn Key="ProjectName" DataFieldName="ProjectName" SearchColumnType="Text"
            HeaderText="<%$CPResource:ProjectName%>" Width="100%" />
        <controls:GeneralGridColumn Key="AssignmentGroup" DataFieldName="ParentGroupName" SearchColumnType="Text"
            HeaderText="<%$CPResource:AssignmentGroup%>" Width="150px" />
        <controls:GeneralGridColumn Key="AssignedCallsCount" DataFieldName="AssignedCallsCount"
            SearchColumnType="Number" HeaderText="<%$CPResource:Count%>" Width="100px" />
        <controls:UnboundGeneralGridColumn Key="SID_Calls" Hidden="true" />
    </Columns>
</controls:Grid>
