<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CallGroupsList.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.Controls.CallGroupsList" %>
<%@ Register TagPrefix="controls" TagName="Grid" Src="~/Controls/GeneralGrid.ascx" %>
<controls:Grid ID="grid" runat="server" HideSelectedColumn="true" GridName="<%$CPResource:CallGroups%>"
    OnDblClickCommand="View" PrimaryKeyColumn="Id" AutoGenerateColumns="false" ShowFullToolbarBorders="False">
	<commands>
		<Controls:ViewCommand Key="View" Caption="View" URL="Resources/PriorityGroupView.aspx" Image="view" IDColumnName="Id" IDName="CallGroupId"/>
		<Controls:OverlayCommand Key="New" DialogMode="Create" RefreshListFrame="true" SelectMode="No" Caption="New" Title="NewCallGroup" URL="Resources/CallGroupProperties.aspx" Height="250"  Width="400" Image="plus"/>		
        <Controls:OverlayCommand Key="Properties" DialogMode="ViewEdit" RefreshListFrame="true" SelectMode="SingleRow" IDName="Id" IDColumnName="Id"  Caption="Properties" Title="EditCallGroup" URL="Resources/CallGroupProperties.aspx" Height="250"  Width="400" Image="edit"/>		
		<controls:Command Key="Delete" SelectMode="SingleRow" Caption="Delete" OnServerClick="DeleteGroup" Confirmation="PriorityGroupList_ConfirmationDeleteGroup" Image="delete"/>
	</commands>
	<toolbaritems>
	    <controls:ToolbarCommandButton Key="View" />
	    <Controls:ToolbarCommandButton Key="New"/>	    
        <controls:ToolbarCommandButton Key="Properties" />
        <controls:ToolbarCommandButton Key="Delete" />
	</toolbaritems>
	<DataMenuItems>
	    <controls:DataMenuItem Key="View" />
		<controls:DataMenuItem Key="New" />		
		<controls:DataMenuItem Key="Delete" />
		<controls:DataMenuItem Key="Properties" />
	</DataMenuItems>
	<Columns>
		<controls:GeneralGridColumn HeaderText="Id" Key="Id" DataFieldName="Id" Width="75px" SearchColumnType="Number" />		
		<controls:GeneralGridColumn HeaderText="Name" Key="Name" DataFieldName="Name" SearchColumnType="Text"
			Width="200px" />
        <controls:GeneralGridColumn HeaderText="Description" Key="Description" DataFieldName="Description" SearchColumnType="Text"
			Width="100%" />
	</Columns>
</controls:Grid>
<script>
    function CallGroupController(settings) {

        this.DeleteSelectedCallGroup = function () {
            
            var groupId = Y.one("#" + settings.ClientGridId + "_hHighlighted").get("value");
            if (groupId) {
                PageMethods.HasGroupAssignments(groupId,
                    deleteGroup,
                    function () {
                        Y.log('An error ocurred while retrieving call group assignment information.');
                    });
            }
        };

        function deleteGroup(parameters) {

            if (parameters.HasAssignments && confirm(parameters.WarningText) == false) {
                return;
            }
            
            eval(settings.DeleteCallGroupPostBackReference);
        }
    }    
    
</script>