using System;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Data;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubISurveyCallDistributionService : ISurveyCallDistributionService 
    {
        private ISurveyCallDistributionService _inner;

        public StubISurveyCallDistributionService()
        {
            _inner = null;
        }

        public ISurveyCallDistributionService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DataTable GetCallsSentToDialerDistributionInt32NullableOfDateTimeInt32Int32OutDelegate(int surveySid, DateTime? dateTime, int timezoneId, out int totalCount);
        public GetCallsSentToDialerDistributionInt32NullableOfDateTimeInt32Int32OutDelegate GetCallsSentToDialerDistributionInt32NullableOfDateTimeInt32Int32Out;

        DataTable ISurveyCallDistributionService.GetCallsSentToDialerDistribution(int surveySid, DateTime? dateTime, int timezoneId, out int totalCount)
        {
            totalCount = default(int);


            if (GetCallsSentToDialerDistributionInt32NullableOfDateTimeInt32Int32Out != null)
            {
                return GetCallsSentToDialerDistributionInt32NullableOfDateTimeInt32Int32Out(surveySid, dateTime, timezoneId, out totalCount);
            } else if (_inner != null)
            {
                return ((ISurveyCallDistributionService)_inner).GetCallsSentToDialerDistribution(surveySid, dateTime, timezoneId, out totalCount);
            }

            return default(DataTable);
        }

        public delegate DataTable GetCallsDispositionCodesInt32DateTimeDateTimeInt32OutDelegate(int surveySid, DateTime startTime, DateTime endTime, out int totalCount);
        public GetCallsDispositionCodesInt32DateTimeDateTimeInt32OutDelegate GetCallsDispositionCodesInt32DateTimeDateTimeInt32Out;

        DataTable ISurveyCallDistributionService.GetCallsDispositionCodes(int surveySid, DateTime startTime, DateTime endTime, out int totalCount)
        {
            totalCount = default(int);


            if (GetCallsDispositionCodesInt32DateTimeDateTimeInt32Out != null)
            {
                return GetCallsDispositionCodesInt32DateTimeDateTimeInt32Out(surveySid, startTime, endTime, out totalCount);
            } else if (_inner != null)
            {
                return ((ISurveyCallDistributionService)_inner).GetCallsDispositionCodes(surveySid, startTime, endTime, out totalCount);
            }

            return default(DataTable);
        }

        public delegate DataTable GetDialerCallsBreakdownInt32Int32OutDelegate(int surveySid, out int totalCount);
        public GetDialerCallsBreakdownInt32Int32OutDelegate GetDialerCallsBreakdownInt32Int32Out;

        DataTable ISurveyCallDistributionService.GetDialerCallsBreakdown(int surveySid, out int totalCount)
        {
            totalCount = default(int);


            if (GetDialerCallsBreakdownInt32Int32Out != null)
            {
                return GetDialerCallsBreakdownInt32Int32Out(surveySid, out totalCount);
            } else if (_inner != null)
            {
                return ((ISurveyCallDistributionService)_inner).GetDialerCallsBreakdown(surveySid, out totalCount);
            }

            return default(DataTable);
        }

        public delegate void CleanupCallsDistributionTimeSpanDelegate(TimeSpan expirationPeriod);
        public CleanupCallsDistributionTimeSpanDelegate CleanupCallsDistributionTimeSpan;

        void ISurveyCallDistributionService.CleanupCallsDistribution(TimeSpan expirationPeriod)
        {

            if (CleanupCallsDistributionTimeSpan != null)
            {
                CleanupCallsDistributionTimeSpan(expirationPeriod);
            } else if (_inner != null)
            {
                ((ISurveyCallDistributionService)_inner).CleanupCallsDistribution(expirationPeriod);
            }
        }

    }
}