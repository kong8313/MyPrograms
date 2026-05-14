using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorDialerDriver.Distribution;

namespace SimulatorDialerDriver
{
    public class SimulatorActivityController 
    {
        public SimulatorActivityController(string name, string owner, ContextInfo context, Dictionary<string, Action<string>> commands)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Owner = owner;
            Context = context;
            Commands = commands;
        }

        public string Id { get; }
        public string Name { get; }
        public string Owner { get; }
        public ContextInfo Context { get; }
        public Dictionary<string, Action<string>> Commands { get; }
    }

    public class SimulatorActivities : ConcurrentDictionary<string, SimulatorActivityController>
    {
        public string Create(string name, string owner, ContextInfo context, Dictionary<string, Action<string>> commands)
        {
            var activity = new SimulatorActivityController(name, owner, context, commands);
            TryAdd(activity.Id, activity);
            return activity.Id;
        }

        public void Execute(string actionId, string command, string args)
        {
            TryGetValue(actionId, out var activity);
            activity.Commands[command](args);
            TryRemove(actionId, out _);
        }
    }
}
