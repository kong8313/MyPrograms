using System;
using Confirmit.CATI.Supervisor.Core.News;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Confirmit.CATI.Supervisor.Core.News.Fakes
{
    public class StubINewsApiService : INewsApiService 
    {
        private INewsApiService _inner;

        public StubINewsApiService()
        {
            _inner = null;
        }

        public INewsApiService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate NewsModel[] GetNewsBooleanDelegate(bool unreadOnly);
        public GetNewsBooleanDelegate GetNewsBoolean;

        NewsModel[] INewsApiService.GetNews(bool unreadOnly)
        {


            if (GetNewsBoolean != null)
            {
                return GetNewsBoolean(unreadOnly);
            } else if (_inner != null)
            {
                return ((INewsApiService)_inner).GetNews(unreadOnly);
            }

            return default(NewsModel[]);
        }

        public delegate Task MarkReadAsyncIEnumerableOfInt32Delegate(IEnumerable<int> newsId);
        public MarkReadAsyncIEnumerableOfInt32Delegate MarkReadAsyncIEnumerableOfInt32;

        Task INewsApiService.MarkReadAsync(IEnumerable<int> newsId)
        {


            if (MarkReadAsyncIEnumerableOfInt32 != null)
            {
                return MarkReadAsyncIEnumerableOfInt32(newsId);
            } else if (_inner != null)
            {
                return ((INewsApiService)_inner).MarkReadAsync(newsId);
            }

            return default(Task);
        }

    }
}