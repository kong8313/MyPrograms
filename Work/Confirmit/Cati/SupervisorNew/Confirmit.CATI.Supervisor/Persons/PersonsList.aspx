<%@ Page Language="C#" MasterPageFile="~/MasterPages/RightFrameWithInfo.master" AutoEventWireup="true"
    CodeBehind="PersonsList.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Persons.PersonsList" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        tbody tr.LockedRow {
            color: gray;
        }
    </style>
    
    <script type="text/javascript">
        function openInterviewerInfoFrame(id) {
            Y.on("domready", function () {
                openAndSetInfoFrame('<%=BaseRelativePath("Persons/PersonProperties.aspx")%>?PersonSID=' + id);
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="RightFrameContent" runat="Server">
    <controls:Grid ID="m_grid" runat="server" OnDblClickCommand="Properties"
        PrimaryKeyColumn="PersonSID" ShowFullToolbarBorders="False" PreserveSelectedState="True" >
        <Columns>
            <controls:GeneralGridColumn HeaderText="ID" Key="PersonSID" SearchColumnType="Number"
                DataFieldName="PersonSID" Width="40" />
            <controls:GeneralGridColumn HeaderText="Login" Key="PersonName" DataFieldName="PersonName" SearchColumnType="Text"
                Width="15%" SortIndicator="Ascending" />
            <controls:UnboundGeneralGridColumn Header-Text="<%$CPResource:LoggedIn%>" Key="LoggedIn"
                SearchColumnType="DropDown" SearchColumnName="LoggedIn" Width="88" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectId%>" Key="SurveyID"
                DataFieldName="SurveyID" SearchColumnType="Text" Width="110" />
            <controls:UnboundGeneralGridColumn Header-Text="<%$CPResource:TaskChoice%>" Key="TaskChoice"
                SearchColumnType="DropDown" SearchColumnName="ManualSelection" Width="12%" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:CallGroup%>" Key="CallGroupName"
                DataFieldName="CallGroupName" SearchColumnType="Text" Width="13%" />
            <controls:UnboundGeneralGridColumn Header-Text="<%$CPResource:DialTypeName%>" Key="DialTypeId"
                SearchColumnType="DropDown" SearchColumnName="DialTypeId" Width="85" />
            <controls:UnboundGeneralGridColumn Header-Text="<%$CPResource:SSOName%>" Key="EnableSoftphoneIntegration"
                SearchColumnType="DropDown" SearchColumnName="EnableSoftphoneIntegration" Width="85" />
            <controls:GeneralGridColumn HeaderText="Description" Key="PersonDescription" SearchColumnType="Text"
                DataFieldName="PersonDescription" Width="15%" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:Groups%>" Key="GroupNamesJson" DataFieldName="GroupNamesJson"
                SearchColumnType="DropDown" SearchColumnName="GroupNamesJson" Width="15%" />
            <controls:GeneralGridColumn HeaderText="Location" Key="PersonLocation" DataFieldName="PersonLocation" SearchColumnType="Text"
                Width="12%" SortIndicator="Ascending" />
            <controls:UnboundGeneralGridColumn Header-Text="<%$CPResource:Locked%>" Key="IsLocked"
                SearchColumnType="DropDown" SearchColumnName="IsLocked" Width="55" />
            <controls:GeneralGridColumn HeaderText="<%$CPResource:LockedDate%>" Key="LockedDate"
                SearchColumnType="DateTime" Width="10%" DataFieldName="LockedDate" />
        </Columns>
        <Commands>
            <controls:OverlayCommand Key="NewInterviewer" Caption="<%$CPResource:NewInterviewerButtonTooltip%>" Title="<%$CPResource:NewInterviewerDialogTitle%>" Url="Persons/PersonProperties.aspx" Top="100" Height="520" Width="720" DialogMode="Create" SelectMode="No"
                Image="person_add" RefreshOwner="True" />
            <controls:OverlayCommand Key="NewIvrAgent" Caption="<%$CPResource:NewIvrAgentButtonTooltip%>" Title="<%$CPResource:NewIvrAgentDialogTitle%>" Url="Persons/IvrAgentProperties.aspx" Top="100" Height="500" Width="720" DialogMode="Create" SelectMode="No"
                Image="person_add" RefreshOwner="True" />
            <controls:ViewCommand Key="Properties" Caption="View" IDColumnName="PersonSID"
                IDName="PersonSID" URL="Persons/PersonProperties.aspx" Image="view" />
            <controls:Command Key="Delete" Caption="Delete" SelectMode="MultiRow" OnServerClick="DeletePerson"
                Confirmation="cnfr_DeleteLiveInterviewer" Image="delete" PromptAcceptCode="1234"/>
            <controls:Command Key="Lock" Caption="Lock" SelectMode="MultiRow" OnServerClick="LockPerson"
                Confirmation="cnfr_LockLiveInterviewer" Image="_lock" />
            <controls:Command Key="Unlock" Caption="Unlock" SelectMode="MultiRow" OnServerClick="UnlockPerson"
                Confirmation="cnfr_UnlockLiveInterviewer" Image="unlock" />
            <controls:OverlayCommand Key="Import" RefreshListFrame="true" Title="Import" Caption="Import" Url="Persons/Import.aspx" Image="publish" Width="650" Top="100"
                Height="600" SelectMode="No" />
            <controls:OverlayCommand Key="AddAssignment" DialogMode="ViewEdit" Caption="AddAssignment" IDColumnName="PersonSID" IDName="IDS" Width="750" Height="700" Top="100" Url="Persons/AddOrReplacePersonSurveyAssignment.aspx?IsGroup=false" Title="AddAssignment" SelectMode="MultiRow" Image="assignment_add" RefreshInfoFrame="True" />
            <controls:OverlayCommand Key="ReplaceAssignment" DialogMode="ViewEdit" Caption="ReplaceAssignment" IDColumnName="PersonSID" IDName="IDS" Width="750" Height="700" Top="100" Url="Persons/AddOrReplacePersonSurveyAssignment.aspx?IsGroup=false&ReplaceAssignment=true" Title="ReplaceAssignment" SelectMode="MultiRow" RefreshInfoFrame="True" Image="assignment_replace" />
            <controls:OverlayCommand Key="ChangeAutomaticSurvey" Caption="ChangeAutomaticSurvey" IDColumnName="PersonSID"
                IDName="ObjectSid" Width="650" Height="700" Top="100" Url="Persons/ChangeAutomaticSurvey.aspx?IsGroup=false"
                Title="ChangeAutomaticSurvey" SelectMode="MultiRow" RefreshInfoFrame="True" Image="change_automatic_survey" />
            <controls:OverlayCommand Key="ChangeTaskChoice" DialogMode="ViewEdit" SelectMode="MultiRow"
                Title="ChangeTaskChoice" Caption="ChangeTaskChoice" RefreshInfoFrame="true" RefreshListFrame="true"
                IDName="IDS" IDColumnName="PersonSID" Width="345" Height="140" Top="100" Url="Persons/ChangeTaskChoice.aspx"
                Image="call_split" />
            <controls:OverlayCommand Key="ChangeLocation" DialogMode="ViewEdit" SelectMode="MultiRow"
                Title="ChangeLocation" Caption="ChangeLocation" RefreshInfoFrame="true" RefreshListFrame="true"
                IDName="IDS" IDColumnName="PersonSID" Width="345" Height="140" Top="100" Url="Persons/ChangeLocation.aspx" Image="change_location" />
            <controls:OverlayCommand Key="ChangeCallGroup" DialogMode="ViewEdit" SelectMode="MultiRow"
                Title="ChangeCallGroup" Caption="ChangeCallGroup" RefreshListFrame="true"
                IDName="IDS" IDColumnName="PersonSID" Width="368" Height="212" Url="Persons/ChangeCallGroup.aspx"
                Image="change_call_group" />
            <controls:OverlayCommand Key="SendMessage" DialogMode="ViewEdit" SelectMode="MultiRow"
                Title="SendMessage" Caption="SendMessage" IDName="IDS" IDColumnName="PersonSID"
                InlineParams="MessageRecipientType=Interviewer" Width="560" Height="390" Top="100" Url="Messaging/SendMessageView.aspx"
                Image="send" />
            <controls:OverlayCommand Key="ChangeSSOIntegration" DialogMode="ViewEdit" SelectMode="MultiRow"
                Title="ChangeSSOIntegration" Caption="ChangeSSOIntegration" RefreshInfoFrame="true" RefreshListFrame="true"
                IDName="IDS" IDColumnName="PersonSID" Width="345" Height="140" Top="100" Url="Persons/ChangeSSOIntegration.aspx"
                Image="account_multiple_check" />
        </Commands>
        <LeftToolbarItems>
            <controls:CheckBox Text="<%$CPResource:IvrAgentCheckboxText%>" ID="cbIvrAgent"
                Checked="False" runat="server" Font-Bold="false" AutoPostBack="true" TextAlign="Right" />
        </LeftToolbarItems>
        <ToolbarItems>
            <controls:ToolbarCommandButton Key="NewInterviewer" />
            <controls:ToolbarCommandButton Key="NewIvrAgent" />
            <controls:ToolbarCommandButton Key="Properties" />
            <controls:ToolbarCommandButton Key="Delete" />
            <controls:XpMenuItem runat="server" ButtonType="Separator" />
            <controls:ToolbarCommandButton Key="Import" />
        </ToolbarItems>
        <DataMenuItems>
            <controls:DataMenuItem Key="NewInterviewer" />
            <controls:DataMenuItem Key="NewIvrAgent" />
            <controls:DataMenuItem Key="Properties" />
            <controls:DataMenuItem Key="Delete" />
            <controls:DataMenuItem Key="AddAssignment" />
            <controls:DataMenuItem Key="ReplaceAssignment" />
            <controls:DataMenuItem Key="ChangeTaskChoice" />
            <controls:DataMenuItem Key="ChangeAutomaticSurvey" />
            <controls:DataMenuItem Key="ChangeLocation" />
            <controls:DataMenuItem Key="ChangeCallGroup" />
            <controls:DataMenuItem Key="ChangeSSOIntegration" />
            <controls:DataMenuItem IsSeparator="True" />
            <controls:DataMenuItem Key="Lock" />
            <controls:DataMenuItem Key="Unlock" />
            <controls:DataMenuItem IsSeparator="True" />
            <controls:DataMenuItem Key="SendMessage" />
        </DataMenuItems>
    </controls:Grid>
</asp:Content>
