using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.PersonLogin
{
    public class CatiConsolePropertiesContainer
    {
        public int MessageInterval { get; set; }

        public bool ShowRedialButton { get; set; }

        public bool EnablePreviousPageToolbarButton { get; set; }

        public bool EnableNextPageToolbarButton { get; set; }

        public bool EnableAppointmentToolbarButton { get; set; }

        public bool EnableRedoToolbarButton { get; set; }

        public bool EnableFastForwardToolbarButton { get; set; }

        public bool EnableCheckSpellingToolbarButton { get; set; }

        public bool EnableRedialToolbarButton { get; set; }

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

        public bool UseHttpsForConsoleStateService { get; set; }

        public int NoCallsTimeout { get; set; }

        public List<BvBreakTypeEntity> BreakTypes { get; set; }
    }
}