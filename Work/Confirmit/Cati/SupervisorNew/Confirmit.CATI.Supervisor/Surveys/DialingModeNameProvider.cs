using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Resources;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Surveys
{
    public class DialingModeNameProvider : IDiallingModeNameProvider
    {
        private static List<DialingModeEntity> _dialingModes = GetDialingModeNamesBasedOnLocale(); 

        public List<DialingModeEntity> GetAll()
        {
            return _dialingModes;
        }

        private static List<DialingModeEntity> GetDialingModeNamesBasedOnLocale()
        {
            //currently we just get English version

            var dialingModes = new List<DialingModeEntity>();

            dialingModes.Add(new DialingModeEntity {Title = Strings.ManualDialingMode, Id = DialingMode.Manual});
            dialingModes.Add(new DialingModeEntity {Title = Strings.PreviewDialingMode, Id = DialingMode.Preview});
            dialingModes.Add(new DialingModeEntity {Title = Strings.AutomaticDialingMode, Id = DialingMode.Automatic});
            dialingModes.Add(new DialingModeEntity {Title = Strings.PredictiveDialingMode, Id = DialingMode.Predictive});
            dialingModes.Add(new DialingModeEntity {Title = Strings.SpecialDialDialingMode, Id = DialingMode.SpecialDial});

            return dialingModes;
        }

    }
}