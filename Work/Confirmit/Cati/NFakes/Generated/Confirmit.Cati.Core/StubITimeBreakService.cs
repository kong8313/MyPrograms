using System;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubITimeBreakService : ITimeBreakService 
    {
        private ITimeBreakService _inner;

        public StubITimeBreakService()
        {
            _inner = null;
        }

        public ITimeBreakService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetBreakTypeNameNullableOfInt32Delegate(int? breakTypeId);
        public GetBreakTypeNameNullableOfInt32Delegate GetBreakTypeNameNullableOfInt32;

        string ITimeBreakService.GetBreakTypeName(int? breakTypeId)
        {


            if (GetBreakTypeNameNullableOfInt32 != null)
            {
                return GetBreakTypeNameNullableOfInt32(breakTypeId);
            } else if (_inner != null)
            {
                return ((ITimeBreakService)_inner).GetBreakTypeName(breakTypeId);
            }

            return default(string);
        }

    }
}