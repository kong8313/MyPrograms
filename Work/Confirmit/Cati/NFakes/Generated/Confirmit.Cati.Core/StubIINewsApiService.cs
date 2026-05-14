using System;
using Confirmit.CATI.Core.Services.News;

namespace Confirmit.CATI.Core.Services.News.Fakes
{
    public class StubIINewsApiService : INewsApiService 
    {
        private INewsApiService _inner;

        public StubIINewsApiService()
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

        public delegate void MarkReadInt32Delegate(int newId);
        public MarkReadInt32Delegate MarkReadInt32;

        void INewsApiService.MarkRead(int newId)
        {

            if (MarkReadInt32 != null)
            {
                MarkReadInt32(newId);
            } else if (_inner != null)
            {
                ((INewsApiService)_inner).MarkRead(newId);
            }
        }

    }
}