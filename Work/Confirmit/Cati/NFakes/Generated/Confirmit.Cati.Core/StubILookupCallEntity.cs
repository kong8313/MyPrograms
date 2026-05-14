using System;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure.Fakes
{
    public class StubILookupCallEntity : ILookupCallEntity 
    {
        private ILookupCallEntity _inner;

        public StubILookupCallEntity()
        {
            _inner = null;
        }

        public ILookupCallEntity Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int? _CallId;
        public Func<int?> CallIdGet;
        public Action<int?> CallIdSetNullableOfInt32;

        int? ILookupCallEntity.CallId
        {
            get
            {
                if (CallIdGet != null)
                {
                    return CallIdGet();
                } else if (_inner != null)
                {
                    return ((ILookupCallEntity)_inner).CallId;
                }

                if (CallIdSetNullableOfInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallId;
                }

                return default(int?);
            }

            set
            {
                if (CallIdSetNullableOfInt32 != null)
                {
                    CallIdSetNullableOfInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ILookupCallEntity)_inner).CallId = value;
                    return;
                }

                if (CallIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CallId = value;
                }

            }
        }

        private int? _SurveyId;
        public Func<int?> SurveyIdGet;
        public Action<int?> SurveyIdSetNullableOfInt32;

        int? ILookupCallEntity.SurveyId
        {
            get
            {
                if (SurveyIdGet != null)
                {
                    return SurveyIdGet();
                } else if (_inner != null)
                {
                    return ((ILookupCallEntity)_inner).SurveyId;
                }

                if (SurveyIdSetNullableOfInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyId;
                }

                return default(int?);
            }

            set
            {
                if (SurveyIdSetNullableOfInt32 != null)
                {
                    SurveyIdSetNullableOfInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ILookupCallEntity)_inner).SurveyId = value;
                    return;
                }

                if (SurveyIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SurveyId = value;
                }

            }
        }

        private int? _InterviewId;
        public Func<int?> InterviewIdGet;
        public Action<int?> InterviewIdSetNullableOfInt32;

        int? ILookupCallEntity.InterviewId
        {
            get
            {
                if (InterviewIdGet != null)
                {
                    return InterviewIdGet();
                } else if (_inner != null)
                {
                    return ((ILookupCallEntity)_inner).InterviewId;
                }

                if (InterviewIdSetNullableOfInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewId;
                }

                return default(int?);
            }

            set
            {
                if (InterviewIdSetNullableOfInt32 != null)
                {
                    InterviewIdSetNullableOfInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ILookupCallEntity)_inner).InterviewId = value;
                    return;
                }

                if (InterviewIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewId = value;
                }

            }
        }

        private int? _ActiveDialId;
        public Func<int?> ActiveDialIdGet;
        public Action<int?> ActiveDialIdSetNullableOfInt32;

        int? ILookupCallEntity.ActiveDialId
        {
            get
            {
                if (ActiveDialIdGet != null)
                {
                    return ActiveDialIdGet();
                } else if (_inner != null)
                {
                    return ((ILookupCallEntity)_inner).ActiveDialId;
                }

                if (ActiveDialIdSetNullableOfInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ActiveDialId;
                }

                return default(int?);
            }

            set
            {
                if (ActiveDialIdSetNullableOfInt32 != null)
                {
                    ActiveDialIdSetNullableOfInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ILookupCallEntity)_inner).ActiveDialId = value;
                    return;
                }

                if (ActiveDialIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ActiveDialId = value;
                }

            }
        }

    }
}