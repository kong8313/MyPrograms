using System;

namespace Confirmit.CATI.Supervisor.Core.News
{
    public class NewsModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Ingress { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public int Active { get; set; }
    }
}