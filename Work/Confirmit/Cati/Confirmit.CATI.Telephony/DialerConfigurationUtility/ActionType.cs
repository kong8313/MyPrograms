using System;

namespace DialerConfigurationUtility
{
    public class ActionType
    {
        /// <summary>
        /// Add dialer to the company database
        /// </summary>
        public const int AddDialer = 0;

        /// <summary>
        /// Update dialer parameters in the company database
        /// </summary>
        public const int UpdateDialer = 1;

        /// <summary>
        /// Remove dialer from the company database
        /// </summary>
        public const int RemoveDialer = 2;

        public static int ParseAction(string strAction)
        {
            strAction = strAction.ToLower();
            if (strAction.Equals("/add"))
            {
                return AddDialer;
            }

            if (strAction.Equals("/update"))
            {
                return UpdateDialer;
            }

            if (strAction.Equals("/remove"))
            {
                return RemoveDialer;
            }

            throw new ArgumentException("Incorrect action");
        }
    }
}