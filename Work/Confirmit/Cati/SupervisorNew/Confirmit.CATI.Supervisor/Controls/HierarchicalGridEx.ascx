<%@ Control Language="C#" AutoEventWireup="true" Codebehind="HierarchicalGridEx.ascx.cs"
	Inherits="Confirmit.CATI.Supervisor.Controls.HierarchicalGridEx" %>
<asp:GridView ID="innerGrid" runat="server" GridLines="None" AutoGenerateColumns="false" OnRowCreated="OnRowCreated" CssClass="hierarchical-grid"
	OnRowDataBound="OnRowDataBound" OnRowCommand="OnRowCommand"  OnSelectedIndexChanged = "OnSelectedChanged" Width="100%" AllowSorting="true" OnSorting="OnSorting">
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
                <asp:ImageButton ID="ibToggle" runat="server" AlternateText=""
                                 CommandName="toggle" CausesValidation="false" CssClass="hierarchical-grid__expand-button"/>
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</asp:GridView>
