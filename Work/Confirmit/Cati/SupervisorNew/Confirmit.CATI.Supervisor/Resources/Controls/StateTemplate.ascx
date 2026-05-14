<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StateTemplate.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Resources.Controls.StateTemplate" %>
<asp:Panel ID="pnlTemplate" runat="server" Width="300px" Style="background-color: LightGray; border: 1px outset white; position:relative; ">
   <table cellspacing="2" cellpadding="2" width="100%">
        <tr>
            <td style="text-align: left; width: 30%;" >
                <asp:Label ID="lblID" Text="<%$CPResource:ID%>" runat="server" />
            </td>
            <td style="text-align: left; width: 50%; padding-right:10px;">
                <controls:TextBox ID="tbxID" runat="server" columnKey="ID" Width="100%" Enabled="false" style="margin-left:4;"/>
            </td>
        </tr>
        <tr>
            <td style="text-align: left; width: 30%;" >
                <asp:Label ID="lblName" Text="<%$CPResource:Name%>" runat="server" />
            </td>
            <td style="text-align: left; width: 50%; padding-right:10px;" >
                <controls:TextBox ID="tbxName" runat="server"  Width="100%" style="margin-left:4;" />
            </td>
        </tr>
        <tr>
            <td style="text-align: left; width: 30%;" >
                <asp:Label ID="lblPriority" Text="<%$CPResource:Priority%>" runat="server" />
            </td>
            <td style="text-align: left; width: 50%; padding-right:8px;" >
            <controls:NumericEdit id="tbxPriority" runat="server" Nullable="False" columnKey="Priority" 
                                  Width="100%" HorizontalAlign="left"  MinValue="1" style="margin-left:4;">
			</controls:NumericEdit>
        </tr>
        <tr>
            <td style="text-align: left; width: 30%; " >
                <asp:Label ID="lblDA" Text="<%$CPResource:DA%>" runat="server" />
            </td>
            <td style="text-align: left;" >
                <input type="checkbox" ID="cbDA" runat="server" columnKey="DisallowActivation" Width="100%" style="margin-left:0px;"/>
            </td>
        </tr>
    </table>
    <div style="text-align: right; margin: 5px;">
        <input id="igtbl_reOkBtn" class="plain_button" onclick="StateTemplate.okClick('<%=tbxName.ClientID%>', '<%=tbxPriority.ClientID%>');"
            type="button" value="OK" name="igtbl_reOkBtn">&nbsp;&nbsp;
        <input id="igtbl_reCancelBtn" class="plain_button" onclick="gridTemplate.closeTemplate(false);"
            type="button" value="Cancel" name="igtbl_reCancelBtn">
    </div>
</asp:Panel>
    

