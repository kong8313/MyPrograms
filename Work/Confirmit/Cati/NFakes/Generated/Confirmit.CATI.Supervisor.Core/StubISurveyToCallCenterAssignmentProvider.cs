using System;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;

namespace Confirmit.CATI.Supervisor.Core.CallCenters.Fakes
{
    public class StubISurveyToCallCenterAssignmentProvider : ISurveyToCallCenterAssignmentProvider 
    {
        private ISurveyToCallCenterAssignmentProvider _inner;

        public StubISurveyToCallCenterAssignmentProvider()
        {
            _inner = null;
        }

        public ISurveyToCallCenterAssignmentProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<BvSpGetSurveyCallCenterAssignmentPageEntity> GetPageStringPagingArgsInt32OutDelegate(string userName, PagingArgs pagingArgs, out int totalCount);
        public GetPageStringPagingArgsInt32OutDelegate GetPageStringPagingArgsInt32Out;

        IEnumerable<BvSpGetSurveyCallCenterAssignmentPageEntity> ISurveyToCallCenterAssignmentProvider.GetPage(string userName, PagingArgs pagingArgs, out int totalCount)
        {
            totalCount = default(int);


            if (GetPageStringPagingArgsInt32Out != null)
            {
                return GetPageStringPagingArgsInt32Out(userName, pagingArgs, out totalCount);
            } else if (_inner != null)
            {
                return ((ISurveyToCallCenterAssignmentProvider)_inner).GetPage(userName, pagingArgs, out totalCount);
            }

            return default(IEnumerable<BvSpGetSurveyCallCenterAssignmentPageEntity>);
        }

    }
}