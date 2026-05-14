using System;
using System.Collections.Generic;
using System.Threading;

namespace SimulatorDialerDriver.Distribution
{
    public abstract class BaseGenerator<T> : IGenerator
    {
        internal Dictionary<string, Func<string, T>> Options { get; set; } = new Dictionary<string, Func<string, T>>();
        public string Name { get; }
        
        public abstract string Type { get; }
        public List<GeneratorBehavior> Behaviors { get; set; } = new List<GeneratorBehavior>();

        public abstract T Parse(string value);

        public string Check(string value)
        {
            try
            {
                Parse(value);
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        protected BaseGenerator(string name)
        {
            Name = name;
            Generators.All.Add(this);
        }

        public T GetValue(ContextInfo context, T defaultValue, EventWaitHandle cancelHandle = null)
        {
            foreach (var behavior in Behaviors)
            {
                if (Match(context, behavior))
                {
                    switch (behavior.Type)
                    {
                        case GeneratorBehaviorType.Value:
                            return Parse(behavior.Value);
                        case GeneratorBehaviorType.Manual:
                            var simulator = Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver.Instance;
                            if (simulator != null)
                            {
                                using (var waiter = new ManualResetEvent(false))
                                {
                                    T value = defaultValue;
                                    var name = $"Generate '{Name}'";
                                    var returnValue = new Action<T>(v => { value = v; waiter.Set(); });
                                    var options = new Dictionary<string, Action<string>>()
                                    {
                                        { "default", (result) => returnValue(defaultValue) },
                                        { "custom", (result) => returnValue(Parse(result)) }
                                    };
                                    foreach (var option in Options)
                                    {
                                        options[option.Key] = (result) => returnValue(option.Value(result));
                                    }

                                    string actionId = simulator.Activities.Create(name, behavior.Owner, context, options);

                                    if (cancelHandle == null)
                                    {
                                        waiter.WaitOne();
                                    }
                                    else
                                    {
                                        if (WaitHandle.WaitAny(new[] {waiter, cancelHandle}) == 1)
                                        {
                                            simulator.Activities.TryRemove(actionId, out _);
                                        }
                                    }

                                    return value;
                                }
                            }
                            break;
                            
                    }
                }
            }
            return defaultValue;
        }

        private bool Match(ContextInfo context, GeneratorBehavior behavior)
        {
            if (behavior.Filter == null)
                return true;

            if (behavior.Filter.CompanyId != null && behavior.Filter.CompanyId != context.CompanyId)
                return false;

            if (behavior.Filter.DialerId != null && behavior.Filter.DialerId != context.DialerId)
                return false;

            if (behavior.Filter.CampaignId != null && behavior.Filter.CampaignId != context.CampaignId)
                return false;

            if (behavior.Filter.AgentId != null && behavior.Filter.AgentId != context.AgentId)
                return false;

            if (behavior.Filter.InterviewId != null && context.InterviewId != null && behavior.Filter.InterviewId != context.InterviewId)
                return false;

            return true;
        }
    }
}