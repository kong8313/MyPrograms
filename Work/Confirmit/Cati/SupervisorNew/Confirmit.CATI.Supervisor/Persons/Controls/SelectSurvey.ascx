<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectSurvey.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Persons.Controls.SelectSurvey" %>
<controls:Grid runat="server" ID="gridSurveys" HasMultySelectionCheckBox="false"
    PrimaryKeyColumn="SID" HideSelectedColumn="true">
    <Columns>
        <controls:GeneralGridColumn HeaderText="ID" Key="SID" DataFieldName="SID"
            Width="60" Hidden="true" />
        <controls:GeneralGridColumn DataFieldName="Name" HeaderText="<%$CPResource:ProjectId%>"
            SearchColumnType="Text" Key="Name" Width="150px" />
        <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectName%>" Key="Description"
            SearchColumnType="Text" DataFieldName="Description" Width="100%" />
    </Columns>
</controls:Grid>
