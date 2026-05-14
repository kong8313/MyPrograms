<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="CallManagement.aspx.cs" Inherits="Confirmit.CATI.Supervisor.CallManagement.CallManagement" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content runat="server" ContentPlaceHolderID="Content" ID="Content">
    <input type="hidden" runat="server" id="selectedSurveyId" />
    <style type="text/css">
        tbody tr td.HasAudio {
            background-color: yellow;
        }
    </style>
    <script type="text/javascript">
        function selectSurvey() {
            var settings = { height: "700px", width: "650px", top: "100px" };
            top.overlay.show('<%=Strings.SelectSurvey %>', "CallManagement/Controls/SelectSurvey.aspx", null, settings, null);
            top.overlay.overlayClosedEvent.on(function (args) {
                if (args.result !== true)
                    return;
                if (args.data) {
                    document.getElementById("<%=selectedSurveyId.ClientID %>").value = args.data;
                }
                Common.updatePanel('<%=updatePanel.ClientID %>');
            });
        }

        function columnResized(grid, event) {
            var ddl = document.getElementById("<%=ddlState.ClientID %>");
            var selectedState = ddl.options[ddl.selectedIndex].value;
            window.PageMethods.SetColumnWidth(selectedState, event.get_column().get_key(), event.get_column().get_width());
        }

        function showXpButtonContextMenu(action) {
            var menu;
            var button;
            if (action == 'ShowCustomViewMenu') {
                menu = $find("<%=viewStateContextMenu.ClientID%>");
                button = document.getElementById("<%=CustomViewActions.ClientID%>");
            }
            else {
                menu = $find("<%=advancedFilterContextMenu.ClientID%>");
                button = document.getElementById("<%=AdvancedFilterActions.ClientID%>");
            }

            // show context menu
            if (menu != null) {
                // clear previously selected item.
                if (menu.get_selectedItem()) {
                    menu.get_selectedItem().set_selected(false);
                }

                var yPosition = button.offsetHeight + button.clientHeight;
                var xPosition = button.offsetLeft;

                // now show menu visible in proper place
                menu.showAt(xPosition, yPosition);
                menu.set_visible(true);
            }
        }

        function showCustomViewProperties(type, title, surveyId) {
            var ddl = document.getElementById("<%=ddlState.ClientID%>");
            var selectedViewName = ddl.options[ddl.selectedIndex].text;

            var settings = { height: "700px", width: "650px", top: "100px" };
            var params = { Type: type, SelectedViewName: selectedViewName, SurveyId: surveyId };

            top.overlay.show(title, "CallManagement/Controls/CustomViewProperties.aspx", params, settings, null);
            top.overlay.overlayClosedEvent.on(function (args) {
                if (args.result !== true)
                    return;

                var hiddenInput = document.getElementById("<%=m_ForceSelectedCustomViewName.ClientID%>");
                hiddenInput.value = args.data;

                Common.updatePanel('<%=ClientID%>');
            });
        }

        function deleteCustomView(confirmation) {
            if (!confirm(confirmation)) {
                return;
            }

            var ddl = document.getElementById("<%=ddlState.ClientID%>");
            var selectedState = ddl.options[ddl.selectedIndex].text;

            window.PageMethods.DeleteCustomView(selectedState, <%=SurveyID%>);

            var hiddenInput = document.getElementById("<%=m_ForceSelectedCustomViewName.ClientID%>");
            hiddenInput.value = "Scheduled";

            Common.updatePanel('<%=ClientID%>');
        }

        function deleteAdvancedFilter(confirmation) {
            if (!confirm(confirmation)) {
                return;
            }

            var ddl = document.getElementById("<%=ddlFilter.ClientID%>");
            var filterId = ddl.options[ddl.selectedIndex].value;

            window.PageMethods.DeleteAdvancedFilter(filterId, onDeleteAdvancedFilterComplete);
            
        }

        function onDeleteAdvancedFilterComplete(errorMessage) {
            if (errorMessage != null) {
                alert(errorMessage);
                return;
            }

            var hiddenInput = document.getElementById("<%=m_FilterId.ClientID%>");
            hiddenInput.value = "0";

            Common.updatePanel('<%=ClientID%>');
        }
    </script>

    <controls:UpdatePanel ID="updatePanel" runat="server" style="height: 100%;">
        <ContentTemplate>
            <controls:Grid ID="m_grid" runat="server" PrimaryKeyColumn="InterviewCallID" simplifiedpagermode="true" ShowLastEmptyColumn="True" 
                           ToolbarCssClass="cati-controls-menu cati-controls-menu--small-margins" CssClass="call-management-grid"
                RightToolbarButtons="CloseWindow" TopToolbarLayout="DoubleMenu" PageSize="20" ColumnResizedClientEvent="columnResized">
                <Commands>
                    <controls:OverlayCommand Key="AddConfirmitVariable" Caption="AddQuestion" Image="filter_1"
                        OnServerClick="DoUpdateWithColumns" SelectMode="No" Title="AddQuestion" Height="400" Width="600" Top="100" Url="CallManagement/SelectConfirmitVariables.aspx" />
                    <controls:OverlayCommand Key="New" DialogMode="Create" Caption="Add" Title="Add Call"
                        Url="CallManagement/CallProperties.aspx" SelectMode="No" Image="plus"
                        Width="540" Height="310" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="EditSelected" Caption="SelectedOnly" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="EditFiltered" Caption="EntireList" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="ActivateSelected" Caption="SelectedOnly" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="ActivateFiltered" Caption="EntireList" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="AssignSelected" Caption="SelectedOnly" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="AssignFiltered" Caption="EntireList" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="MoveSelected" Caption="SelectedOnly" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="MoveFiltered" Caption="EntireList" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="MoveAndRescheduleSelected" Caption="SelectedOnly" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="MoveAndRescheduleFiltered" Caption="EntireList" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="ChangePrioritySelected" Caption="SelectedOnly" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="ChangePriorityFiltered" Caption="EntireList" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="ReviewerCreateSessionSelected" Caption="SelectedOnly" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="ReviewerCreateSessionFiltered" Caption="EntireList" OnServerClick="DoUpdate" />
                    <controls:Command Key="ReviewerCreateSessionAndOpenSelected" Caption="SelectedOnly" OnServerClick="ReviewerCreateSessionAndOpenSelectedHandler" />
                    <controls:Command Key="ReviewerCreateSessionAndOpenFiltered" Caption="EntireList" OnServerClick="ReviewerCreateSessionAndOpenFilteredHandler" />
                    <controls:BaseOverlayCommand Key="ChangeShiftTypeSelected" Caption="SelectedOnly" OnServerClick="DoUpdate" />
                    <controls:BaseOverlayCommand Key="ChangeShiftTypeFiltered" Caption="EntireList" OnServerClick="DoUpdate" />
                    <controls:Command Key="DeleteSelected" Caption="SelectedOnly" OnServerClick="DeleteSelected" />
                    <controls:Command Key="DeleteFiltered" Caption="EntireList" OnServerClick="DeleteFiltered"
                        Confirmation="conf_DeleteFilteredCalls" />
                    <controls:Command Key="DisableSelected" Caption="SelectedOnly" OnServerClick="DisableSelected" />
                    <controls:Command Key="DisableFiltered" Caption="EntireList" OnServerClick="DisableFiltered"
                        Confirmation="conf_DisableFilteredCalls" />
                    <controls:Command Key="EnableSelected" Caption="SelectedOnly" OnServerClick="EnableSelected" />
                    <controls:Command Key="EnableFiltered" Caption="EntireList" OnServerClick="EnableFiltered"
                        Confirmation="conf_EnableFilteredCalls" />
                    <controls:Command Key="ShowTimeMode" Caption="ShowTimeInRespondentTZ" Image="time" />
                    <controls:Command Key="RefreshAll" Caption="Refresh" Image="refresh" OnServerClick="DoUpdate" />
                    <controls:OverlayCommand Key="History" Title="History" Caption="History" Url="Surveys/CallListHistoryTabs.aspx"
                        DialogMode="ViewEdit" IDColumnName="InterviewID" IDName="InterviewID" Image="library" Height="650" Width="1250" Top="50" />
                    <controls:OverlayCommand Key="QuotaStatus" Title="QuotaStatus" Caption="QuotaStatus" Url="CallManagement/InterviewQuotaStatus.aspx"
                        DialogMode="ViewEdit" IDColumnName="InterviewID" IDName="InterviewID" Image="chart-pie" Height="550" Width="700" Top="50" />
                    <controls:OverlayCommand Key="ActiveCalls" Title="ActiveCalls" Caption="ActiveCalls" Url="CallManagement/ActiveCalls.aspx"
                        DialogMode="ViewEdit" SelectMode="No" IDColumnName="InterviewID" IDName="InterviewID" Image="active_calls" Height="380" Width="560" Top="50" />
                    <controls:BaseOverlayCommand Key="Export" Caption="Export" Image="export" OnServerClick="OnExport" />
                    <controls:Command Key="RecordingRetrievalMode" Caption="RetrieveAudio" Image="audio" />
                    <controls:Command Key="SwitchTimezone" Caption="ShowTimeInRespondentTZ" Image="time" />
                    <controls:ViewCommand Key="PlayRecordings" Caption="PlayAudio" IDColumnName="InterviewID" IDName="InterviewID" Width="640" Height="480" FloatingMode="true" SelectMode="SingleRow" Image="play_circle" />
                    <controls:Command Key="SetPreviewSelected" Caption="SelectedOnly" OnServerClick="SetPreviewSelected" />
                    <controls:Command Key="SetPreviewFiltered" Caption="EntireList" OnServerClick="SetPreviewFiltered" />
                    <controls:Command Key="SetSpecialDialSelected" Caption="SelectedOnly" OnServerClick="SetSpecialDialSelected" />
                    <controls:Command Key="SetSpecialDialFiltered" Caption="EntireList" OnServerClick="SetSpecialDialFiltered" />
                    <controls:Command Key="ResetSelected" Caption="SelectedOnly" OnServerClick="ResetDialModeSelected" />
                    <controls:Command Key="ResetFiltered" Caption="EntireList" OnServerClick="ResetDialModeFiltered" />
                    <controls:Command Key="ReviewerOpen" Caption="Go to Reviewer" Image="open_in_new" />
                </Commands>
                <LeftToolbarItems>
                    <controls:XpMenuItem runat="server" ButtonType="Generic">
                            <div class="flex-panel flex-panel-row">
                                <asp:Label runat="server" Text="<%$CPResource:StateLabel%>" Style="padding-right: 10px;" />
                                <controls:DropDownList ID="ddlState" runat="server" AutoPostBack="true" Style="width: 112px;">
                                </controls:DropDownList>
                            </div>
                    </controls:XpMenuItem>
                    <controls:XpMenuItem runat="server" ButtonType="Button" ID="CustomViewActions" ImageName="settings" Visible="True" OnClientClick="showXpButtonContextMenu('ShowCustomViewMenu');" />
                    <asp:Table ID="Table2" runat="Server" CellPadding="0" CellSpacing="0" HorizontalAlign="Left" style="width: 180px; margin-left: 10px;">
                        <asp:TableRow>
                            <asp:TableCell>
                                     <asp:Label runat="server" Text="<%$CPResource:AdvancedFilter%>&nbsp;" class="toolbar-label"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell>
                                <controls:DropDownList ID="ddlFilter" runat="server" Width="140" AutoPostBack="true"
                                    MaintainSelectedItemDuringDataBind="True" />
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                    <controls:XpMenuItem runat="server" ButtonType="Button" ID="AdvancedFilterActions" ImageName="filter_list" CssClass="toolbar-button--with-right-margin" Visible="True" OnClientClick="showXpButtonContextMenu('ShowAdvancedFilterMenu');" />                    
                    <controls:ToolbarCommandButton Key="AddConfirmitVariable" />
                    <controls:ToolbarCommandButton Key="RecordingRetrievalMode" ButtonType="ToggleButton"
                        ID="btnRetrieveAudio" runat="server" />
                    <controls:ToolbarCommandButton Key="SwitchTimezone" ButtonType="ToggleButton" ID="cbShowTimeMode"
                        runat="server" />
                    <controls:XpMenuItem runat="server" ButtonType="Button" ID="SelectSurveyButton" Text="<%$CPResource:Surveys%>" ImageName="assignment_turned_in" Visible="True" OnClientClick="selectSurvey();">
                    </controls:XpMenuItem>
                    <controls:XpMenuItem runat="server" ButtonType="Generic" Width="150px">
                        <controls:CheckBox runat="server" ID="CallsAvailableNowCheckBox" CssClass="toolbar-checkbox--with-left-padding" AutoPostBack="true"  ToolTip="<%$CPResource:CallsAvailableNowToolTip%>"
                                           Text="<%$CPResource:CallsAvailableNow%>" OnCheckedChanged="DoUpdateWithColumns" />
                    </controls:XpMenuItem>
                </LeftToolbarItems>
                <ToolbarItems>
                    <controls:XpMenuItem runat="server" ButtonType="Separator" />
                    <controls:ToolbarStdBlock />
                    <controls:ToolbarCommandButton Key="ReviewerOpen" />
                    <controls:ToolbarCommandButton Key="New" />
                    <controls:ToolbarCommandButton Key="History" />
                    <controls:ToolbarCommandButton Key="ActiveCalls" />
                    <controls:ToolbarCommandButton Key="Export" />
                </ToolbarItems>
                <DataMenuItems>
                    <controls:DataMenuItem Key="History" />
                    <controls:DataMenuItem Key="PlayRecordings" />
                    <controls:DataMenuItem Key="New" />
                    <controls:DataMenuItem Key="Edit" Text="Edit" ImageUrl="edit">
                        <Items>
                            <controls:DataMenuItem Key="EditSelected" />
                            <controls:DataMenuItem Key="EditFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="Activate" Text="Activate">
                        <Items>
                            <controls:DataMenuItem Key="ActivateSelected" />
                            <controls:DataMenuItem Key="ActivateFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="Delete" Text="Deactivate">
                        <Items>
                            <controls:DataMenuItem Key="DeleteSelected" />
                            <controls:DataMenuItem Key="DeleteFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="Move" Text="Move">
                        <Items>
                            <controls:DataMenuItem Key="MoveSelected" />
                            <controls:DataMenuItem Key="MoveFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="MoveAndReschedule" TextId="MoveAndReschedule">
                        <Items>
                            <controls:DataMenuItem Key="MoveAndRescheduleSelected" />
                            <controls:DataMenuItem Key="MoveAndRescheduleFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="ChangePriority" TextId="ChangePriority">
                        <Items>
                            <controls:DataMenuItem Key="ChangePrioritySelected" />
                            <controls:DataMenuItem Key="ChangePriorityFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="ReviewerCreateSessionAndOpen" TextId="Review">
                        <Items>
                            <controls:DataMenuItem Key="ReviewerCreateSessionAndOpenSelected" />
                            <controls:DataMenuItem Key="ReviewerCreateSessionAndOpenFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="ReviewerCreateSession" TextId="ReviewSession">
                        <Items>
                            <controls:DataMenuItem Key="ReviewerCreateSessionSelected" />
                            <controls:DataMenuItem Key="ReviewerCreateSessionFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="ChangeShiftType" TextId="ChangeShiftType">
                        <Items>
                            <controls:DataMenuItem Key="ChangeShiftTypeSelected" />
                            <controls:DataMenuItem Key="ChangeShiftTypeFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="AssignTo" TextId="AssignTo">
                        <Items>
                            <controls:DataMenuItem Key="AssignSelected" />
                            <controls:DataMenuItem Key="AssignFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="EnableItem" Text="Enable">
                        <Items>
                            <controls:DataMenuItem Key="EnableSelected" />
                            <controls:DataMenuItem Key="EnableFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="DisableItem" Text="Disable" ImageUrl="block">
                        <Items>
                            <controls:DataMenuItem Key="DisableSelected" />
                            <controls:DataMenuItem Key="DisableFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="SetPreviewDialingMode" TextId="SetPreviewMode">
                        <Items>
                            <controls:DataMenuItem Key="SetPreviewSelected" />
                            <controls:DataMenuItem Key="SetPreviewFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="SetSpecialDialDialingMode" TextId="SetSpecialDialDialingMode">
                        <Items>
                            <controls:DataMenuItem Key="SetSpecialDialSelected" />
                            <controls:DataMenuItem Key="SetSpecialDialFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="ResetDialingMode" TextId="ResetDialingMode">
                        <Items>
                            <controls:DataMenuItem Key="ResetSelected" />
                            <controls:DataMenuItem Key="ResetFiltered" />
                        </Items>
                    </controls:DataMenuItem>
                    <controls:DataMenuItem Key="QuotaStatus" />
                </DataMenuItems>
                <Columns>
                    <controls:GeneralGridColumn DataFieldName="InterviewID" SearchColumnType="Number"
                        HeaderText="<%$CPResource:InterviewId%>" Key="InterviewID" Width="100" MinWidth="90">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="TelephoneNumber" SearchColumnType="Text"
                        HeaderText="<%$CPResource:TelNumber%>" Key="TelephoneNumber" Width="140" MinWidth="115">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="RespondentName" SearchColumnType="Text"
                        HeaderText="<%$CPResource:RespondentName%>" Key="RespondentName" Width="140" MinWidth="115">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="DialingMode" SearchColumnType="DropDown"
                        SearchColumnName="DialingMode" HeaderTextId="DialingModeColumnText" Width="80"
                        Key="DialingMode" />
                    <controls:GeneralGridColumn DataFieldName="DialTypeName" SearchColumnType="DropDown"
                        SearchColumnName="DialTypeId" HeaderText="<%$CPResource:DialTypeName%>" Key="DialTypeId" Width="80">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="TimeText" SearchColumnType="DateTime"
                        SearchColumnName="TimeInShift" HeaderText="<%$CPResource:TimeInShift%>" Key="TimeText"
                        Width="150" MinWidth="80">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="Priority" SearchColumnType="Number" HeaderText="<%$CPResource:CallPriority%>"
                        Key="Priority" Width="100" MinWidth="80">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="StateName" SearchColumnType="DropDown"
                        SearchColumnName="TransientState" HeaderText="<%$CPResource:ExtendedStatus%>"
                        Key="StateName" Width="120" MinWidth="105">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="Resource" SearchColumnType="Text" HeaderText="<%$CPResource:AssignedTo%>"
                        Key="Resource" Width="150" MinWidth="80">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="TimezoneName" SearchColumnType="DropDown"
                        SearchColumnName="TimezoneID" HeaderText="<%$CPResource:Timezone%>" Key="TimezoneName"
                        Width="160" MinWidth="70">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="AttemptNumber" SearchColumnType="Number"
                        HeaderText="<%$CPResource:CallAttempts%>" Key="AttemptNumber" Width="85">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="ShiftType" SearchColumnType="TextDropDown"
                        SearchColumnName="ShiftType" HeaderText="<%$CPResource:ShiftTypeName%>" Key="ShiftType"
                        Width="100" MinWidth="70">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="ExpireTimeText" SearchColumnType="DateTime"
                        SearchColumnName="ExpireTime" HeaderText="<%$CPResource:ExpireTime%>" Key="ExpireTimeText"
                        Width="150" MinWidth="100">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="LastCallTimeText" SearchColumnType="DateTime"
                        SearchColumnName="LastCallTime" HeaderText="<%$CPResource:LastCallTime%>" Key="LastCallTimeText"
                        Width="140" MinWidth="110">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="LastInterviewerName" SearchColumnType="Text"
                        SearchColumnName="LastInterviewerName" HeaderText="<%$CPResource:LastInterviewerName%>" Key="LastInterviewerName" Width="130">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="ApptTimeText" SearchColumnType="DateTime"
                        SearchColumnName="Time" HeaderText="<%$CPResource:AppointmentTime%>" Key="ApptTimeText"
                        Width="150" MinWidth="115">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="ExpTimeText" SearchColumnType="DateTime"
                        SearchColumnName="ExpTime" HeaderText="<%$CPResource:AppointmentExpTime%>" Key="ExpTimeText"
                        Width="150" MinWidth="140">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="CallStateText" SearchColumnType="TextDropDown"
                        SearchColumnName="CallState" HeaderText="<%$CPResource:StateName%>"
                        Key="CallState" Width="130">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn DataFieldName="CallID" SearchColumnType="Number" HeaderText="<%$CPResource:CallID%>"
                        Key="CallID" Width="75" Hidden="true">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn HeaderText="" Key="ApptTime" DataFieldName="ApptTime"
                        Width="150" Hidden="true" />
                    <controls:GeneralGridColumn HeaderText="ExpTime" Key="ExpTime" DataFieldName="ExpTime"
                        Width="150" Hidden="true" />
                    <controls:GeneralGridColumn HeaderText="Time" Key="Time" DataFieldName="Time" Width="150"
                        Hidden="true" />
                    <controls:GeneralGridColumn HeaderText="ExpireTime" Key="ExpireTime" DataFieldName="ExpireTime"
                        Width="150" Hidden="true" />
                    <controls:GeneralGridColumn HeaderText="LastCallTime" Key="LastCallTime" DataFieldName="LastCallTime"
                        Width="100" Hidden="true" />
                    <controls:GeneralGridColumn DataFieldName="ReviewStatusText" SearchColumnType="DropDown"
                        SearchColumnName="ReviewStatus" HeaderText="<%$CPResource:ReviewStatus%>" Key="ReviewStatus" Width="140">
                    </controls:GeneralGridColumn>
                    <controls:GeneralGridColumn HeaderText="" Key="InterviewCallID" DataFieldName="InterviewCallID" Hidden="true" />
                </Columns>
            </controls:Grid>
            <input type="hidden" runat="server" id="m_FilterId" name="m_FilterId" />
            <input type="hidden" runat="server" id="m_ExportResult" name="m_ExportResult" />
            <input type="hidden" runat="server" id="m_ForceSelectedCustomViewName" name="m_ForceSelectedCustomViewName" />
            <controls:AntiForgery ID="AntiForgery" SessionName="CallManagementAntiForgery" runat="server" />

            <controls:DataMenu runat="server" ID="viewStateContextMenu" EnableViewState="False">
                <Items>
                </Items>
            </controls:DataMenu>
            <controls:DataMenu runat="server" ID="advancedFilterContextMenu" EnableViewState="False">
                <Items>
                </Items>
            </controls:DataMenu>
        </ContentTemplate>
    </controls:UpdatePanel>
</asp:Content>
