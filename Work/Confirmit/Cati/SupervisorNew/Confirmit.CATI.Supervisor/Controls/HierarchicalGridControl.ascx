<%@ Control Language="c#" AutoEventWireup="true" CodeBehind="HierarchicalGridControl.ascx.cs"
	Inherits="Confirmit.CATI.Supervisor.Controls.HierarchicalGridControl" %>
<%@ Register TagPrefix="controls" TagName="GridToolbar" Src="~/Controls/GeneralToolbar.ascx" %>
<input type="hidden" runat="server" value="" id="hHighlighted" name="hHighlighted" />
<input type="hidden" runat="server" value="" id="hExpandedRows" name="hExpandedRows" />

<div class="hierarchical-grid" style="display: flex; flex-direction: column; height: 100%;">
	<table cellpadding="0" cellspacing="0" border="0" id="gridHeadTable">
		<tr runat="server" id="topToolbarRow" style="height: 25px">
			<td>
				<controls:GridToolbar runat="server" ID="topToolbar" />
			</td>
		</tr>
	</table>
	<div runat="server" id="gridHolder" visible="true" class="gridHolder" style="flex: 1 1 auto; overflow: auto; padding: 0 !important;">
		<iggrid:WebHierarchicalDataGrid ID="m_grid" runat="server" Width="100%" Height="100%" InitialDataBindDepth="1" EnableRelativeLayout="True" CssClass="heirarchical-grid-inner"
			EnableViewState="False" EnableDataViewState="False" EnableAjaxViewState="False" AutoGenerateColumns="false" MaxDataBindDepth="2" InitialExpandDepth="0" EnableAjax="true" OnInitializeRow="InitializeRowHandler" >
			<Behaviors>
				<iggrid:RowSelectors RowNumbering="false" EnableInheritance="true" />
				<iggrid:Activation Enabled="true" />
				<iggrid:ColumnResizing Enabled="True" />
				<iggrid:Selection RowSelectType="Single" CellClickAction="Row">
					<SelectionClientEvents RowSelectionChanged="rowSelectionChangedHandler" />
				</iggrid:Selection>
			</Behaviors>
		</iggrid:WebHierarchicalDataGrid>
	</div>
</div>
