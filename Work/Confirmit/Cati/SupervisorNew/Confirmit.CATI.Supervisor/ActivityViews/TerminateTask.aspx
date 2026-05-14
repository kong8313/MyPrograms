<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true"
    CodeBehind="TerminateTask.aspx.cs" Inherits="Confirmit.CATI.Supervisor.ActivityViews.TerminateTask" %>

<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" runat="server" Mode="Modal" HideHeader="true">
        <OKButton OnClick="OKButtonClick" OnClientClick="if(!validate()) return false;" Text="Log out" />
        <Content>
            <main class="content-panel">
                <div class="flex-panel flex-panel-column">
                    <asp:Label ID="lblConfirmation" runat="server" CssClass="boldLabel"/>

                    <span style="padding: 10px 0px;">Select the option that best describes why you want to log this interviewer out</span>
                    <controls:RadioButtonList runat="server" ID="rblReason">
                        <asp:ListItem Text="No reason specified" Value="0" Selected="True" />
                        <asp:ListItem Text="Interviewer did not log out" Value="1" />
                        <asp:ListItem Text="Interviewer is not receiving calls" Value="2" />
                        <asp:ListItem Text="Interviewer console is unresponsive" Value="3" />
                        <asp:ListItem Text="Telephony related error" Value="4" />
                    </controls:RadioButtonList>

                    <div style="padding-top: 10px;"><%=Strings.TerminateTaskAdditionalComments %></div>
                    <controls:TextBox ID="tbxComments" runat="server" TextMode="MultiLine" style="resize: none;height:62px;width: 100%;" CssClass="textarea-framed"></controls:TextBox>
                </div>
            </main>
        </Content>
    </controls:Dialog>
    <script type="text/javascript">
        function validate() {
            var comments = document.getElementById('<%=tbxComments.ClientID %>').value;

            if (comments.length > 1024) {
                alert("<%=Strings.Err_CommentsAreTooLong %>".replace("{0}", comments.length));
                return false;
            }

            return true;
        }

    </script>
</asp:Content>
