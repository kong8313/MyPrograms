<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PersonsTreeControl.ascx.cs"
	Inherits="Confirmit.CATI.Supervisor.Controls.PersonsTreeControl" %>

<asp:PlaceHolder ID="phFilter" runat="server">
	<table width="100%" cellpadding="1" cellspacing="0">
		<tr>
			<td width="20%" nowrap>
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
			<td align="right" nowrap="nowrap">
				<controls:Button ID="btnFindFirst" runat="server" Text="<%$CPResource:First%>" Width="60" OnClick="btnFindFirst_Click" />							
				<controls:Button ID="btnFindNext" runat="server" Text="<%$CPResource:Next%>" Width="60" OnClick="btnFindNext_Click" />							
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
			<td align="right" nowrap="nowrap">
				<controls:Button ID="btnFilter" runat="server" ResName="Apply" Width="60" OnClick="btnFilter_Click" />
				<controls:Button ID="btnReset" runat="server" ResName="Reset" Width="60" OnClick="btnReset_Click" />
			</td>						
		</tr>
	</table>
</asp:PlaceHolder>
<div style="margin-top: 3px; margin-left: 1px; margin-right: 1px; border-style: dotted;
	border-width: 1px; border-color: #a5a2a5; background-color: white;" >
	<controls:BaseTreeControl runat="server" ID="tree" Font-Size="9pt" Font-Names="Tahoma" 
		EnableAjax="false" SelectionType="Single" Height="100%" Width="100%" Visible="true">
		<NodeSettings ParentNodeImageUrl="~/images/small/icon_group_closed.gif"
			LeafNodeImageUrl="~/images/small/p.gif" />
	</controls:BaseTreeControl>
</div>