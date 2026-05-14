<%@ Page Language="C#" MasterPageFile="~/MasterPages/RightFrameWithInfo.master" AutoEventWireup="true" CodeBehind="AsyncOperations.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.AsyncOperations" %>

<asp:Content ID="Content" ContentPlaceHolderID="RightFrameContent" runat="server">
    <controls:Grid ID="grid" runat="server" HideSelectedColumn="true" GridName="<%$CPResource:AsyncOperations%>"
        PrimaryKeyColumn="Id" SortedColumnName="Id" SortIndicator="Descending" AutoGenerateColumns="false" ShowFullToolbarBorders="False" TopToolbarLayout="DoubleMenu"  OnDblClickCommand="Details">
        <Commands>                            
              <controls:OverlayCommand Key="Details" Title="Task View" Caption="Details" Url="AsyncOperations/AsyncOperationView.aspx"
                        DialogMode="ViewEdit" IDColumnName="Id" IDName="OperationId" Image="view" Height="425" Width="640" Top="180"  />              
              <Controls:Command Key="Abort" Caption="Task Abort" SelectMode="SingleRow" OnClientClick="canTaskBeAborted()" OnServerClick="AbortTask" Image="cancel"/>
          </Commands>
        
        <LeftToolbarItems>            
            <controls:CheckBox ID="cbShowAllCallCenters" runat="server" AutoPostBack="True" Text="<%$CPResource:ShowAllCallCenters%>">
            </controls:CheckBox>
        </LeftToolbarItems>

        <Columns>
            <controls:GeneralGridColumn Key="OperationTitle" DataFieldName="OperationTitle" Hidden="True" />				
            <controls:GeneralGridColumn Key="OperationState" DataFieldName="OperationState" Hidden="True" />				

            <controls:GeneralGridColumn HeaderText="Task ID" Key="Id" DataFieldName="Id" SearchColumnType="Number" 
                Width="80px" />
            <controls:GeneralGridColumn HeaderText="User ID" Key="InitiatorName" DataFieldName="InitiatorName" SearchColumnType="Text"
                Width="100px" />
            <controls:GeneralGridColumn HeaderText="Survey ID" Key="ProjectId" DataFieldName="ProjectId" SearchColumnType="Text"
                Width="100px" />
            <controls:GeneralGridColumn HeaderText="Call Center" Key="CallCenterName" DataFieldName="CallCenterName"  SearchColumnName="CallCenterId"  SearchColumnType="DropDown"
                Width="130px" />
            <controls:GeneralGridColumn HeaderText="Task Type" Key="OperationType" DataFieldName="OperationType" SearchColumnType="DropDown"
                Width="160px" />            
            <controls:GeneralGridColumn HeaderText="Status" Key="OperationStateName" DataFieldName="OperationState" SearchColumnType="DropDown"
                Width="130px" />
            <controls:GeneralGridColumn HeaderText="Start Time" Key="StartedTime" DataFieldName="StartedTime" SearchColumnType="DateTime"
                Width="150px" />
            <controls:UnboundGeneralGridColumn Header-Text="Duration" Key="Duration" SearchColumnType="None" Width="120px" />
            <controls:UnboundGeneralGridColumn Header-Text="Elapsed Time" Key="ElapsedTime" SearchColumnType="None" />
            <controls:GeneralGridColumn HeaderText="Call Center ID" Key="CallCenterId" DataFieldName="CallCenterId" Hidden ="true" />
            
        </Columns>
        <ToolbarItems>
            <controls:ToolbarCommandButton Key="Details" />            
            <controls:ToolbarCommandButton Key="Abort" />            
        </ToolbarItems>
        <DataMenuItems>
            <controls:DataMenuItem Key="Details" />
        </DataMenuItems>
    </controls:Grid>     
    
    <script type="text/javascript">
        
        function canTaskBeAborted(gridController, queuedState, executingState) {
            
            var currentState = gridController.GetSelectedRow().get_cellByColumnKey('OperationState').get_value();                        
            
            if (currentState == queuedState)
            {
                return true;
            }
            
            if(currentState == executingState)
            {
                return confirm("Are you sure you want to abort this task? If the task is currently executing it will be stopped but current progress will not be reversed.");
            }
            
            alert("Task can not be aborted");
            return false;
        }
        
    </script>
    

</asp:Content>
