<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="SrvInfo.Summary.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Surveys.Controls.SrvInfo_Summary" %>
<div class="tab-content">
    <script type="text/javascript">
        Common.onGlobalEvent("FiltersListChangedEvent", function () {
            document.location.href += "";
        });
    </script>
    <controls:Grid ID="gridSummary" runat="server" EnablePaging="false" HideSelectedColumn="true" TopToolbarLayout="DoubleMenu" ShowFullToolbarBorders="True">
        <LeftToolbarItems>
            <asp:Label ID="GridTitle" runat="server" CssClass="srvinfo-summary-grid-title"></asp:Label>
            <controls:CheckBox runat="server" ID="cbExludeFreshSampleStatus"
                Text="<%$CPResource:ExcludeFreshSampleStatus%>" Checked="False" AutoPostBack="true" TextAlign="Right" />
        </LeftToolbarItems>
        <ToolbarItems>
            <asp:Table ID="Table1" runat="Server" CellPadding="0" CellSpacing="0" HorizontalAlign="Left"
                Width="180px">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Label ID="Label1" runat="server" Text="<%$CPResource:AdvancedFilter%>" CssClass="toolbar-label" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <controls:DropDownList ID="ddlFilter" runat="server" Width="140" AutoPostBack="true"
                            MaintainSelectedItemDuringDataBind="True" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <controls:XpMenuItem runat="server" ButtonType="Separator" />
            <controls:ToolbarStdBlock />
        </ToolbarItems>
        <Columns>
            <controls:GeneralGridColumn HeaderText="ID" Key="Id" DataFieldName="Id" Width="50" />
            <controls:GeneralGridColumn HeaderText="Name" Key="Name" DataFieldName="Name" Width="100%" />
            <controls:GeneralGridColumn HeaderText="Total Count" Key="TotalCount" DataFieldName="TotalCount"
                Width="100" />
            <controls:GeneralGridColumn HeaderText="Enabled" Key="EnabledCallCount" DataFieldName="EnabledCallCount"
                Width="100" />
            <controls:GeneralGridColumn HeaderText="Disabled by quota" Key="FcdDisabledCallCount" DataFieldName="FcdDisabledCallCount"
                Width="100" />
            <controls:GeneralGridColumn HeaderText="Disabled" Key="UserDisabledCallCount" DataFieldName="UserDisabledCallCount"
                Width="100" />
        </Columns>
    </controls:Grid>
</div>
