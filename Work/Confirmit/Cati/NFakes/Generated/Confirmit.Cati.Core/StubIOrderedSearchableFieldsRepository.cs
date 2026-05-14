using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIOrderedSearchableFieldsRepository : IOrderedSearchableFieldsRepository 
    {
        private IOrderedSearchableFieldsRepository _inner;

        public StubIOrderedSearchableFieldsRepository()
        {
            _inner = null;
        }

        public IOrderedSearchableFieldsRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvSearchableFieldsOrderedEntity> GetBySurveyIdInt32Delegate(int surveyId);
        public GetBySurveyIdInt32Delegate GetBySurveyIdInt32;

        List<BvSearchableFieldsOrderedEntity> IOrderedSearchableFieldsRepository.GetBySurveyId(int surveyId)
        {


            if (GetBySurveyIdInt32 != null)
            {
                return GetBySurveyIdInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IOrderedSearchableFieldsRepository)_inner).GetBySurveyId(surveyId);
            }

            return default(List<BvSearchableFieldsOrderedEntity>);
        }

        public delegate void UpdateListOfBvSearchableFieldsOrderedEntityDelegate(List<BvSearchableFieldsOrderedEntity> searchableFields);
        public UpdateListOfBvSearchableFieldsOrderedEntityDelegate UpdateListOfBvSearchableFieldsOrderedEntity;

        void IOrderedSearchableFieldsRepository.Update(List<BvSearchableFieldsOrderedEntity> searchableFields)
        {

            if (UpdateListOfBvSearchableFieldsOrderedEntity != null)
            {
                UpdateListOfBvSearchableFieldsOrderedEntity(searchableFields);
            } else if (_inner != null)
            {
                ((IOrderedSearchableFieldsRepository)_inner).Update(searchableFields);
            }
        }

    }
}