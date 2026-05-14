using System;
using Confirmit.CATI.Core.Validators.Interfaces;
using Confirmit.CATI.Core.Validators;

namespace Confirmit.CATI.Core.Validators.Interfaces.Fakes
{
    public class StubIMultipleAssignmentValidator : IMultipleAssignmentValidator 
    {
        private IMultipleAssignmentValidator _inner;

        public StubIMultipleAssignmentValidator()
        {
            _inner = null;
        }

        public IMultipleAssignmentValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate MultipleAssignmentValidationResult ValidateMultipleAssignmentArrayOfStringDelegate(string[] keys);
        public ValidateMultipleAssignmentArrayOfStringDelegate ValidateMultipleAssignmentArrayOfString;

        MultipleAssignmentValidationResult IMultipleAssignmentValidator.ValidateMultipleAssignment(string[] keys)
        {


            if (ValidateMultipleAssignmentArrayOfString != null)
            {
                return ValidateMultipleAssignmentArrayOfString(keys);
            } else if (_inner != null)
            {
                return ((IMultipleAssignmentValidator)_inner).ValidateMultipleAssignment(keys);
            }

            return default(MultipleAssignmentValidationResult);
        }

        public delegate MultipleAssignmentValidationResult ValidateMultipleAssignmentByCountsInt32Int32Delegate(int groupsInAssignmentCount, int personsInAssignmentCount);
        public ValidateMultipleAssignmentByCountsInt32Int32Delegate ValidateMultipleAssignmentByCountsInt32Int32;

        MultipleAssignmentValidationResult IMultipleAssignmentValidator.ValidateMultipleAssignmentByCounts(int groupsInAssignmentCount, int personsInAssignmentCount)
        {


            if (ValidateMultipleAssignmentByCountsInt32Int32 != null)
            {
                return ValidateMultipleAssignmentByCountsInt32Int32(groupsInAssignmentCount, personsInAssignmentCount);
            } else if (_inner != null)
            {
                return ((IMultipleAssignmentValidator)_inner).ValidateMultipleAssignmentByCounts(groupsInAssignmentCount, personsInAssignmentCount);
            }

            return default(MultipleAssignmentValidationResult);
        }

        public delegate bool IsMultipleAssignmentGroupInt32Delegate(int sid);
        public IsMultipleAssignmentGroupInt32Delegate IsMultipleAssignmentGroupInt32;

        bool IMultipleAssignmentValidator.IsMultipleAssignmentGroup(int sid)
        {


            if (IsMultipleAssignmentGroupInt32 != null)
            {
                return IsMultipleAssignmentGroupInt32(sid);
            } else if (_inner != null)
            {
                return ((IMultipleAssignmentValidator)_inner).IsMultipleAssignmentGroup(sid);
            }

            return default(bool);
        }

    }
}