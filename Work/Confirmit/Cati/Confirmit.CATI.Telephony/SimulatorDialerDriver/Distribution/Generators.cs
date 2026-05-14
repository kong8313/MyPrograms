using System;
using System.Collections.Generic;
using ConfirmitDialerInterface;
using SimulatorDialerDriver.Models;

namespace SimulatorDialerDriver.Distribution
{
    public static class GeneratorsHost
    {
        public static List<IGenerator> All = new List<IGenerator>();
    }

    public class Generators
    {
        public static List<IGenerator> All => GeneratorsHost.All;

        public static DialerErrorCodeGenerator RegisterAgentSoftphoneResultCode = new DialerErrorCodeGenerator("Methods.RegisterAgentSoftphone.ResultCode");
        public static ObjectGenerator<RegisteredSoftphoneAgent> RegisterAgentSoftphoneOutcome
            = new ObjectGenerator<RegisteredSoftphoneAgent>("Methods.RegisterAgentSoftphone.Outcome");
        public static DialerErrorCodeGenerator LoginResultCode = new DialerErrorCodeGenerator("Methods.Login.ResultCode");
        public static DialerErrorCodeGenerator SetCampaignResultCode = new DialerErrorCodeGenerator("Methods.SetCampaign.ResultCode");
        public static DialerErrorCodeGenerator SendNumberToAgentResultCode = new DialerErrorCodeGenerator("Methods.SendNumberToAgent.ResultCode");
        public static DialerErrorCodeGenerator LogoutResultCode = new DialerErrorCodeGenerator("Methods.Logout.ResultCode");
        public static TimeSpanGenerator NotifyAgentStateDelay = new TimeSpanGenerator("Notifications.NotifyAgentState.Delay")
        {
            Options =
            {
                { "no wait(0)", (value) => TimeSpan.Zero }
            }
        };

        public static AgentStateGenerator NotifyAgentStateValue =
            new AgentStateGenerator("Notifications.NotifyAgentState.Value")
            {
                Options = {
                    {"LoggedIn", (value) => AgentState.LoggedIn},
                    {"LoggedOut", (value) => AgentState.LoggedOut},
                    {"NotReady", (value) => AgentState.NotReady},
                    {"Ready", (value) => AgentState.Ready},
                    {"OffHook", (value) => AgentState.OffHook},
                    {"OnHook", (value) => AgentState.OnHook},
                }
            };

        public static TimeSpanGenerator CallOutcomeDelay = new TimeSpanGenerator("Notifications.CallOutcome.Delay")
        {
            Options =
            {
                { "no wait(0)", (value) => TimeSpan.Zero }
            }
        };

        public static TimeSpanGenerator ExternalTransferOutcomeDelay = new TimeSpanGenerator("Notifications.ExternalTransferOutcome.Delay")
        {
            Options =
            {
                { "no wait(0)", (value) => TimeSpan.Zero }
            }
        };

        public static TargetOutcomeGenerator ExternalTransferOutcomeValue = new TargetOutcomeGenerator("Notifications.ExternalTransferOutcome.Value")
        {
            Options = {
                {"Connected", (value) => TargetOutcome.Connected},
                {"Busy", (value) => TargetOutcome.Busy},
                {"NoReply", (value) => TargetOutcome.NoReply}
            }
        };

        public static CallOutcomeGenerator CallOutcomeValue = new CallOutcomeGenerator("Notifications.CallOutcome.Value")
        {
            Options = {
                {"Connected", (value) => CallOutcome.Connected},
                {"Busy", (value) => CallOutcome.Busy},
                {"NoReply", (value) => CallOutcome.NoReply}
            }
        };
    }
}