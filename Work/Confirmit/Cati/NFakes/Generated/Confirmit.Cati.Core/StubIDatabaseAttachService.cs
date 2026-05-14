using System;
using Confirmit.CATI.Core.DAL.Framework;

namespace Confirmit.CATI.Core.DAL.Framework.Fakes
{
    public class StubIDatabaseAttachService : IDatabaseAttachService 
    {
        private IDatabaseAttachService _inner;

        public StubIDatabaseAttachService()
        {
            _inner = null;
        }

        public IDatabaseAttachService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool IsSurveyDatabaseAttachedStringDelegate(string projectId);
        public IsSurveyDatabaseAttachedStringDelegate IsSurveyDatabaseAttachedString;

        bool IDatabaseAttachService.IsSurveyDatabaseAttached(string projectId)
        {


            if (IsSurveyDatabaseAttachedString != null)
            {
                return IsSurveyDatabaseAttachedString(projectId);
            } else if (_inner != null)
            {
                return ((IDatabaseAttachService)_inner).IsSurveyDatabaseAttached(projectId);
            }

            return default(bool);
        }

        public delegate void AttachSurveyDatabaseStringDelegate(string projectId);
        public AttachSurveyDatabaseStringDelegate AttachSurveyDatabaseString;

        void IDatabaseAttachService.AttachSurveyDatabase(string projectId)
        {

            if (AttachSurveyDatabaseString != null)
            {
                AttachSurveyDatabaseString(projectId);
            } else if (_inner != null)
            {
                ((IDatabaseAttachService)_inner).AttachSurveyDatabase(projectId);
            }
        }

        public delegate void DetachSurveyDatabaseStringDelegate(string projectId);
        public DetachSurveyDatabaseStringDelegate DetachSurveyDatabaseString;

        void IDatabaseAttachService.DetachSurveyDatabase(string projectId)
        {

            if (DetachSurveyDatabaseString != null)
            {
                DetachSurveyDatabaseString(projectId);
            } else if (_inner != null)
            {
                ((IDatabaseAttachService)_inner).DetachSurveyDatabase(projectId);
            }
        }

    }
}