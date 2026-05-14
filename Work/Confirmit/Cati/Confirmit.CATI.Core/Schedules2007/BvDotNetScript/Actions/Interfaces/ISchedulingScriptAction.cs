using BvDotNetScript.ScriptObjects;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces
{
    public interface ISchedulingScriptAction
    {
        void Execute(ExtendedSchedulingAPI api);
    }

    public interface ISchedulingScriptAction<T>
    {
        void Execute(ExtendedSchedulingAPI api, T parameter);
    }
}
