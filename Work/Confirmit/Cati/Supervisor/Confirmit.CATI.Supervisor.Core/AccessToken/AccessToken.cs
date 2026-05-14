using System;

namespace Confirmit.CATI.Supervisor.Core.AccessToken
{
    [Serializable]
    public class AccessToken
    {
        public AccessToken(string accessToken, int secondsDuration)
        {
            Value = accessToken;
            ExpirationUtcTime = DateTime.UtcNow.AddSeconds(secondsDuration / 2);
        }

        public string Value { get; set; }

        public DateTime ExpirationUtcTime { get; set; }
    }
}