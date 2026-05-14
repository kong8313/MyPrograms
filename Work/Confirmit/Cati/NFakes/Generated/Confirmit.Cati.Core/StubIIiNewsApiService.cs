using System;
using Confirmit.CATI.Core.Services.News;

namespace Confirmit.CATI.Core.Services.News.Fakes
{
    public class StubIIiNewsApiService : IINewsApiService 
    {
        private IINewsApiService _inner;

        public StubIIiNewsApiService()
        {
            _inner = null;
        }

        public IINewsApiService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate NewsModel[] GetNewsBooleanDelegate(bool unreadOnly);
        public GetNewsBooleanDelegate GetNewsBoolean;

        NewsModel[] IINewsApiService.GetNews(bool unreadOnly)
        {


            if (GetNewsBoolean != null)
            {
                return GetNewsBoolean(unreadOnly);
            } else if (_inner != null)
            {
                return ((IINewsApiService)_inner).GetNews(unreadOnly);
            }

            return default(NewsModel[]);
        }

        public delegate void MarkReadInt32Delegate(int newId);
        public MarkReadInt32Delegate MarkReadInt32;

        void IINewsApiService.MarkRead(int newId)
        {

            if (MarkReadInt32 != null)
            {
                MarkReadInt32(newId);
            } else if (_inner != null)
            {
                ((IINewsApiService)_inner).MarkRead(newId);
            }
        }

    }
}