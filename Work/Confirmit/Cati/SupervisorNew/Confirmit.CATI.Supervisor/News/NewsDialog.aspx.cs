using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.News;

namespace Confirmit.CATI.Supervisor.News
{
    public partial class NewsDialog : BaseForm
    {
        private readonly INewsApiService _newsApiService;
        protected List<NewsModel> News = new List<NewsModel>();

        public NewsDialog()
        {
            _newsApiService = ServiceLocator.Resolve<INewsApiService>();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            RegisterStartupScript("Common._disableAutoFocus = true;");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                News = _newsApiService.GetNews().OrderByDescending(x => x.Date).ToList();
            }
        }

        protected void ConfirmClick(object sender, EventArgs e)
        {
            if (cbMarkAllAsRead.Checked && !string.IsNullOrEmpty(selectedNews.Value))
            {
	            _newsApiService.MarkReadAsync(selectedNews.Value.Split(';').Select(x => Convert.ToInt32(x))).Wait();
            }

            CloseOverlay(true);
        }
    }
}