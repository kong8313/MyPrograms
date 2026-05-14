<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Main.Master"
    CodeBehind="TelephoneNumberDialog.aspx.cs" Inherits="Confirmit.CATI.Supervisor.ActivityViews.TelephoneNumberDialog" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <script type="text/javascript">

        Y.on("load", function () {

            Y.one("document").on("keyup", function (evt) {

                if (evt.keyCode == 27) {

                    overlay.closeLast();

                    var e = Y.Event.getEvent(evt, null, true);
                    if (e) {
                        new Y.DOMEventFacade(e).halt();
                    }
                }
            });
        });

    </script>
    <controls:Dialog ID="dialog" Mode="Modal" runat="server" HideHeader="true" ShowSaveButton="True">
        <OKButton OnClick="btnStart_Click" CausesValidation="True" ResName="Start" />
        <SaveButton OnClick="btnOnlyVideo_Click" CausesValidation="False" ResName="VideoOnly" />
        <Content>
            <main class="content-panel">
                <div class="flex-panel flex-panel-row">
                    <asp:Label ID="lblSpecifyTelephoneNumber" Text="<%$CPResource:SpecifyTelephoneNumber%>"
                        runat="server" />
                    <controls:TextFieldValidator ID="TextFieldValidator1" ControlToValidate="tbTelephoneNumber"
                        IsRequired="true" FieldRequredErrorMessage="Err_EmptyTelephoneNumber" ValidationErrorMessage="ErrorIncorrectValue"
                        Text="*" runat="server" />
                </div>

                <controls:TextBox ID="tbTelephoneNumber" Width="100%" runat="server" />

            </main>
        </Content>
    </controls:Dialog>
</asp:Content>
