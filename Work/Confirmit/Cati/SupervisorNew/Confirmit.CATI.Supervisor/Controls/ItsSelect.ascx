<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItsSelect.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.ItsSelect" %>
<%@ Import Namespace="Confirmit.CATI.Supervisor.Resources" %>
<asp:Panel ID="pnlITS" runat="server" CssClass="popup-extender-container" Width="380px" Height="355px">
    <controls:UpdatePanel ID="updatePanelIts" runat="server" class="popup-extender-panel">
        <ContentTemplate>
            <div class="popup-selector">
                <div id="itsList" class="popup-selector__content">
                    <controls:CheckBoxList ID="cblITS" runat="server" RepeatColumns="1" RepeatDirection="Vertical"  
                        AutoPostBack="false" />
                </div>
                <div class="popup-selector__controls popup-selector__controls--its">
                    <a ID="lbtnSelectUnselectAll" href="javascript:void(0)" style="left:20px;"></a>
                    <controls:Button ID="btnSelectITS" runat="server" Text="<%$CPResource:Dlg_Ok%>" IsSubmit="false"
                        OnClientClick="hidePopup()" />
                </div>
            </div>
        </ContentTemplate>
    </controls:UpdatePanel>
</asp:Panel>
<controls:PopupExtender ID="peITS" MasterID="btnITS" SlaveID="pnlITS" runat="server" />
<script type="text/javascript">
    function InitSelectAllButtonLabel() {
        if (IsAllCheckBoxListSelected('<%=cblITS.ClientID%>')) {
            document.getElementById('lbtnSelectUnselectAll').text = '<%=Strings.DeselectAll%>';
        }
        else {
            document.getElementById('lbtnSelectUnselectAll').text = '<%=Strings.SelectAll%>';
        }
    }

    function IsAllCheckBoxListSelected(cbControl) {
        var chkBoxList = document.getElementById(cbControl);
        var chkBoxCount = chkBoxList.getElementsByTagName("input");

        for (var i = 0; i < chkBoxCount.length; i++) {
            if (chkBoxCount[i].checked == false)
                return false;
        }

        return true;
    }

    function pageLoad() {
        Y.one('#lbtnSelectUnselectAll')
            .detach("click")
            .on('click', function () {
                SetCheckBoxListState('<%=cblITS.ClientID%>', 'lbtnSelectUnselectAll', '<%=Strings.SelectAll%>', '<%=Strings.DeselectAll%>');
            });

        InitSelectAllButtonLabel();         

        function SetCheckBoxListState(cbControl, lbtnSelectUnselectControl, selectText, deselectText) {
            var lbtnSelectUnselect = document.getElementById(lbtnSelectUnselectControl);
            var state;
            if (lbtnSelectUnselect.text == selectText) {
                state = true;
                lbtnSelectUnselect.text = deselectText;
            }
            else {
                state = false;
                lbtnSelectUnselect.text = selectText;
            }

            var chkBoxList = document.getElementById(cbControl);
            var chkBoxCount = chkBoxList.getElementsByTagName("input");

            for (var i = 0; i < chkBoxCount.length; i++) {
                chkBoxCount[i].checked = state;
            }
        }         
    }
</script>
