using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common.Types;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubITelephoneBlacklistRepository : ITelephoneBlacklistRepository 
    {
        private ITelephoneBlacklistRepository _inner;

        public StubITelephoneBlacklistRepository()
        {
            _inner = null;
        }

        public ITelephoneBlacklistRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvTelephoneBlacklistEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<BvTelephoneBlacklistEntity> ITelephoneBlacklistRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((ITelephoneBlacklistRepository)_inner).GetAll();
            }

            return default(List<BvTelephoneBlacklistEntity>);
        }

        public delegate List<BvTelephoneBlacklistEntity> GetPagePagingArgsInt32OutDelegate(PagingArgs pageArguments, out int totalCount);
        public GetPagePagingArgsInt32OutDelegate GetPagePagingArgsInt32Out;

        List<BvTelephoneBlacklistEntity> ITelephoneBlacklistRepository.GetPage(PagingArgs pageArguments, out int totalCount)
        {
            totalCount = default(int);


            if (GetPagePagingArgsInt32Out != null)
            {
                return GetPagePagingArgsInt32Out(pageArguments, out totalCount);
            } else if (_inner != null)
            {
                return ((ITelephoneBlacklistRepository)_inner).GetPage(pageArguments, out totalCount);
            }

            return default(List<BvTelephoneBlacklistEntity>);
        }

        public delegate BvTelephoneBlacklistEntity GetByDisplayPatternStringDelegate(string displayPattern);
        public GetByDisplayPatternStringDelegate GetByDisplayPatternString;

        BvTelephoneBlacklistEntity ITelephoneBlacklistRepository.GetByDisplayPattern(string displayPattern)
        {


            if (GetByDisplayPatternString != null)
            {
                return GetByDisplayPatternString(displayPattern);
            } else if (_inner != null)
            {
                return ((ITelephoneBlacklistRepository)_inner).GetByDisplayPattern(displayPattern);
            }

            return default(BvTelephoneBlacklistEntity);
        }

        public delegate BvTelephoneBlacklistEntity GetByIdInt32Delegate(int id);
        public GetByIdInt32Delegate GetByIdInt32;

        BvTelephoneBlacklistEntity ITelephoneBlacklistRepository.GetById(int id)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(id);
            } else if (_inner != null)
            {
                return ((ITelephoneBlacklistRepository)_inner).GetById(id);
            }

            return default(BvTelephoneBlacklistEntity);
        }

        public delegate BvTelephoneBlacklistEntity GetByNumberStringDelegate(string telephoneNumber);
        public GetByNumberStringDelegate GetByNumberString;

        BvTelephoneBlacklistEntity ITelephoneBlacklistRepository.GetByNumber(string telephoneNumber)
        {


            if (GetByNumberString != null)
            {
                return GetByNumberString(telephoneNumber);
            } else if (_inner != null)
            {
                return ((ITelephoneBlacklistRepository)_inner).GetByNumber(telephoneNumber);
            }

            return default(BvTelephoneBlacklistEntity);
        }

        public delegate int InsertBvTelephoneBlacklistEntityDelegate(BvTelephoneBlacklistEntity entity);
        public InsertBvTelephoneBlacklistEntityDelegate InsertBvTelephoneBlacklistEntity;

        int ITelephoneBlacklistRepository.Insert(BvTelephoneBlacklistEntity entity)
        {


            if (InsertBvTelephoneBlacklistEntity != null)
            {
                return InsertBvTelephoneBlacklistEntity(entity);
            } else if (_inner != null)
            {
                return ((ITelephoneBlacklistRepository)_inner).Insert(entity);
            }

            return default(int);
        }

        public delegate void UpdateBvTelephoneBlacklistEntityDelegate(BvTelephoneBlacklistEntity entity);
        public UpdateBvTelephoneBlacklistEntityDelegate UpdateBvTelephoneBlacklistEntity;

        void ITelephoneBlacklistRepository.Update(BvTelephoneBlacklistEntity entity)
        {

            if (UpdateBvTelephoneBlacklistEntity != null)
            {
                UpdateBvTelephoneBlacklistEntity(entity);
            } else if (_inner != null)
            {
                ((ITelephoneBlacklistRepository)_inner).Update(entity);
            }
        }

        public delegate int DeleteAllDelegate();
        public DeleteAllDelegate DeleteAll;

        int ITelephoneBlacklistRepository.DeleteAll()
        {


            if (DeleteAll != null)
            {
                return DeleteAll();
            } else if (_inner != null)
            {
                return ((ITelephoneBlacklistRepository)_inner).DeleteAll();
            }

            return default(int);
        }

        public delegate void DeleteIEnumerableOfInt32Delegate(IEnumerable<int> ids);
        public DeleteIEnumerableOfInt32Delegate DeleteIEnumerableOfInt32;

        void ITelephoneBlacklistRepository.Delete(IEnumerable<int> ids)
        {

            if (DeleteIEnumerableOfInt32 != null)
            {
                DeleteIEnumerableOfInt32(ids);
            } else if (_inner != null)
            {
                ((ITelephoneBlacklistRepository)_inner).Delete(ids);
            }
        }

        public delegate Range<int> ImportListOfBvTelephoneBlacklistEntityDelegate(List<BvTelephoneBlacklistEntity> entities);
        public ImportListOfBvTelephoneBlacklistEntityDelegate ImportListOfBvTelephoneBlacklistEntity;

        Range<int> ITelephoneBlacklistRepository.Import(List<BvTelephoneBlacklistEntity> entities)
        {


            if (ImportListOfBvTelephoneBlacklistEntity != null)
            {
                return ImportListOfBvTelephoneBlacklistEntity(entities);
            } else if (_inner != null)
            {
                return ((ITelephoneBlacklistRepository)_inner).Import(entities);
            }

            return default(Range<int>);
        }

    }
}