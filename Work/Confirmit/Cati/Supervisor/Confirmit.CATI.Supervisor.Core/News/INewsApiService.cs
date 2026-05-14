using System.Collections.Generic;
using System.Threading.Tasks;

namespace Confirmit.CATI.Supervisor.Core.News
{
    public interface INewsApiService
    {
        NewsModel[] GetNews(bool unreadOnly = true);
        Task MarkReadAsync(IEnumerable<int> newsId);
    }
}