using System.Web.UI;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class BaseControl : Control
    {
        public new BaseForm Page
        {
            get
            {
                return (BaseForm)base.Page;
            }
        }

        /// <summary>
        /// Produces a path, relative to base application dir
        /// </summary>
        protected string BaseRelativePath(string path)
        {
            return Page.Request.ApplicationPath + "/" + path;
        }

        protected string GetResString(string key)
        {
            return (Core.Common.ResourceWrapper.Instance.GetString(key));
        }
    }
}