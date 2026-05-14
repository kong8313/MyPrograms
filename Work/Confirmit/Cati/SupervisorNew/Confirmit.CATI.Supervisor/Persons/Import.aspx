<%@ Page AutoEventWireup="true" Buffer="true" CodeBehind="Import.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Persons.Import"
    MasterPageFile="~/MasterPages/Main.Master" Language="C#" %>

<%@ Import Namespace="Confirmit.CATI.Core.Services.PersonImport" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="Server">
    <script type="text/javascript">

        var cell = null;

        //Sets row for specified column (detect using global variable 'cell')
        function setRole(role, hText) {
            var grid = cell.get_grid();
            var columns = grid.get_columns();

            for (var i = 0; i < columns.get_length(); i++) {
                var column = columns.get_column(i);
                if (column.columnRole == role) {
                    column.columnRole = null;
                    column.get_headerElement().innerHTML = "";
                    break;
                }
            }

            column = cell.get_column();
            column.columnRole = role;
            column.get_headerElement().innerHTML = hText;
        }

        //Sets role (Group, Login, Password, Description) for each column in the grid depending on data from first row
        function autoAssignFromFirstRow() {
            var grid = $find('<%=m_grid.GridClientId %>');
            var r = grid.get_rows().get_row(0);

            for (var i = 0; i < r.get_cellCount(); i++) {
                cell = r.get_cell(i);
                var cVal = cell.get_value();
                if (cVal == null)
                    continue;
                if (cVal.toLowerCase().indexOf('group') > -1) {
                    setRole('<%=(int)ColumnRole.Group%>', '<%=GetResString(ColumnRole.Group.ToString())%>');
                    continue;
                }
                if (cVal.toLowerCase().indexOf('login') > -1) {
                    setRole('<%=(int)ColumnRole.Login%>', '<%=GetResString(ColumnRole.Login.ToString())%>');
                    continue;
                }
                if (cVal.toLowerCase().indexOf('password') > -1) {
                    setRole('<%=(int)ColumnRole.Password%>', '<%=GetResString(ColumnRole.Password.ToString())%>');
                    continue;
                }
                if (cVal.toLowerCase().indexOf('person') > -1) {
                    setRole('<%=(int)ColumnRole.PersonDescription%>', '<%=GetResString(ColumnRole.PersonDescription.ToString())%>');
                    continue;
                }
                if (cVal.toLowerCase().indexOf('choice') > -1) {
                    setRole('<%=(int)ColumnRole.TaskChoice%>', '<%=GetResString(ColumnRole.TaskChoice.ToString())%>');
                    continue;
                }
                if (cVal.toLowerCase().indexOf('location') > -1) {
                    setRole('<%=(int)ColumnRole.PersonLocation%>', '<%=GetResString(ColumnRole.PersonLocation.ToString())%>');
                    continue;
                }
                if (cVal.toLowerCase().indexOf('automaticsurvey') > -1) {
                    setRole('<%=(int)ColumnRole.AutomaticSurvey%>', '<%=GetResString(ColumnRole.AutomaticSurvey.ToString())%>');
                    continue;
                }
            }
        }

        function autoAssignFromColumnOrder() {
            var r = cell.get_row();
            for (var i = 0; i < r.get_cellCount(); i++) {
                cell = r.get_cell(i);
                if (i == '<%=(int)ColumnRole.Group%>') {
                    setRole('<%=(int)ColumnRole.Group%>', '<%=GetResString(ColumnRole.Group.ToString())%>');
                    continue;
                }
                if (i == '<%=(int)ColumnRole.Login%>') {
                    setRole('<%=(int)ColumnRole.Login%>', '<%=GetResString(ColumnRole.Login.ToString())%>');
                    continue;
                }
                if (i == '<%=(int)ColumnRole.Password%>') {
                    setRole('<%=(int)ColumnRole.Password%>', '<%=GetResString(ColumnRole.Password.ToString())%>');
                    continue;
                }
                if (i == '<%=(int)ColumnRole.PersonDescription%>') {
                    setRole('<%=(int)ColumnRole.PersonDescription%>', '<%=GetResString(ColumnRole.PersonDescription.ToString())%>');
                    continue;
                }
                if (i == '<%=(int)ColumnRole.TaskChoice%>') {
                    setRole('<%=(int)ColumnRole.TaskChoice%>', '<%=GetResString(ColumnRole.TaskChoice.ToString())%>');
                    continue;
                }
                if (i == '<%=(int)ColumnRole.PersonLocation%>') {
                    setRole('<%=(int)ColumnRole.PersonLocation%>', '<%=GetResString(ColumnRole.PersonLocation.ToString())%>');
                    continue;
                }
                if (i == '<%=(int)ColumnRole.AutomaticSurvey%>') {
                    setRole('<%=(int)ColumnRole.AutomaticSurvey%>', '<%=GetResString(ColumnRole.AutomaticSurvey.ToString())%>');
                    continue;
                }
            }
        }

        function GetMode() {
            return $get("<%=Mode.ClientID %>").value;
        }

        function grid_ContextMenu(sender, args) {
            if (GetMode() != "1") return;

            var type = args.get_type();
            if (type == "cell") {
                // select clicked row
                cell = args.get_item();
                row = cell.get_row();
                col = cell.get_column();
            }
        }

        function grid_Initialize() {
            var sender = $find('<%=m_grid.GridClientId %>');

            if (!sender)
                return;

            if (GetMode() != "1") {
                return;
            }

            // Clear column headers
            for (var i = 0; i < sender.get_columns().get_length(); i++) {
                var column = sender.get_columns().get_column(i);
                column.get_headerElement().innerHTML = "&nbsp;";
            }

            var row = sender.get_rows().get_row(0);
            if (row) {
                cell = row.get_cell(0);

                //initialize grid header
                autoAssignFromFirstRow();
            }
        }

        //Is invoked on page submit
        //Writes information about mapping name and column of column into hidden field
        function Prepare() {
            if (GetMode() != "1") return;
            var str = "";
            if (cell != null) {
                var columns = cell.get_grid().get_columns();
                for (var i = 0; i < columns.get_length(); i++) {
                    var column = columns.get_column(i);
                    if (column.get_key() != null && column.columnRole != null)
                        str += (column.get_key() + "=" + column.columnRole + ";");
                }
            }
            $get("<%=ColRoles.ClientID %>").value = str;
        }

        Y.on("load", function () {
            Y.all("form").on("submit", Prepare);
        });

    </script>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <input id="Mode" runat="server" name="Mode" type="hidden" value="0" />
    <input id="ColRoles" runat="server" name="ColRoles" type="hidden" value="" />
    <controls:Dialog ID="dialogControl" runat="server" EnableViewState="true" HideHeader="True"
        Mode="Modal">
        <OKButton OnClientClick="Prepare()" Text="Continue"></OKButton>
        <Content>
            <main class="content-panel">
                <table width="100%" runat="server" id="Upload" class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td nowrap width="0%">
                            <asp:Label ID="uploadLabel" runat="server" />&nbsp;
                        </td>
                        <td>
                            <input class="plain_textbox" type="file" runat="server" id="FileBox" name="FileBox"
                                size="75" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <controls:CheckBox ID="ReportCheckBox" runat="server" Checked="true"></controls:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div id="divPersonImportHelp" class="plain_text" runat="server" />
                        </td>
                    </tr>
                    <tr height="100%">
                        <td colspan="2">&nbsp;
                        </td>
                    </tr>
                </table>
                <div class="scrollable-container" runat="server" ID="divReport" Visible="False"><asp:Literal ID="Report" runat="server" /></div>
                <div style="flex: 1 1 auto; padding-bottom: 10px;" id="gridContainer" runat="server" visible="False">
                    <controls:Grid runat="server" ID="m_grid" EnableSorting="False" EnablePaging="False" HintText="Right-click on the column to change mapping"
                        HideSelectedColumn="True" HideToolBar="True">
                        <DataMenuItems>
                            <controls:DataMenuItem TextId="ColumnRole" Key="ColumnRole">
                            </controls:DataMenuItem>
                            <controls:DataMenuItem TextId="AutoAssign" Key="AutoAssign">
                                <Items>
                                    <controls:DataMenuItem TextId="AutoAssignFromFirstRow" NavigateUrl="javascript:autoAssignFromFirstRow()" />
                                    <controls:DataMenuItem TextId="AutoAssignFromColumnOrder" NavigateUrl="javascript:autoAssignFromColumnOrder()" />
                                </Items>
                            </controls:DataMenuItem>
                        </DataMenuItems>
                    </controls:Grid>
                </div>

                <table width="100%" runat="server" id="importOptions" class="settings-table settings-table--default-columns settings-table--no-min-width">
                    <tr>
                        <td>
                            <controls:CheckBox ID="ImportFirstRow" runat="server" Checked="false" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <controls:CheckBox ID="OverrideDataAndMembership" runat="server" Checked="false" Visible="false" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
