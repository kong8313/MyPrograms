<%@ Page Language="C#" MasterPageFile="~/MasterPages/RightFrameWithInfo.master" AutoEventWireup="true"
    CodeBehind="DialersList.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.DialersList" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        tbody tr td.ConnectedAndActivated {
            color: black;
        }

        tbody tr td.ConnectedAndDeactivated {
            color: red;
        }

        tbody tr td.DisconnectedAndDeactivated {
            color: red;
        }
        
        tbody tr td.DisconnectedTryingToConnect {
            color: red;
        }
        
        tbody tr td.DisconnectedTryingToConnectAndActivate {
            color: red;
        }
    </style>

    <script type="text/javascript">
        function openDialerSettingsFrame() {
            Y.on("domready", function () {
                openAndSetInfoFrame('<%=BaseRelativePath("Resources/DialerSettings.aspx")%>');
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="RightFrameContent" runat="Server">
    <controls:Grid ID="grid" runat="server" GridName="<%$CPResource:Dialers%>" HideSelectedColumn="true"
        PrimaryKeyColumn="Id" EnablePaging="true" PageSize="50" OnDblClickCommand="EditDialer">
        <commands>
            <Controls:OverlayCommand
                Key="ConnectOrActivateDialer"
                Title="<%$CPResource:ConnectOrActivateDialer%>"      
                Caption="<%$CPResource:ConnectOrActivateDialer%>"
                SelectMode="SingleRow"
                Image="sync"
                Url="Resources/ConnectOrActivateDialer.aspx"
                OnServerClick="DoUpdate"
                IDColumnName="Id"
                IDName="Id"
                DialogMode="ViewEdit"
                Height="280"
                Width="500"
                Top="50"/>
            <Controls:OverlayCommand
                Key="DisconnectOrDeactivateDialer"
                Title="<%$CPResource:DisconnectOrDeactivateDialer%>"
                Caption="<%$CPResource:DisconnectOrDeactivateDialer%>"
                SelectMode="SingleRow"
                Image="sync_disabled"
                Url="Resources/DisconnectOrDeactivateDialer.aspx"
                OnServerClick="DoUpdate"
                IDColumnName="Id"
                IDName="Id"
                DialogMode="ViewEdit"
                Height="280"
                Width="500"
                Top="50"/>
            <Controls:OverlayCommand
                Key="DialerFeaturesView"
                Title="<%$CPResource:DialerFeaturesView%>"
                Caption="<%$CPResource:DialerFeaturesView%>"
                SelectMode="SingleRow"
                Image="ballot"
                Url="Resources/DialerFeaturesView.aspx"
                IDColumnName="Id"
                IDName="Id"
                DialogMode="ViewEdit"
                Height="520" 
                Width="560" 
                Top="50"/>
            <Controls:OverlayCommand
                Key="ViewDialerLogs"
                Title="<%$CPResource:ViewDialerLogs%>"
                Caption="<%$CPResource:ViewDialerLogs%>"
                SelectMode="SingleRow"
                Image="event_note"
                Url="Resources/ViewDialerLogs.aspx"
                IDColumnName="Id"
                IDName="Id"
                DialogMode="ViewEdit"
                Height="640" 
                Width="760" 
                Top="50"/>
            <Controls:OverlayCommand
                Key="AddDialer"
                Title="<%$CPResource:AddDialer%>"
                Caption="<%$CPResource:AddDialer%>"
                SelectMode="No"
                Image="plus"
                RefreshOwner="True"
                Width="850"
                Height="700"
                DialogMode="Create"
                Url="Resources/DialerProperties.aspx" />
            <Controls:OverlayCommand
                Key="EditDialer"
                Title="<%$CPResource:EditDialer%>"
                Caption="<%$CPResource:EditDialer%>"
                SelectMode="SingleRow"
                Image="edit"
                RefreshOwner="True"
                IDName="Id"
                IDColumnName="Id"
                Width="850"
                Height="700"
                DialogMode="ViewEdit"
                Url="Resources/DialerProperties.aspx" />
            <Controls:Command Key="Delete" Caption="<%$CPResource:Delete%>" SelectMode="SingleRow" OnServerClick="DeleteDialer" Image="delete" Confirmation="cnfr_DeleteDialer"/>
        </commands>
        <toolbaritems>  
            <controls:ToolbarCommandButton Key="ConnectOrActivateDialer" />
            <controls:ToolbarCommandButton Key="DisconnectOrDeactivateDialer" />
            <controls:XpMenuItem runat="server" ButtonType="Separator" />
            <controls:ToolbarCommandButton Key="DialerFeaturesView" />
            <controls:ToolbarCommandButton Key="ViewDialerLogs" />
			<controls:XpMenuItem runat="server" ButtonType="Separator" />
            <controls:ToolbarCommandButton Key="AddDialer" />
            <controls:ToolbarCommandButton Key="EditDialer" />
            <controls:ToolbarCommandButton Key="Delete" />
        </toolbaritems>
        <datamenuitems>
            <controls:DataMenuItem Key="ConnectOrActivateDialer" />
            <controls:DataMenuItem Key="DisconnectOrDeactivateDialer" />
            <controls:DataMenuItem IsSeparator="true" />
            <controls:DataMenuItem Key="DialerFeaturesView" />
            <controls:DataMenuItem Key="ViewDialerLogs" />
			<controls:DataMenuItem IsSeparator="true" />
            <controls:DataMenuItem Key="AddDialer" />
            <controls:DataMenuItem Key="EditDialer" />
            <controls:DataMenuItem Key="Delete" />
        </datamenuitems>
        <columns>
            <controls:GeneralGridColumn DataFieldName="Id" HeaderText="<%$CPResource:ID%>" SearchColumnType="Number" Key="Id" Width="80" />
            <controls:GeneralGridColumn DataFieldName="Name" HeaderText="<%$CPResource:Name%>" SearchColumnType="Text" Key="Name" Width="300" />
            <controls:GeneralGridColumn DataFieldName="DialerActualState" HeaderText="<%$CPResource:DialerActualState%>"  SearchColumnType="TextDropDown" SearchColumnName="DialerActualState" Key="DialerActualStateText" Width="100%" />
            <controls:GeneralGridColumn DataFieldName="DialerActualState" Key="DialerActualState" Width="100" Hidden="true" />
            <controls:GeneralGridColumn DataFieldName="DialType" HeaderText="<%$CPResource:DialTypeName%>" SearchColumnType="Text" Key="DialType" Width="100" />
            <controls:GeneralGridColumn DataFieldName="DialerConfigurationType" HeaderText="<%$CPResource:DialerConfigurationTypeName%>" SearchColumnType="None" Key="DialerConfigurationType" Width="100" />
            <controls:GeneralGridColumn DataFieldName="DialerVersion" HeaderText="<%$CPResource:DialerVersion%>" SearchColumnType="None" Key="DialerVersion" Width="100" />
        </columns>
    </controls:Grid>
</asp:Content>
