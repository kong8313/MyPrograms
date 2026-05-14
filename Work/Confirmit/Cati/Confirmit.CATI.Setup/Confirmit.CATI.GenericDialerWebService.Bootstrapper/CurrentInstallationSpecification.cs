using BootstrapperLibrary;

namespace Confirmit.CATI.GenericDialerWebService.Bootstrapper
{
    public class CurrentInstallationSpecification
    {
#if Generic
        public const GenericDialerInstallationType CurrentGenericDialerInstallationType = GenericDialerInstallationType.Generic;
#endif

#if Simulator
        public const GenericDialerInstallationType CurrentGenericDialerInstallationType = GenericDialerInstallationType.SimulatorGeneric;
#endif

#if SimulatorLtu
        public const GenericDialerInstallationType CurrentGenericDialerInstallationType = GenericDialerInstallationType.LtuSimulatorGeneric;
#endif

#if x64
        public const SystemType CurrentInstallationSystemType = SystemType.x64;
#endif

#if x86
        public const SystemType CurrentInstallationSystemType = SystemType.x86;
#endif
    }
}