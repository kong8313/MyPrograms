using System;
using System.Web.SessionState;
using System.Web.UI;

using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes
{
    /// <summary>
    /// Stores ASP.NET page view state and control state on the Web server in session.
    /// Separate session keys used for different pages.
    /// Queue for old view state values is not supported.
    /// </summary>
    public class LightSessionPageStatePersister: PageStatePersister
    {
        private HttpSessionState session;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightSessionPageStatePersister"/> class.
        /// </summary>
        /// <param name="page">The <see cref="T:System.Web.UI.Page"/> that the view state persistence mechanism is created for.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="page"/> parameter is null.
        /// </exception>
        public LightSessionPageStatePersister(Page page) : base(page)
        {
            try
            {
                session = page.Session;
            }
            catch
            {
                throw new ApplicationException(Strings.ViewStateCanNotBeInitialized);
            }

            if (session == null)
            {
                throw new InvalidOperationException(Strings.SessionMustBeEnabled);
            }
        }

        /// <summary>
        /// Deserialize and load persisted state information when a <see cref="T:System.Web.UI.Page"/> object initializes its control hierarchy.
        /// </summary>
        public override void Load()
        {
            string key = Page.Request["_ViewStateKey"];
            if (!string.IsNullOrEmpty(key))
            {
                key = (string)StateFormatter.Deserialize(key);
                if (session[key] == null)
                {
                    throw new InvalidOperationException(Strings.ViewStateKeyIsNotFoundInSession);
                }

                Pair statePair = (Pair)session[key];
                ViewState = statePair.First;
                ControlState = statePair.Second;
            }
        }

        /// <summary>
        /// Serialize persisted state information when a <see cref="T:System.Web.UI.Page"/> object is unloaded from memory.
        /// </summary>
        public override void Save()
        {
            if (ViewState != null || ControlState != null)
            {
                Pair statePair = new Pair(ViewState, ControlState);
                string Key = "_VIEWSTATE_" + Page.GetType().FullName;
                session[Key] = statePair;
                if (ScriptManager.GetCurrent(Page) != null)
                {
                    ScriptManager.RegisterHiddenField(Page, "_ViewStateKey", StateFormatter.Serialize(Key));
                }
                else
                {
                    Page.ClientScript.RegisterHiddenField("_ViewStateKey", StateFormatter.Serialize(Key));
                }
            }
        }
    }
}