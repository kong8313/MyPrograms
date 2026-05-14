<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StatusBreakdown.ascx.cs"
	Inherits="Confirmit.CATI.Supervisor.ActivityViews.Controls.StatusBreakdown" %>
<table style="" cellpadding="0" cellspacing="0" border="0">
	<tr>
		<td>
			<div  class="hierarchical-grid__child" onresize="parentNode.style.height=29+offsetHeight-clientHeight;" >
				<asp:Repeater ID="repeater" runat="server" OnItemDataBound="repeater_ItemDataBound">
					<HeaderTemplate>
						<table>
							<tr>
					</HeaderTemplate>
					<ItemTemplate>
						<td>
							<table class="hierarchical-grid__child__item">
								<tr>
									<th>
										<asp:Label ID="lblHeader" runat="server" Width="100%" />
									</th>
								</tr>
								<tr>
									<td>
										<asp:Label ID="lblValue" runat="server" Width="100%" />
									</td>
								</tr>
							</table>
						</td>
					</ItemTemplate>
					<FooterTemplate>
						</tr> </table>
					</FooterTemplate>
				</asp:Repeater>
			</div>
		</td>
	</tr>
</table>
