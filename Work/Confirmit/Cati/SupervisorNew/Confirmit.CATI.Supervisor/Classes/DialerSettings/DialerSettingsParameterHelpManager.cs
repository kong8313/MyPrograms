using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Classes.DialerSettings
{
    public static class DialerSettingsParameterHelpManager
    {
        internal static bool IsSharedParameter(string parameterId)
        {
            return (parameterId == "Email") || (parameterId == "RespondentVariableToSendToTheDialer");
        }

        internal static string GetHelpStringKey(DiallerType diallerType, string parameterId)
        {
            string key = "help";

            if (parameterId == "Email")
            {
                //common parameter
                key += "_Email";
            }
            else if (parameterId == "RespondentVariableToSendToTheDialer")
            {
                key += "_RespondentVariableToSendToTheDialer";
            }
            else
            {
                switch (diallerType)
                {
                    case DiallerType.PROTS:

                        key += "_PROTS";

                        switch (parameterId)
                        {
                            case "AbandonmentRate":
                                key += '_' + "AbandonmentRate";
                                break;
                            case "AnsMachineDetect":
                                key += '_' + "AnsMachineDetect";
                                break;
                            case "AnsMachineAudioMessageUrl":
                                key += '_' + "AnsMachineAudioMessageUrl";
                                break;
                            case "BillingCode":
                                key += '_' + "BillingCode";
                                break;
                            case "MaxRings":
                                key += '_' + "MaxRings";
                                break;
                        }
                        break;

                    case DiallerType.BvTCI:

                        key += "_BvTCI";

                        switch (parameterId)
                        {
                            case "TelephoneNumberPrefix":
                                key += '_' + "TelephoneNumberPrefix";
                                break;
                            case "MaxRings":
                                key += '_' + "MaxRings";
                                break;
                        }

                        break;

                   case DiallerType.Generic:

                        key += "_Generic_" + parameterId;

                        break;                        
                }
            }

            return key;
        }
    }
}
