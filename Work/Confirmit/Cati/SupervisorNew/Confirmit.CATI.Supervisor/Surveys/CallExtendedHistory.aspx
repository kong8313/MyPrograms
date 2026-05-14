<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" 
    CodeBehind="CallExtendedHistory.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Surveys.CallExtendedHistory" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" ID="dialog" EnableViewState="true" HideHeader="True" HideButtons="True" Mode="Frame" PutActionButtonsInsideGridIfPossible="False">
        <okbutton visible="false" />
        <content>
    		<Controls:Grid id="gridCallHistory" HintText ="<%$CPResource:CallHistoryHint %>" runat="server" PrimaryKeyColumn="Id" HideSelectedColumn="true" EnablePaging="false" EnableSorting="False">
                <Commands>
                    <controls:Command Key="OperationDetails" Caption="OperationDetails" SelectMode="SingleRow" Image="view" OnClientClick="ShowOperationDetails()" />
                </Commands>
                 <ToolbarItems>
                    <controls:ToolbarCommandButton Key="OperationDetails" ID="ToolbarCommandButton2" runat="server" />
                </ToolbarItems>
				<Columns>
					<controls:GeneralGridColumn 
						HeaderText="Id" 
						Key="Id" 
						DataFieldName="Id"
						Width="100"
						Hidden="true"/>
                   <controls:GeneralGridColumn
						HeaderText="<%$CPResource:EventTime%>" 
						Key="FiredTime" 
						DataFieldName="FiredTime" 
						Width="120"/>
       				<controls:GeneralGridColumn
						HeaderText="ApptId" 
						Key="ApptId" 
						DataFieldName="ApptId" 
						Width="135"
						Hidden="true"/>
       				<controls:GeneralGridColumn
						HeaderText="<%$CPResource:ExtendedStatusCode%>" 
						Key="ITS" 
						DataFieldName="ITS" 
						Width="135"
						Hidden="true"/>
					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:ExtendedStatus%>" 
						Key="TransientState" 
						DataFieldName="TransientState" 
						Width="100"/>
                    <controls:GeneralGridColumn
						HeaderText="<%$CPResource:DialTypeName%>" 
						Key="DialType" 
						DataFieldName="DialType" 
						Width="70"
                        Hidden="true"/>
       				<controls:GeneralGridColumn
						HeaderText="ShiftTypeId" 
						Key="ShiftTypeId" 
						DataFieldName="ShiftTypeId" 
						Width="50"
						Hidden="true"/>
					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:ShiftType%>" 
						Key="ShiftType" 
						DataFieldName="ShiftType" 
						Width="65"/>
					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:CallState%>" 
						Key="CallStateInfo" 
						DataFieldName="CallStateInfo" 
						Width="65"/>
  					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:Priority%>" 
						Key="Priority" 
						DataFieldName="Priority" 
						Width="44"/>
 					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:SearchableField_TimeToCall%>" 
						Key="TimeInShift" 
						DataFieldName="TimeInShift" 
						Width="120"/>
 					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:DialingMode%>" 
						Key="DialingMode" 
						DataFieldName="DialingMode" 
						Width="80"/>
      				<controls:GeneralGridColumn
						HeaderText="ExplicitSid" 
						Key="ExplicitSid" 
						DataFieldName="ExplicitSid" 
						Width="50"
						Hidden="true"/>
      				<controls:GeneralGridColumn
						HeaderText="ExplicitType" 
						Key="ExplicitType" 
						DataFieldName="ExplicitType" 
						Width="50"
						Hidden="true"/>
 					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:AssignedTo%>" 
						Key="Resource" 
						DataFieldName="Resource" 
						Width="100"/>                                      
      				<controls:GeneralGridColumn
						HeaderText="CellId" 
						Key="CellId" 
						DataFieldName="CellId" 
						Width="50"
						Hidden="true"/>
 					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:AsyncOperationProgress_OperationId%>" 
						Key="OperationId" 
						DataFieldName="OperationId" 
						Width="70"/>     
 					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:Operation%>" 
						Key="OperationType" 
						DataFieldName="OperationType" 
						Width="120"/>     
                    <controls:GeneralGridColumn
						HeaderText="<%$CPResource:ExpireTime%>" 
						Key="ExpireTime" 
						DataFieldName="ExpireTime" 
						Width="120"/>
 					<controls:GeneralGridColumn
						HeaderText="<%$CPResource:CallCenter%>" 
						Key="CallCenterName" 
						DataFieldName="CallCenterName" 
						Width="90"/>   
				</Columns>
			</Controls:Grid>
		</content>
    </controls:Dialog>
     <script type="text/javascript">
         function ShowOperationDetails(gridController) {

             var operationId = gridController.GetSelectedRow().get_cellByColumnKey('OperationId').get_value();
             var role = gridController.GetSelectedRow().get_cellByColumnKey('OperationType').get_value();

             if (operationId == 0) {
                 alert("No details for this operation.");
                 return;
             }

             var settings = { height: 370 + "px", width: 680 + "px" };
             var params = {
                 OperationId: operationId,
                 OperationTitle: role
             };

             PageMethods.CheckOperation(operationId,
                 function(result) {
                     if (result) {
                         overlay.show(role, "AsyncOperations/AsyncOperationView.aspx", params, settings, null);
                     } else {
                         alert("The operational data required for this request has expired.");
                     }
                 });
         }
     </script>
</asp:Content>
