using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Services.CleaningService;

namespace Confirmit.CATI.Core.Services.CleaningService.Fakes
{
    public class StubICleaningServiceEmailGenerator : ICleaningServiceEmailGenerator 
    {
        private ICleaningServiceEmailGenerator _inner;

        public StubICleaningServiceEmailGenerator()
        {
            _inner = null;
        }

        public ICleaningServiceEmailGenerator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetWarningBodyListOfCleaningServiceEmailInfoDelegate(List<CleaningServiceEmailInfo> surveys);
        public GetWarningBodyListOfCleaningServiceEmailInfoDelegate GetWarningBodyListOfCleaningServiceEmailInfo;

        string ICleaningServiceEmailGenerator.GetWarningBody(List<CleaningServiceEmailInfo> surveys)
        {


            if (GetWarningBodyListOfCleaningServiceEmailInfo != null)
            {
                return GetWarningBodyListOfCleaningServiceEmailInfo(surveys);
            } else if (_inner != null)
            {
                return ((ICleaningServiceEmailGenerator)_inner).GetWarningBody(surveys);
            }

            return default(string);
        }

        public delegate string GetCleanupBodyListOfCleaningServiceEmailInfoDelegate(List<CleaningServiceEmailInfo> surveys);
        public GetCleanupBodyListOfCleaningServiceEmailInfoDelegate GetCleanupBodyListOfCleaningServiceEmailInfo;

        string ICleaningServiceEmailGenerator.GetCleanupBody(List<CleaningServiceEmailInfo> surveys)
        {


            if (GetCleanupBodyListOfCleaningServiceEmailInfo != null)
            {
                return GetCleanupBodyListOfCleaningServiceEmailInfo(surveys);
            } else if (_inner != null)
            {
                return ((ICleaningServiceEmailGenerator)_inner).GetCleanupBody(surveys);
            }

            return default(string);
        }

    }
}