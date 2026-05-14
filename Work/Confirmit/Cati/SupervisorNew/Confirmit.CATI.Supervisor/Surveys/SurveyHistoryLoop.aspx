<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SurveyHistoryLoop.aspx.cs"
    Inherits="Confirmit.CATI.Supervisor.Surveys.SurveyHistoryLoop" MasterPageFile="~/MasterPages/Main.Master" %>

<%@ Register Src="~/Controls/HierarchicalGridEx.ascx" TagName="HierarchicalGridEx"
    TagPrefix="controls" %>
<%@ Register TagPrefix="controls" TagName="Hint" Src="./../Controls/Hint.ascx" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <style>
        body {
            background-color: white;
        }
        .gridContainer tbody>tr>td {
            cursor: default;
        }
        .gridContainer__wrap tbody>tr>td {
            white-space: pre-line;
        }
        .no-data-placeholder {
            margin: 25px auto 0;
            font-weight: bold;
        }
    </style>
    
    <div class="flex-panel flex-panel-column" style="height: 100%;">
        <controls:Hint ID="gridHint" runat="server" Text="<%$CPResource:SurveyHistoryLoopHint%>" />
        
        <div id="nodata" runat="server" Visible="False" class="no-data-placeholder">
            <asp:Label runat="server" Text="<%$CPResource:CallHistoryInfoNotSupported%>"/>
        </div>
        <div id="data" runat="server" class="flex-panel flex-panel-column" style="height: 100%;overflow: auto">
            <controls:GeneralToolbar ID="toolbar" runat="server" ToolbarLayout="LabelAndMenu">
                <RightMenuItems>
                    <controls:XpMenuItem ID="btnRefresh" runat="server" ButtonType="Button" ImageName="refresh"
                        Text="<%$CPResource:Refresh%>" OnClick="btnRefresh_Click">
                    </controls:XpMenuItem>
                    <controls:XpMenuItem ID="btnWrap" runat="server" ButtonType="ToggleButton" ImageName="format-text-wrapping-wrap"
                        TextId="WrapText"  ToggleButtonPressed="True" OnClientClick="toggleWrap(); return false;" CssClass="toggleWrap" >
                    </controls:XpMenuItem>
                </RightMenuItems>
            </controls:GeneralToolbar>

            <div class="content-panel flex-panel flex-panel-column" style="flex: 1 1 auto; display: flex;">
                <div class="scrollable-container gridContainer gridContainer__wrap">
                    <controls:HierarchicalGridEx GridLines="Both" AutoGenerateColumns="true" ID="grid" AllowSorting="false"
                        runat="server" HideToggleColumn="true" OnRowDataBound="grid_OnRowDataBound">
                    </controls:HierarchicalGridEx>
                </div>
            </div>
        </div>
    </div>
    <script>
    function toggleWrap() {
        Y.one('.gridContainer').toggleClass("gridContainer__wrap");
        Y.one('.toggleWrap').toggleClass("XpButton").toggleClass("XpButtonPressed");
    }
    </script>

</asp:Content>
