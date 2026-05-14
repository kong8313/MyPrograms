using System.Collections.Generic;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    public class CatiConsolePropertiesContainer
    {
        public int MessageInterval { get; set; }

        public bool ShowRedialButton { get; set; }

        public bool ShowInternalCallTransferButton { get; set; }

        public bool ShowExternalCallTransferButton { get; set; }

        public bool EnablePreviousPageToolbarButton { get; set; }

        public bool EnableNextPageToolbarButton { get; set; }

        public bool EnableAppointmentToolbarButton { get; set; }

        public bool EnableRedoToolbarButton { get; set; }

        public bool EnableFastForwardToolbarButton { get; set; }

        public bool EnableCheckSpellingToolbarButton { get; set; }

        public bool EnableRedialToolbarButton { get; set; }

        public bool EnableInternalCallTransferButton { get; set; }

        public bool EnableExternalCallTransferButton { get; set; }

        public bool EnableHangUpToolbarButton { get; set; }

        public bool EnableLogoutAfterFinishToolbarButton { get; set; }

        public bool EnableTerminateToolbarButton { get; set; }

        public bool EnableTakeBreakToolbarButton { get; set; }

        public bool EnableChangeTaskChoiceToolbarButton { get; set; }

        public bool EnableMessageFormToolbarButton { get; set; }

        public bool EnableAppointmensListToolbarButton { get; set; }

        public bool EnableRefreshToolbarButton { get; set; }

        public bool EnableLogoutToolbarButton { get; set; }

        public bool EnableRedialNewNumberRedialDialogAbility { get; set; }

        public bool EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes { get; set; }

        public bool EnableAbilityToCancelDial { get; set; }

        public bool EnablePersistentConnectionClosing { get; set; }

        public int KeepAliveCallsToSave { get; set; }

        public int GoodConnectionThresholdMs { get; set; }

        public int NormalConnectionThresholdMs { get; set; }

        public int NoCallsTimeout { get; set; }

        public List<BreakType> BreakTypes { get; set; }

        public bool ForceUpdateToNewVersion { get; set; }
    }
}