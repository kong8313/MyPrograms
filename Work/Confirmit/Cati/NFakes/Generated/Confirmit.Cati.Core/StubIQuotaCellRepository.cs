using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIQuotaCellRepository : IQuotaCellRepository 
    {
        private IQuotaCellRepository _inner;

        public StubIQuotaCellRepository()
        {
            _inner = null;
        }

        public IQuotaCellRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvSurveyQuotaCellEntity TryGetByIdInt32Int32Int32Delegate(int surveyId, int quotaId, int cellId);
        public TryGetByIdInt32Int32Int32Delegate TryGetByIdInt32Int32Int32;

        BvSurveyQuotaCellEntity IQuotaCellRepository.TryGetById(int surveyId, int quotaId, int cellId)
        {


            if (TryGetByIdInt32Int32Int32 != null)
            {
                return TryGetByIdInt32Int32Int32(surveyId, quotaId, cellId);
            } else if (_inner != null)
            {
                return ((IQuotaCellRepository)_inner).TryGetById(surveyId, quotaId, cellId);
            }

            return default(BvSurveyQuotaCellEntity);
        }

        public delegate void MergeBvSurveyQuotaCellEntityDelegate(BvSurveyQuotaCellEntity cell);
        public MergeBvSurveyQuotaCellEntityDelegate MergeBvSurveyQuotaCellEntity;

        void IQuotaCellRepository.Merge(BvSurveyQuotaCellEntity cell)
        {

            if (MergeBvSurveyQuotaCellEntity != null)
            {
                MergeBvSurveyQuotaCellEntity(cell);
            } else if (_inner != null)
            {
                ((IQuotaCellRepository)_inner).Merge(cell);
            }
        }

        public delegate void InsertListOfBvSurveyQuotaCellEntityDelegate(List<BvSurveyQuotaCellEntity> cells);
        public InsertListOfBvSurveyQuotaCellEntityDelegate InsertListOfBvSurveyQuotaCellEntity;

        void IQuotaCellRepository.Insert(List<BvSurveyQuotaCellEntity> cells)
        {

            if (InsertListOfBvSurveyQuotaCellEntity != null)
            {
                InsertListOfBvSurveyQuotaCellEntity(cells);
            } else if (_inner != null)
            {
                ((IQuotaCellRepository)_inner).Insert(cells);
            }
        }

        public delegate void DeleteInt32IEnumerableOfInt32Delegate(int surveyId, IEnumerable<int> quotaIds);
        public DeleteInt32IEnumerableOfInt32Delegate DeleteInt32IEnumerableOfInt32;

        void IQuotaCellRepository.Delete(int surveyId, IEnumerable<int> quotaIds)
        {

            if (DeleteInt32IEnumerableOfInt32 != null)
            {
                DeleteInt32IEnumerableOfInt32(surveyId, quotaIds);
            } else if (_inner != null)
            {
                ((IQuotaCellRepository)_inner).Delete(surveyId, quotaIds);
            }
        }

        public delegate void DeleteAllInt32Delegate(int surveyId);
        public DeleteAllInt32Delegate DeleteAllInt32;

        void IQuotaCellRepository.DeleteAll(int surveyId)
        {

            if (DeleteAllInt32 != null)
            {
                DeleteAllInt32(surveyId);
            } else if (_inner != null)
            {
                ((IQuotaCellRepository)_inner).DeleteAll(surveyId);
            }
        }

        public delegate void MergeAnyCellsInt32Int32ListOfBvSurveyQuotaCellEntityDelegate(int surveyId, int quotaId, List<BvSurveyQuotaCellEntity> cells);
        public MergeAnyCellsInt32Int32ListOfBvSurveyQuotaCellEntityDelegate MergeAnyCellsInt32Int32ListOfBvSurveyQuotaCellEntity;

        void IQuotaCellRepository.MergeAnyCells(int surveyId, int quotaId, List<BvSurveyQuotaCellEntity> cells)
        {

            if (MergeAnyCellsInt32Int32ListOfBvSurveyQuotaCellEntity != null)
            {
                MergeAnyCellsInt32Int32ListOfBvSurveyQuotaCellEntity(surveyId, quotaId, cells);
            } else if (_inner != null)
            {
                ((IQuotaCellRepository)_inner).MergeAnyCells(surveyId, quotaId, cells);
            }
        }

        public delegate List<BvSurveyQuotaCellEntity> GetBySurveyIdInt32Delegate(int surveyId);
        public GetBySurveyIdInt32Delegate GetBySurveyIdInt32;

        List<BvSurveyQuotaCellEntity> IQuotaCellRepository.GetBySurveyId(int surveyId)
        {


            if (GetBySurveyIdInt32 != null)
            {
                return GetBySurveyIdInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IQuotaCellRepository)_inner).GetBySurveyId(surveyId);
            }

            return default(List<BvSurveyQuotaCellEntity>);
        }

        public delegate List<BvSurveyQuotaCellEntity> GetCellsInt32Int32Delegate(int surveyId, int quotaId);
        public GetCellsInt32Int32Delegate GetCellsInt32Int32;

        List<BvSurveyQuotaCellEntity> IQuotaCellRepository.GetCells(int surveyId, int quotaId)
        {


            if (GetCellsInt32Int32 != null)
            {
                return GetCellsInt32Int32(surveyId, quotaId);
            } else if (_inner != null)
            {
                return ((IQuotaCellRepository)_inner).GetCells(surveyId, quotaId);
            }

            return default(List<BvSurveyQuotaCellEntity>);
        }

    }
}