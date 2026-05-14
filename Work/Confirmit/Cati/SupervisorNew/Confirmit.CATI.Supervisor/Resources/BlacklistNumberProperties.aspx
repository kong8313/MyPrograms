<%@ Page Language="C#" MasterPageFile="~/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BlacklistNumberProperties.aspx.cs" Inherits="Confirmit.CATI.Supervisor.Resources.BlacklistNumberProperties" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true">
        <OKButton OnClick="OKButtonClick" OnClientClick="if(!validateNumber()) return false; " />
        <Content>
            <main class="content-panel">
                <table class="settings-table--default-columns settings-table--no-min-width settings-table">
                    <tr>
                        <td width="200px">
                            <asp:Label ID="lblTelephoneNumber" Text="<%$CPResource:TelNumber%>" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbTelephoneNumber" runat="server" Width="100%" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblComment" Text="<%$CPResource:Comment%>" runat="server" />
                        </td>
                        <td>
                            <controls:TextBox ID="tbComment" runat="server" Width="100%" MaxLength="74" />
                        </td>
                    </tr>
                </table>
            </main>
        </Content>
    </controls:Dialog>
    <script type="text/javascript">
        const emptyNumberMessage = "Telephone number cannot be empty.";
        const invalidFormatMessage = "The telephone number should only contain digits 0-9 or the * symbol (which may be placed at the end of the number). The total number length cannot be more than 255 characters.";
        const invalidComment = "The comment field supports only characters from the ASCII or single-byte character set (e.g., English letters, numbers, and basic symbols). Characters from languages requiring Unicode (such as Chinese, Arabic, or special symbols) are not supported.";
        function validateNumber() {
            let tbTelephoneNumber = document.getElementById("<%=tbTelephoneNumber.ClientID %>");
            let value = tbTelephoneNumber.value.trim();
            if (value === "") {
                alert(emptyNumberMessage);
                tbTelephoneNumber.focus();
                return false;
            }

            let regEx = new RegExp("^[0-9]{1,255}[\*]?$");
            if (!regEx.test(value)) {
                alert(invalidFormatMessage);
                tbTelephoneNumber.focus();
                return false;
            }

            let tbComment = document.getElementById("<%=tbComment.ClientID %>");
            let comment = tbComment.value;
            if([...comment].some(char => char.charCodeAt(0) > 127))
            {
                alert(invalidComment);
                tbComment.focus();
                return false;
            }
            return true;
        }
    </script>
</asp:Content>
