using System;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIPersonGroupService : IPersonGroupService 
    {
        private IPersonGroupService _inner;

        public StubIPersonGroupService()
        {
            _inner = null;
        }

        public IPersonGroupService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool IsExistsAndNotAdministrativeArrayOfInt32Delegate(int[] groupIds);
        public IsExistsAndNotAdministrativeArrayOfInt32Delegate IsExistsAndNotAdministrativeArrayOfInt32;

        bool IPersonGroupService.IsExistsAndNotAdministrative(int[] groupIds)
        {


            if (IsExistsAndNotAdministrativeArrayOfInt32 != null)
            {
                return IsExistsAndNotAdministrativeArrayOfInt32(groupIds);
            } else if (_inner != null)
            {
                return ((IPersonGroupService)_inner).IsExistsAndNotAdministrative(groupIds);
            }

            return default(bool);
        }

        public delegate bool IsGroupContainsInterviewerInt32StringDelegate(int interviewerId, string groupName);
        public IsGroupContainsInterviewerInt32StringDelegate IsGroupContainsInterviewerInt32String;

        bool IPersonGroupService.IsGroupContainsInterviewer(int interviewerId, string groupName)
        {


            if (IsGroupContainsInterviewerInt32String != null)
            {
                return IsGroupContainsInterviewerInt32String(interviewerId, groupName);
            } else if (_inner != null)
            {
                return ((IPersonGroupService)_inner).IsGroupContainsInterviewer(interviewerId, groupName);
            }

            return default(bool);
        }

    }
}