<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SurveysTreeControl.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.SurveysTreeControl" %>

<asp:PlaceHolder ID="phFilter" runat="server">
    <table width="100%">
        <tr>
            <td nowrap>
                <asp:Label ID="lblFind" runat="server" Text="<%$CPResource:Find%>" Font-Bold="true" />
            </td>
            <td>
                &nbsp;
            </td>
            <td width="80%">
                <controls:TextBox ID="tbxFind" runat="server" Width="100%" />
            </td>
            <td>
                &nbsp;
            </td>
            <td align="right" nowrap>
                <controls:Button ID="btnFindFirst" runat="server" Text="<%$CPResource:First%>" Width="60px"
                    OnClick="btnFindFirst_Click" />
                <controls:Button ID="btnFindNext" runat="server" Text="<%$CPResource:Next%>" Width="60px"
                    OnClick="btnFindNext_Click" />
            </td>
        </tr>
        <tr>
            <td nowrap>
                <asp:Label ID="lblFilter" runat="server" Text="<%$CPResource:PersonFilter%>" Font-Bold="true" />
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                <controls:TextBox ID="tbxFilter" runat="server" Width="100%" />
            </td>
            <td>
                &nbsp;
            </td>
            <td align="right" nowrap>
                <controls:Button ID="btnFilter" runat="server" ResName="Apply" Width="60" OnClick="btnFilter_Click" />
                <controls:Button ID="btnReset" runat="server" ResName="Reset" Width="60" OnClick="btnReset_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <controls:CheckBox ID="chkSortMode" runat="server" AutoPostBack="true" OnCheckedChanged="chkSortMode_CheckedChanged"
                    Text="<%$CPResource:SortSurveysByProjectID%>" />
            </td>
            <td>
                <controls:Button ID="btnCheckOpenSurveys" runat="server" ResName="SelectOnlyOpen"
                    OnClick="btnCheckOpenSurveys_Click" Width="120" />
            </td>
        </tr>
    </table>

</asp:PlaceHolder>
<div style="margin-top: 3; margin-left: 1px; margin-right: 1px; border-style: dotted;
    border-width: 1px; border-color: #a5a2a5;background-color: white;">
	<controls:BaseTreeControl runat="server" ID="tree" Font-Size="9pt" Font-Names="Tahoma"
		EnableAjax="false" SelectionType="Single" Height="100%" Width="100%" Visible="true">
		<NodeSettings ParentNodeImageUrl="~/images/small/icon_surveys_folder_closed.gif"
			LeafNodeImageUrl="~/images/small/icon_survey_folder_closed.gif" />
	</controls:BaseTreeControl>
</div>
