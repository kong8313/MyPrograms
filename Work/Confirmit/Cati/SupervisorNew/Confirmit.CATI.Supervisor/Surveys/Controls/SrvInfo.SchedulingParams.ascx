<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SrvInfo.SchedulingParams.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Surveys.Controls.SrvInfoSchedulingParams" %>
<div class="tab-content">
<controls:Grid ID="m_grid" runat="server" HideSelectedColumn="true"
    HideResetButton="True" SortedColumnName="Id" PrimaryKeyColumn="Id" EnablePaging="False"
    ShowFullToolbarBorders="False" OnDblClickCommand="Edit" HasMultySelectionCheckBox="False"
    EnableSorting="False">
    <Commands>
        <controls:OverlayCommand Key="Edit" Caption="Edit" Title="Edit Parameter" SelectMode="SingleRow"
            Image="edit" RefreshOwner="True" Width="430" Height="215" DialogMode="ViewEdit"
            IDName="ParameterId" IDColumnName="Id" Url="Surveys/SurveyScriptParameterProperties.aspx" />
        <controls:Command Key="ResetToDefault" Caption="Reset to default values" Confirmation="<%$CPResource:SchedulingParamsResetConfirmation%>"
            SelectMode="No" Image="reset" OnServerClick="ResetParams" />
    </Commands>
    <ToolbarItems>
        <controls:ToolbarCommandButton Key="Edit" />
        <controls:ToolbarCommandButton Key="ResetToDefault" />
    </ToolbarItems>
    <DataMenuItems>
        <controls:DataMenuItem Key="Edit" />
    </DataMenuItems>
    <Columns>
        <controls:GeneralGridColumn DataFieldName="ParamID" HeaderTextId="ID" Key="Id" Width="35px" />
        <controls:GeneralGridColumn DataFieldName="Name" HeaderTextId="Name" Key="Name" Width="160px" />
        <controls:UnboundGeneralGridColumn Header-Text="<%$CPResource:ParamType%>" Key="TypeName"
            Width="160px" />
        <controls:GeneralGridColumn DataFieldName="Type" Key="Type" Hidden="True" />
        <controls:GeneralGridColumn DataFieldName="Value" HeaderTextId="Value"
            Key="Value" Width="160px" />
        <controls:GeneralGridColumn DataFieldName="Description" HeaderTextId="Description"
            Key="Description" Width="100%" />
    </Columns>
</controls:Grid>
<script type="text/javascript">
    Common.onGlobalEvent("SurveyViewAssignedScriptChanged", function () {
        document.location.href += "";
    });
</script>
    </div>