<%@ Page Language="c#" MasterPageFile="~/MasterPages/RightFrameWithInfo.master"
	Codebehind="GroupsList.aspx.cs" AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.Persons.GroupsList" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="Server">
    <script type="text/javascript">
        function openGroupInfoFrame(id) {
            Y.on("domready", function () {
                openAndSetInfoFrame('<%=BaseRelativePath("Persons//GroupProperties.aspx")%>?ID=' + id);
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="RightFrameContent" runat="Server">
	<controls:grid id="groupsListGrid" runat="server" primarykeycolumn="ID" ondblclickcommand="Properties" GridName="Groups"
		RightToolbarButtons="Logoff" ShowFullToolbarBorders="False">
			<Commands>
				<Controls:OverlayCommand Key="Add" Caption="Add group" RefreshListFrame="true" Title="NewGroup" URL="Persons/GroupProperties.aspx" Image="plus" Top="100" Height="650" Width="650" DialogMode="Create" SelectMode="No"/>
				<Controls:ViewCommand Key="Properties" Caption="View" URL="Persons/GroupProperties.aspx" Image="view"/>
				<Controls:Command Key="Delete" Caption="Delete group" SelectMode="MultiRow" OnServerClick="DeleteGroup" Image="delete" Confirmation="DoYouWantToDeleteSelectedGroups"/>
				<Controls:OverlayCommand Key="AddAssignment" DialogMode="ViewEdit" Caption="AddAssignment" IDColumnName="ID" IDName="IDS" Width="750" Height="700" Top="100" URL="Persons/AddOrReplacePersonSurveyAssignment.aspx?IsGroup=true" Title="AddAssignment" SelectMode="MultiRow" Image="assignment_add" RefreshInfoFrame="True"/>
				<Controls:OverlayCommand Key="ReplaceAssignment" DialogMode="ViewEdit" Caption="ReplaceAssignment" IDColumnName="ID" IDName="IDS" Width="750" Height="700" Top="100" URL="Persons/AddOrReplacePersonSurveyAssignment.aspx?IsGroup=true&ReplaceAssignment=true" Title="ReplaceAssignment" SelectMode="MultiRow" RefreshInfoFrame="True" Image="assignment_replace"/>
				<controls:OverlayCommand Key="ChangeAutomaticSurvey" Caption="ChangeAutomaticSurvey" IDColumnName="ID" IDName="ObjectSid" Width="650" Height="700" Top="100" URL="Persons/ChangeAutomaticSurvey.aspx?IsGroup=true" Title="ChangeAutomaticSurvey" SelectMode="MultiRow" RefreshInfoFrame="True" Image="change_automatic_survey"/>
                <Controls:OverlayCommand Key="ChangeTaskChoice" DialogMode="ViewEdit" SelectMode="MultiRow" InlineParams="IsGroup=true" Title="ChangeTaskChoice" Caption="ChangeTaskChoice" IDName="IDS" IDColumnName="ID" Width="345" Height = "140" Top="100" URL="Persons/ChangeTaskChoice.aspx" Image="call_split"/>
				<Controls:OverlayCommand Key="SendMessage" DialogMode="ViewEdit" SelectMode="MultiRow" Title="SendMessage" Caption="SendMessage" IDName="IDS" IDColumnName="ID" InlineParams="MessageRecipientType=InterviewerGroup" Width="560" Height = "390" Top="100" URL="Messaging/SendMessageView.aspx" Image="send"/>
			</Commands>
			
			<ToolbarItems>
				<Controls:ToolbarCommandButton Key="Add"/>
				<Controls:ToolbarCommandButton Key="Properties"/>
				<Controls:ToolbarCommandButton Key="Delete"/>
			</ToolbarItems>
			
			<DataMenuItems>
				<Controls:DataMenuItem Key="Add"/>
				<Controls:DataMenuItem Key="Properties"/>
				<Controls:DataMenuItem Key="Delete"/>
				<controls:DataMenuItem Key="AddAssignment"/>
				<controls:DataMenuItem Key="ReplaceAssignment"/>
                <controls:DataMenuItem Key="ChangeAutomaticSurvey" />
				<Controls:DataMenuItem Key="ChangeTaskChoice"/>
				<Controls:DataMenuItem IsSeparator="true"/>
				<Controls:DataMenuItem Key="SendMessage"/>
			</DataMenuItems>
			
			<Columns>
			    <controls:GeneralGridColumn HeaderText="ID" Key="ID" SearchColumnType="Number" DataFieldName="Id" Width="100"/>
				<controls:GeneralGridColumn HeaderText="Name" Key="Name" SearchColumnType="Text" DataFieldName="Name" Width="100%" SortIndicator="Ascending"/>
			    <controls:GeneralGridColumn HeaderText="Description" Key="Description" SearchColumnType="Text" DataFieldName="Description" Width="100%" SortIndicator="Ascending"/>
				<controls:GeneralGridColumn HeaderText="Administrative" Key="IsAdministrative" SearchColumnType="DropDown" DataFieldName="IsAdministrative" Width="100" SortIndicator="Ascending"/>
			    <controls:GeneralGridColumn HeaderText="<%$CPResource:InboundSetting%>" Key="Inbound" SearchColumnType="DropDown" DataFieldName="InboundCallBehavior" Width="210" SortIndicator="Ascending"/>
			    <controls:GeneralGridColumn HeaderText="<%$CPResource:TransferSetting%>" Key="Transfer" SearchColumnType="DropDown" DataFieldName="CallTransferBehavior" Width="230" SortIndicator="Ascending"/>
			    <controls:GeneralGridColumn HeaderText="<%$CPResource:MemberCount%>" Key="Count" SearchColumnType="Number" DataFieldName="Count" Width="100" SortIndicator="Ascending"/>
			</Columns>
		</controls:grid>
</asp:Content>
