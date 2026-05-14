using System;
using Confirmit.CATI.Core.Services.Survey.Quota;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Survey.Quota.Fakes
{
    public class StubIQuotaDatabaseReader : IQuotaDatabaseReader 
    {
        private IQuotaDatabaseReader _inner;

        public StubIQuotaDatabaseReader()
        {
            _inner = null;
        }

        public IQuotaDatabaseReader Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<string> GetAllFieldsInt32Delegate(int surveySid);
        public GetAllFieldsInt32Delegate GetAllFieldsInt32;

        IEnumerable<string> IQuotaDatabaseReader.GetAllFields(int surveySid)
        {


            if (GetAllFieldsInt32 != null)
            {
                return GetAllFieldsInt32(surveySid);
            } else if (_inner != null)
            {
                return ((IQuotaDatabaseReader)_inner).GetAllFields(surveySid);
            }

            return default(IEnumerable<string>);
        }

        public delegate IEnumerable<ClrQuotaInfo> GetQuotasInt32Delegate(int surveySid);
        public GetQuotasInt32Delegate GetQuotasInt32;

        IEnumerable<ClrQuotaInfo> IQuotaDatabaseReader.GetQuotas(int surveySid)
        {


            if (GetQuotasInt32 != null)
            {
                return GetQuotasInt32(surveySid);
            } else if (_inner != null)
            {
                return ((IQuotaDatabaseReader)_inner).GetQuotas(surveySid);
            }

            return default(IEnumerable<ClrQuotaInfo>);
        }

        public delegate IEnumerable<string> GetQuotaFieldsInt32Int32Delegate(int surveySid, int quotaId);
        public GetQuotaFieldsInt32Int32Delegate GetQuotaFieldsInt32Int32;

        IEnumerable<string> IQuotaDatabaseReader.GetQuotaFields(int surveySid, int quotaId)
        {


            if (GetQuotaFieldsInt32Int32 != null)
            {
                return GetQuotaFieldsInt32Int32(surveySid, quotaId);
            } else if (_inner != null)
            {
                return ((IQuotaDatabaseReader)_inner).GetQuotaFields(surveySid, quotaId);
            }

            return default(IEnumerable<string>);
        }

        public delegate IEnumerable<string> GetFieldPrecodesInt32Int32StringDelegate(int surveySid, int quotaId, string fieldName);
        public GetFieldPrecodesInt32Int32StringDelegate GetFieldPrecodesInt32Int32String;

        IEnumerable<string> IQuotaDatabaseReader.GetFieldPrecodes(int surveySid, int quotaId, string fieldName)
        {


            if (GetFieldPrecodesInt32Int32String != null)
            {
                return GetFieldPrecodesInt32Int32String(surveySid, quotaId, fieldName);
            } else if (_inner != null)
            {
                return ((IQuotaDatabaseReader)_inner).GetFieldPrecodes(surveySid, quotaId, fieldName);
            }

            return default(IEnumerable<string>);
        }

        public delegate Dictionary<string, HashSet<string>> GetFieldPrecodesInt32Int32Delegate(int surveySid, int quotaId);
        public GetFieldPrecodesInt32Int32Delegate GetFieldPrecodesInt32Int32;

        Dictionary<string, HashSet<string>> IQuotaDatabaseReader.GetFieldPrecodes(int surveySid, int quotaId)
        {


            if (GetFieldPrecodesInt32Int32 != null)
            {
                return GetFieldPrecodesInt32Int32(surveySid, quotaId);
            } else if (_inner != null)
            {
                return ((IQuotaDatabaseReader)_inner).GetFieldPrecodes(surveySid, quotaId);
            }

            return default(Dictionary<string, HashSet<string>>);
        }

        public delegate IEnumerable<QuotaCellInfo> GetQuotaCellsInt32Int32ArrayOfStringBooleanDelegate(int surveySid, int quotaId, string[] fields, bool isSupportOptimisticQuota);
        public GetQuotaCellsInt32Int32ArrayOfStringBooleanDelegate GetQuotaCellsInt32Int32ArrayOfStringBoolean;

        IEnumerable<QuotaCellInfo> IQuotaDatabaseReader.GetQuotaCells(int surveySid, int quotaId, string[] fields, bool isSupportOptimisticQuota)
        {


            if (GetQuotaCellsInt32Int32ArrayOfStringBoolean != null)
            {
                return GetQuotaCellsInt32Int32ArrayOfStringBoolean(surveySid, quotaId, fields, isSupportOptimisticQuota);
            } else if (_inner != null)
            {
                return ((IQuotaDatabaseReader)_inner).GetQuotaCells(surveySid, quotaId, fields, isSupportOptimisticQuota);
            }

            return default(IEnumerable<QuotaCellInfo>);
        }

    }
}