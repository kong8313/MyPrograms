<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Dialog.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Controls.Dialog" %>
<script type="text/javascript">

    Y.on("load", function () {

        Y.one("document").on("keyup", function (evt) {

            if (evt.keyCode == 27) {
                //Escape pressed
                var btnCancel = document.getElementById("<%=cancel.ClientID%>");
                if (btnCancel) {

                    btnCancel.click();

                    var e = Y.Event.getEvent(evt, null, true);
                    if (e) {
                        new Y.DOMEventFacade(e).halt();
                    }

                }
            }
        });
    });

    function changeProcessingState(started) {
        Y.one('#wait_td').setStyle('display', started ? '' : 'none');
    }
</script>
<asp:Panel runat="server" Style="height: 100%; position: relative; overflow: hidden"
    ID="dialogPanel">
    <div id="trHeader" runat="server" class="FrameTableHeader activity-view-header">
        <h2><asp:Label ID="lbTitle" runat="server" /></h2>
    </div>
    <div class="dataarea <%=ShowBottomBorder?"area--with-bottom-border":"" %> <%=!PutActionButtonsInsideGridIfPossible ? "no-bottom-borders-for-grids": "" %>">
        <div class="contentWrapper">
            <asp:PlaceHolder runat="server" ID="phContent" />
        </div>
    </div>
    <div id="wait_td" style="white-space: nowrap; position: absolute; top: 50%; left: 50%; margin-top: -25px; margin-left: -24px; display: none; z-index: 1000">
        <div class="comd-busy-dots comd-busy-dots--large">
            <div class="comd-busy-dots__dot"></div>
            <div class="comd-busy-dots__dot"></div>
            <div class="comd-busy-dots__dot"></div>
        </div>
    </div>
    <div class="dialogStatusButtons" runat="server" id="divButtonsHolder">
        <controls:Button ID="btnOK" ResName="<Action>" runat="server" />
        <controls:Button ID="btnSave" ResName="Dlg_Save" OnClientClick="" runat="server" CssClass="plain_button button-save" />
        <a href="javascript:void(0);" onclick="top.overlay.closeLast()" runat="server" class="plain_button button-cancel"
            id="cancel" style="font-weight: 600; vertical-align: middle">Cancel</a>
    </div>
</asp:Panel>
