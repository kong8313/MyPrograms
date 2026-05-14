<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="SendMessageView.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Messaging.SendMessageView" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">
    <controls:Dialog ID="dialog" runat="server" Title="<%$CPResource:SendMessage%>" Mode="Modal" HideHeader="True">
        <OKButton Text="Send" OnClick="SendClick" OnClientClick="if(!validate()) return false;" />
        <content>
            <main class="content-panel flex-panel-column">
            <asp:Label runat="server" Text= "<%$CPResource:To%>" Font-Bold="True" style="line-height: 20px" />
            <controls:TextBox Rows="2" ReadOnly="true" TextMode="MultiLine" ID="tbSendTo" runat="server" Width="100%" Style="overflow:auto; min-height: 28px;line-height: 14px; resize: vertical;margin-bottom: 10px;" />
            <asp:Label runat="server" Text= "<%$CPResource:Message%>" Font-Bold="True" style="line-height: 20px"/>
            <controls:TextBox MaxLength="1024" TextMode="MultiLine" ID="tbMessageBody" runat="server" Width="100%" style="line-height: 14px; height: 200px; resize: none;margin-bottom: 10px;" CssClass="plain_textbox" />
            <controls:CheckBox ID="cbDeliverToUserNotOnline" runat="server" Text="<%$CPResource:DeliverToUsersNotCurrentlyOnline%>" />
            </main>
        </content>
    </controls:Dialog>
    <script type="text/javascript">
        function validate() {
            var message = document.getElementById('<%=tbMessageBody.ClientID %>').value;

            if (message.trim().length == 0) {
                alert("<%=Strings.ErrorMessageEmpty %>");
                return false;
            }

            if (message.length > 1024) {
                alert("<%=Strings.ErrorMessageTooLong %>".replace("{0}", message.length));
                return false;
            }
            return true;
        }
    </script>
</asp:Content>
