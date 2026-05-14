<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="true" CodeBehind="DoubleListBox.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Controls.DoubleListBox" %>
<input id="leftIDs" type="hidden" runat="server" />
<input id="rightIDs" type="hidden" runat="server" />
<table width="100%">
    <tr style="font-weight: bold">
        <td>
            <asp:Label ID="leftCaption" runat="server" /></td>
        <td></td>
        <td>
            <asp:Label ID="rightCaption" runat="server" /></td>
    </tr>
    <tr>
        <td width="40%">
            <controls:ListBox ID="leftList" runat="server" EnableViewState="true" Width="100%" />
        </td>
        <td align="center" width="20%">
            <controls:ImageButton ID="bttnRight" runat="server" IsSubmit="false" ImageName="arrow_forward" />
            <br />
            <controls:ImageButton ID="bttnLeft" runat="server" IsSubmit="false" ImageName="arrow_back" />
            <br />
        </td>
        <td width="40%">
            <controls:ListBox ID="rightList" runat="server" EnableViewState="true" Width="100%" />
        </td>
    </tr>
</table>
<script type="text/javascript">

    // determines if some items are selected (there are items in right listbox)
    function DoesRightListHaveItems() {
        var rightIDs = document.getElementById("<%=rightIDs.ClientID%>");
        if (rightIDs.value == "")
            return false;
        else return true;
    }

</script>
