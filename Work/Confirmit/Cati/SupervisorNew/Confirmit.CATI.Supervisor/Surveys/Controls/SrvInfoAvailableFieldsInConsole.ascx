<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SrvInfoAvailableFieldsInConsole.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Surveys.Controls.SrvInfoAvailableFieldsInConsole" %>
<controls:StateChecker runat="server" ID="stateChecker" AutomaticallySubscribeOnChangeEvents="True" ShowBeforeUnloadWarning="True" />
<div class="tab-content">
<controls:Grid ID="m_grid" NoDataMessage="NoCatiVariables"
    runat="server" PrimaryKeyColumn="FieldName" Height="100%" Width="100%" EnablePaging="false"
    EnableSorting="false">
    <Commands>
        <controls:Command Key="Save" Caption="Save" Image="save" OnServerClick="btnSave_Click" />
    </Commands>
    <ToolbarItems>
        <controls:ToolbarCommandButton Key="Save" ID="btnSave" runat="server"/>
    </ToolbarItems>
    <Columns>
        <controls:GeneralGridColumn Key="FieldName" DataFieldName="FieldName" Hidden="true" />
        <controls:GeneralGridColumn HeaderText="<%$CPResource:Name%>" Key="DisplayName" DataFieldName="DisplayName" Width="200px" />
        <controls:GeneralGridColumn HeaderText="<%$CPResource:SearchableFieldType %>" Key="FieldType" DataFieldName="FieldType" Width="100%"/>
    </Columns>
</controls:Grid>
</div>