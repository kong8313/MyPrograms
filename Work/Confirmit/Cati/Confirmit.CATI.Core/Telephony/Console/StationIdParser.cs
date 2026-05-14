using System.Text.RegularExpressions;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Core.PersonLogin
{
    internal class StationIdParser : IStationIdParser
    {
        private const string StationIdentifierFormat = @"^[a-zA-Z]{1,8}(?<extNumber>\d{0,6})(?<hasLocal>L)?$";

        public StationInfo Parse(string stationId)
        {
            var result = new StationInfo
            {
                IsLocal = false,
                ExtensionNumber = string.Empty,
                DialerId = 0,
                StationId = stationId
            };

            if (string.IsNullOrEmpty(stationId))
            {
                return result;
            }

            var match = Regex.Match(stationId, StationIdentifierFormat);

            if (!match.Success)
            {
                throw new UserMessageException(
                    string.Format("Station identifier '{0}' has incorrect format.", stationId),
                    "Error_StationIdentifierHasIncorrectFormat");
            }

            var strDialerIdAndExtensionNumber = match.Groups["extNumber"].Value;
            int dialerIdAndExtensionNumber;

            if (int.TryParse(strDialerIdAndExtensionNumber, out dialerIdAndExtensionNumber))
            {
                result.DialerId = (dialerIdAndExtensionNumber / 100000) + 1;
                result.ExtensionNumber = (dialerIdAndExtensionNumber % 100000).ToString();
            }

            result.IsLocal = !string.IsNullOrEmpty(match.Groups["hasLocal"].Value);

            return result;
        }
    }
}
