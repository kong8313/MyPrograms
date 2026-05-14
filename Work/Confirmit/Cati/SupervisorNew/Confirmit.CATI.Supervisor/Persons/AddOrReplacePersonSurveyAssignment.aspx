<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="AddOrReplacePersonSurveyAssignment.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Persons.AddOrReplacePersonSurveyAssignment" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog runat="server" ID="dialogControl" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" />
        <Content>
            <controls:Grid ID="surveyListGrid" runat="server" PrimaryKeyColumn="Id" GridNameWidth="100%" SortedColumnName="DefaultOrderID">
                <ToolbarItems>
                    <controls:CheckBox ID="cbRecent" runat="server" AutoPostBack="True" Text="<%$CPResource:Recent%>" />
                    <asp:Table ID="DialTypeTable" runat="Server" CellPadding="0" CellSpacing="0" HorizontalAlign="Left">
                        <asp:TableRow>                            
                            <asp:TableCell style="padding-left: 20px; padding-right: 5px"><%=Strings.DialTypeName%>:</asp:TableCell>
                            <asp:TableCell>
                                <controls:DialTypeDropDownList ID="ddlDialType" runat="server" AddNoChangeOption="True" WrapperCssClass="dropdown-control--medium">
                                </controls:DialTypeDropDownList>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </ToolbarItems>

                <Columns>
                    <controls:GeneralGridColumn HeaderText="ID" Key="Id" SearchColumnType="Number" DataFieldName="Id"
                        Width="50" Hidden="True" />
                    <controls:GeneralGridColumn HeaderText="DefaultOrderID" Key="DefaultOrderID" SearchColumnType="Number" DataFieldName="DefaultOrderID"
                                                Width="50" Hidden="True" />
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectId%>" Key="ConfirmitID" SearchColumnType="Text"
                        DataFieldName="ConfirmitID" Width="100px" SortIndicator="Ascending" />
                    <controls:GeneralGridColumn HeaderText="<%$CPResource:ProjectName%>" Key="Name" SearchColumnType="Text"
                        DataFieldName="Name" Width="100%" />
                </Columns>
            </controls:Grid>
        </Content>
    </controls:Dialog>
</asp:Content>
