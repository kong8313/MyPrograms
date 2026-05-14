<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="DateTimeEdit.ascx.cs"
    Inherits="Confirmit.CATI.Supervisor.Controls.DateTimeEdit" %>
<div class="date-time-edit__control flex-panel flex-panel-row">
    <controls:DatePicker ID="wdteDate" Style="width: 110px;" CssClass="date-time-edit__date"
                         runat="server"  Nullable="false" UseLastGoodDate="True">
    </controls:DatePicker>
    <controls:DateTimeEditor ID="wdteTime" runat="server" HorizontalAlign="Center" CssClass="date-time-edit__time" style="width: 110px;"
                             EditModeFormat="H:mm:ss" Nullable="false" MinimumNumberOfValidFields="3">
        <Buttons SpinButtonsDisplay="OnRight">
        </Buttons>
    </controls:DateTimeEditor>
</div>
<script language="javascript" type="text/javascript">
    function DateTimeEdit(settings) {
        // Controls are initialized by Infragistics's scripts, so they are not available at this point.
        
        this.setEnabled = function (enabled) {
            var dateCtrl = $IG.WebTextEditor.find(settings.DateControlId);
            var timeCtrl = $IG.WebTextEditor.find(settings.TimeControlId);

            dateCtrl.set_enabled(enabled);
            timeCtrl.set_enabled(enabled);
        };

        this.getDate = function () {
            var dateCtrl = $IG.WebTextEditor.find(settings.DateControlId);
            return dateCtrl.get_value();
        };

        this.getTime = function () {
            var timeCtrl = $IG.WebTextEditor.find(settings.TimeControlId);
            return timeCtrl.get_value();
        };

        this.getText = function () {
            $IG.WebTextEditor.find(settings.DateControlId);
            var dateCtrl = $IG.WebTextEditor.find(settings.DateControlId);
            var timeCtrl = $IG.WebTextEditor.find(settings.TimeControlId);

            return dateCtrl.get_text() + " " + timeCtrl.get_text();
        };
    }

</script>
