<%@ Page Language="c#" CodeBehind="DateDialog.aspx.cs" AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.DateDialog"
    MasterPageFile="~/MasterPages/Main.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="Content" ID="Content">
    <script language="javascript" type="text/javascript">
        var args = window.dialogArguments;

        function IsDateValid(Date) {
            if (Date.getFullYear() < 1902) {
                return false;
            }
            return true;
        }

        function save() {
            var date = <%=dteCalendar.ClientControllerName%>.getDate();
            var time = <%=dteCalendar.ClientControllerName%>.getTime();
            date.setHours(time.getHours());
            date.setMinutes(time.getMinutes());
            date.setSeconds(time.getSeconds());

            var result = {
                text: <%=dteCalendar.ClientControllerName%>.getText(),
                dateMilliseconds: date.valueOf()
            };

            if (IsDateValid(date)) {
                parent.overlay.closeLast(true, Y.JSON.stringify(result));
            } else {
                document.getElementById("ErrorFlag").innerHTML = "<font color=#CC0000>*</font>";
            }
        }
        function cancel() {
            parent.overlay.closeLast(false);
        }
    </script>
    <div class="date-selection-dialog">
        <div class="flex-panel flex-panel-row">
            <div>
                <controls:DateTimeEdit ID="dteCalendar" runat="server" CalendarExpanded="true" />
            </div>
            <div id="ErrorFlag">
            </div>
        </div>
        <div style="position: absolute; bottom: 10px; right: 10px">
            <controls:Button ID="btnOK" runat="server" IsSubmit="false" ResName="Dlg_Ok" OnClientClick="save()" />
            <controls:Button ID="btnCancel" runat="server" IsSubmit="false" ResName="Dlg_Cancel"
                OnClientClick="cancel()" />
        </div>
    </div>
</asp:Content>
