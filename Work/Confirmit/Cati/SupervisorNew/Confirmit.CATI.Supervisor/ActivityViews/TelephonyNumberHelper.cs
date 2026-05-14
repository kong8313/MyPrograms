using System.Web;

namespace Confirmit.CATI.Supervisor.ActivityViews
{
    /// <summary>
    /// Represents results of TelephonyNumber dialog
    /// </summary>
    public enum DialogResult
    {
        /// <summary>
        /// User selects start video and audio
        /// </summary>
        StartAudioVideo,
        /// <summary>
        /// User selects start only video
        /// </summary>
        StartOnlyVideo
    }

    /// <summary>
    /// Helper class for TelephonyNumber dialog
    /// Used for storing and retriving telephone number and dialog result from Session
    /// </summary>
    public static class TelephonyNumberHelper
    {
        private static string m_Number = "Number";
        private static string m_DialogResult = "DialogResult";

        /// <summary>
        /// Gets stored telephony number from session
        /// </summary>
        public static string GetTelephonyNumber(string sessionKey)
        {
            return (string)(HttpContext.Current.Session[sessionKey + m_Number] ?? string.Empty);
        }

        /// <summary>
        /// Sets telephony number in session
        /// </summary>
        public static void SetTelephonyNumber(string sessionKey, string number)
        {
            HttpContext.Current.Session[sessionKey + m_Number] = number;
        }

        /// <summary>
        /// Gets dialog result from session
        /// </summary>
        public static DialogResult GetDialogResult(string sessionKey)
        {
            return (DialogResult)(HttpContext.Current.Session[sessionKey + m_DialogResult] ?? DialogResult.StartOnlyVideo);
        }

        /// <summary>
        /// Sets dialog result in session
        /// </summary>
        public static void SetDialogResult(string sessionKey, DialogResult result)
        {
            HttpContext.Current.Session[sessionKey + m_DialogResult] = result;
        }

        public static void ResetDialogResult(string sessionKey)
        {
            HttpContext.Current.Session[sessionKey + m_DialogResult] = DialogResult.StartOnlyVideo;
        }
    }
}
