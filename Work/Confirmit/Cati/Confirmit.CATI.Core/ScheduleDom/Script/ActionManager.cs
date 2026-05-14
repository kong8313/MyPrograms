namespace Confirmit.CATI.Core.ScheduleDom.Script
{
    /// <summary>
    /// Represents utility methods for managing actions. 
    /// </summary>
    public static class ActionManager
    {
        private const int RecallOnNextShiftOfSpecifiedTypeId = 7;
        private const int SetNextRuleActionId = 25;
        private const int GoToRuleActionId = 23;
        private const int SetShiftType = 37;
        private const int RecallOnTheSpecificShift = 35;
        private const int SetCallExpirationTime = 34;
        private const int RecallOnSpecificTime = 33;

        /// <summary>
        /// Magic number that means [Any Valid] shift type
        /// </summary>
        public const int AnyValidShiftTypeId = 0;

        /// <summary>
        /// Magic number that means [None] shift type
        /// </summary>
        public const int NoneShiftTypeId = -1;

        /// <summary>
        /// Determines if given action identifier belongs to "Recall on next shift of specified type" action.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <returns>true, if action is "Recall on next shift of specified type"; otherwise false.</returns>
        public static bool IsRecallOnNextShiftOfSpecifiedType(int actionId)
        {
            return actionId == RecallOnNextShiftOfSpecifiedTypeId;
        }

        /// <summary>
        /// Determines if given action identifier belongs to "Set next rule" action.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <returns>true, if action is "Set next rule"; otherwise false.</returns>
        public static bool IsSetNextRuleAction(int actionId)
        {
            return actionId == SetNextRuleActionId;
        }

        /// <summary>
        /// Determines if given action identifier belongs to "Go to" action.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <returns>true, if action is "Go to"; otherwise false.</returns>
        public static bool IsGoToAction(int actionId)
        {
            return actionId == GoToRuleActionId;
        }

        /// <summary>
        /// Determines if given action identifier belongs to "Set shift type" action.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <returns>true, if action is "Set shift type"; otherwise false.</returns>
        public static bool IsSetShiftType(int actionId)
        {
            return actionId == SetShiftType;
        }

        /// <summary>
        /// Determines if given action identifier belongs to "Set next rule" action.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <returns>true, if action is "Set next rule"; otherwise false.</returns>
        public static bool IsRecallOnSpecificTime(int actionId)
        {
            return actionId == RecallOnSpecificTime;
        }

        /// <summary>
        /// Determines if given action identifier belongs to "Set call expiration time" action.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <returns>true, if action is "Set call expiration time"; otherwise false.</returns>
        public static bool IsSetCallExpirationTime(int actionId)
        {
            return actionId == SetCallExpirationTime;
        }

        /// <summary>
        /// Determines if given action identifier belongs to "Recall on the specific shift" action.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <returns>true, if action is "Recall on the specific shift"; otherwise false.</returns>
        public static bool IsRecallOnTheSpecificShift(int actionId)
        {
            return actionId == RecallOnTheSpecificShift;
        }
    }
}
