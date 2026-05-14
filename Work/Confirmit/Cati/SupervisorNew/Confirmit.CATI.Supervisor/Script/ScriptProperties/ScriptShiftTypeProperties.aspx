<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="ScriptShiftTypeProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Script.ScriptShiftTypeProperties" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
    </style>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClientClick="if(!ValidateChanges()) return false;" OnClick="OKButtonClick" />
        <Content>
            <main class="content-panel">
                <table class="settings-table settings-table--default-columns settings-table--no-min-width settings-table--controls-100percent">
                    <tr>
                        <td nowrap>
                            <asp:Label ID="lblShiftTypeName" Text="Type Name" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbShiftTypeName" runat="server" columnKey="Name" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td nowrap>
                            <asp:Label ID="lblColor" Text="Type Color" runat="server" />
                        </td>
                        <td>
                            <controls:DropDownList ID="ddlColor" runat="server" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 14px"></td>
                        <td>
                            <div id="divColor" style="width: 100%; height: 14px; padding: 0px;" runat="server" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>

    <script language="javascript" type="text/javascript">
        function ChangeColor(ddlColorID, divColorID) {
            var ddlColor = document.getElementById(ddlColorID);
            var divColor = document.getElementById(divColorID);
            divColor.style.background = ddlColor.value;
        }

        function ValidateChanges() {
            var tbShiftTypeName = document.getElementById('<%=tbShiftTypeName.ClientID%>');
            var value = tbShiftTypeName.value.replace(new RegExp("[\\W]+"), "");

            if (value == "") {
                alert("<%=GetResString("Err_EmptyName")%>");
                tbShiftTypeName.focus();
                return false;
            }
            return true;
        }

    </script>

</asp:Content>


