using System;

namespace Confirmit.CATI.Supervisor.Classes.Script
{
    public enum ShiftStatus
    {
        Default,
        HasOverridden,
        Current
    }
    public enum CurrentDisplay
    {
        Shifts = 0,
        Exclusions = 1,
        Both
    };

    public enum GridBandType
    {
        Rules = 0,
        Subrules = 1,
        Actions = 2
    }

    public enum ConvertDirection
    { 
       FromClient = 0,
       ToClient = 1
    }
}
