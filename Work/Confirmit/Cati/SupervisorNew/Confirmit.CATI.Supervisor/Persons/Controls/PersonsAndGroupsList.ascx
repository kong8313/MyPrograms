<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonsAndGroupsList.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Persons.Controls.PersonsAndGroupsList" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<controls:Grid ID="personsAndGroupsListGrid" runat="server" PrimaryKeyColumn="Id_IsGroup" GridNameWidth="100%">
    <ToolbarItems>
        <asp:Table ID="DialTypeTable" runat="Server" CellPadding="0" CellSpacing="0" HorizontalAlign="Left">
            <asp:TableRow>
                <asp:TableCell><%=Strings.DialTypeName%>:&nbsp;</asp:TableCell>
                <asp:TableCell>
                    <controls:DialTypeDropDownList ID="ddlDialType" runat="server" AddNoChangeOption="True" WrapperCssClass="dropdown-control--medium">
                    </controls:DialTypeDropDownList>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </ToolbarItems>
    <Columns>
        <controls:GeneralGridColumn HeaderText="<%$CPResource:ID%>" Key="Id" SearchColumnType="Number"
            DataFieldName="Id" Width="50px" Hidden="True" />
        <controls:GeneralGridColumn HeaderText="<%$CPResource:Name%>" Key="PersonName" SearchColumnType="Text"
            DataFieldName="Name" Width="200px" SortIndicator="Ascending" />
        <controls:GeneralGridColumn HeaderText="<%$CPResource:Description%>" Key="Name" SearchColumnType="Text"
            DataFieldName="Description" Width="100%" />
        <controls:GeneralGridColumn HeaderText="<%$CPResource:ObjType%>" Key="IsGroup" SearchColumnType="DropDown"
            DataFieldName="IsGroup" Width="75px" EnableSorting="False" />
        <controls:UnboundGeneralGridColumn Key="Id_IsGroup" Hidden="True" />
    </Columns>
</controls:Grid>
