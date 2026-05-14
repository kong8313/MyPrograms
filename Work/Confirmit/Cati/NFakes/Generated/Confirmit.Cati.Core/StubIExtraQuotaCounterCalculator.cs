using System;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Handmade.Entity;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation.Fakes
{
    public class StubIExtraQuotaCounterCalculator : IExtraQuotaCounterCalculator 
    {
        private IExtraQuotaCounterCalculator _inner;

        public StubIExtraQuotaCounterCalculator()
        {
            _inner = null;
        }

        public IExtraQuotaCounterCalculator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<QuotaCellCounter> GetCellCounterDelegate();
        public GetCellCounterDelegate GetCellCounter;

        IEnumerable<QuotaCellCounter> IExtraQuotaCounterCalculator.GetCellCounter()
        {


            if (GetCellCounter != null)
            {
                return GetCellCounter();
            } else if (_inner != null)
            {
                return ((IExtraQuotaCounterCalculator)_inner).GetCellCounter();
            }

            return default(IEnumerable<QuotaCellCounter>);
        }

        public delegate IEnumerable<KeyValuePair<int, int>> GetItsCountersForCellInt32Delegate(int cellId);
        public GetItsCountersForCellInt32Delegate GetItsCountersForCellInt32;

        IEnumerable<KeyValuePair<int, int>> IExtraQuotaCounterCalculator.GetItsCountersForCell(int cellId)
        {


            if (GetItsCountersForCellInt32 != null)
            {
                return GetItsCountersForCellInt32(cellId);
            } else if (_inner != null)
            {
                return ((IExtraQuotaCounterCalculator)_inner).GetItsCountersForCell(cellId);
            }

            return default(IEnumerable<KeyValuePair<int, int>>);
        }

        public delegate int GetTotalCounterDelegate();
        public GetTotalCounterDelegate GetTotalCounter;

        int IExtraQuotaCounterCalculator.GetTotalCounter()
        {


            if (GetTotalCounter != null)
            {
                return GetTotalCounter();
            } else if (_inner != null)
            {
                return ((IExtraQuotaCounterCalculator)_inner).GetTotalCounter();
            }

            return default(int);
        }

        public delegate string GetFormatedTotalCounterDelegate();
        public GetFormatedTotalCounterDelegate GetFormatedTotalCounter;

        string IExtraQuotaCounterCalculator.GetFormatedTotalCounter()
        {


            if (GetFormatedTotalCounter != null)
            {
                return GetFormatedTotalCounter();
            } else if (_inner != null)
            {
                return ((IExtraQuotaCounterCalculator)_inner).GetFormatedTotalCounter();
            }

            return default(string);
        }

    }
}