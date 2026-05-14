<%@ Control Language="C#" AutoEventWireup="true"
    Codebehind="DropDownCheckBoxList.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.DropDownCheckBoxList" %>
<Controls:TextBox ID="tbText" runat="server" Width="150" OnKeyDown="return false;" />
<asp:Panel ID="panel" runat="server" CssClass="popupPanelStyle" Width="200">
	<asp:UpdatePanel id=updatePanel runat=server>
	<ContentTemplate>
		<div style="overflow: auto; height: <%=DropDownPanelHeight.ToString()%>;">
			<controls:CheckBoxList ID="cblList" runat="server" AutoPostBack="false" />
		</div>
		<div style="text-align: right; margin: 5px;">
			<Controls:Button ID="btnConfirm" runat="server" OnClick="btnConfirm_Click" ResName="OK" />
			<Controls:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" ResName="Cancel" />
		</div>
	</ContentTemplate>
	</asp:UpdatePanel>
</asp:Panel>
<ACToolKit:PopupControlExtender ID="pcExtender" runat="server" TargetControlID="tbText"
    PopupControlID="panel" CommitProperty="value" Position="Bottom">
</ACToolKit:PopupControlExtender>