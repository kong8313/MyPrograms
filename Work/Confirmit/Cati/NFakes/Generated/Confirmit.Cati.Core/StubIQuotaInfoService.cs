using System;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIQuotaInfoService : IQuotaInfoService 
    {
        private IQuotaInfoService _inner;

        public StubIQuotaInfoService()
        {
            _inner = null;
        }

        public IQuotaInfoService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool HasQuotasInt32Delegate(int surveyId);
        public HasQuotasInt32Delegate HasQuotasInt32;

        bool IQuotaInfoService.HasQuotas(int surveyId)
        {


            if (HasQuotasInt32 != null)
            {
                return HasQuotasInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IQuotaInfoService)_inner).HasQuotas(surveyId);
            }

            return default(bool);
        }

        public delegate string[] GetQuotaFieldsInt32Int32Delegate(int surveyId, int quotaId);
        public GetQuotaFieldsInt32Int32Delegate GetQuotaFieldsInt32Int32;

        string[] IQuotaInfoService.GetQuotaFields(int surveyId, int quotaId)
        {


            if (GetQuotaFieldsInt32Int32 != null)
            {
                return GetQuotaFieldsInt32Int32(surveyId, quotaId);
            } else if (_inner != null)
            {
                return ((IQuotaInfoService)_inner).GetQuotaFields(surveyId, quotaId);
            }

            return default(string[]);
        }

        public delegate QuotaInfo[] GetQuotaInfosInt32Delegate(int surveyId);
        public GetQuotaInfosInt32Delegate GetQuotaInfosInt32;

        QuotaInfo[] IQuotaInfoService.GetQuotaInfos(int surveyId)
        {


            if (GetQuotaInfosInt32 != null)
            {
                return GetQuotaInfosInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IQuotaInfoService)_inner).GetQuotaInfos(surveyId);
            }

            return default(QuotaInfo[]);
        }

        public delegate string[] GetQuotaFieldsInt32StringDelegate(int surveyId, string quotaName);
        public GetQuotaFieldsInt32StringDelegate GetQuotaFieldsInt32String;

        string[] IQuotaInfoService.GetQuotaFields(int surveyId, string quotaName)
        {


            if (GetQuotaFieldsInt32String != null)
            {
                return GetQuotaFieldsInt32String(surveyId, quotaName);
            } else if (_inner != null)
            {
                return ((IQuotaInfoService)_inner).GetQuotaFields(surveyId, quotaName);
            }

            return default(string[]);
        }

        public delegate string GetQuotaNameInt32Int32Delegate(int surveyId, int quotaId);
        public GetQuotaNameInt32Int32Delegate GetQuotaNameInt32Int32;

        string IQuotaInfoService.GetQuotaName(int surveyId, int quotaId)
        {


            if (GetQuotaNameInt32Int32 != null)
            {
                return GetQuotaNameInt32Int32(surveyId, quotaId);
            } else if (_inner != null)
            {
                return ((IQuotaInfoService)_inner).GetQuotaName(surveyId, quotaId);
            }

            return default(string);
        }

        public delegate string GetQuotaTableBvSurveyEntityInt32Delegate(BvSurveyEntity survey, int quotaId);
        public GetQuotaTableBvSurveyEntityInt32Delegate GetQuotaTableBvSurveyEntityInt32;

        string IQuotaInfoService.GetQuotaTable(BvSurveyEntity survey, int quotaId)
        {


            if (GetQuotaTableBvSurveyEntityInt32 != null)
            {
                return GetQuotaTableBvSurveyEntityInt32(survey, quotaId);
            } else if (_inner != null)
            {
                return ((IQuotaInfoService)_inner).GetQuotaTable(survey, quotaId);
            }

            return default(string);
        }

        public delegate string GetQuotaTableBvSurveyEntityStringDelegate(BvSurveyEntity survey, string name);
        public GetQuotaTableBvSurveyEntityStringDelegate GetQuotaTableBvSurveyEntityString;

        string IQuotaInfoService.GetQuotaTable(BvSurveyEntity survey, string name)
        {


            if (GetQuotaTableBvSurveyEntityString != null)
            {
                return GetQuotaTableBvSurveyEntityString(survey, name);
            } else if (_inner != null)
            {
                return ((IQuotaInfoService)_inner).GetQuotaTable(survey, name);
            }

            return default(string);
        }

        public delegate string[] GetCellValuesInt32Int32Int32ArrayOfStringDelegate(int surveyId, int quotaId, int cellId, string[] fields);
        public GetCellValuesInt32Int32Int32ArrayOfStringDelegate GetCellValuesInt32Int32Int32ArrayOfString;

        string[] IQuotaInfoService.GetCellValues(int surveyId, int quotaId, int cellId, string[] fields)
        {


            if (GetCellValuesInt32Int32Int32ArrayOfString != null)
            {
                return GetCellValuesInt32Int32Int32ArrayOfString(surveyId, quotaId, cellId, fields);
            } else if (_inner != null)
            {
                return ((IQuotaInfoService)_inner).GetCellValues(surveyId, quotaId, cellId, fields);
            }

            return default(string[]);
        }

        public delegate bool IsExistsBvSurveyEntityStringDelegate(BvSurveyEntity survey, string quotaName);
        public IsExistsBvSurveyEntityStringDelegate IsExistsBvSurveyEntityString;

        bool IQuotaInfoService.IsExists(BvSurveyEntity survey, string quotaName)
        {


            if (IsExistsBvSurveyEntityString != null)
            {
                return IsExistsBvSurveyEntityString(survey, quotaName);
            } else if (_inner != null)
            {
                return ((IQuotaInfoService)_inner).IsExists(survey, quotaName);
            }

            return default(bool);
        }

        public delegate Dictionary<string, string> GellQuotaCellValuesMapStringStringDelegate(string projectId, string quotaName);
        public GellQuotaCellValuesMapStringStringDelegate GellQuotaCellValuesMapStringString;

        Dictionary<string, string> IQuotaInfoService.GellQuotaCellValuesMap(string projectId, string quotaName)
        {


            if (GellQuotaCellValuesMapStringString != null)
            {
                return GellQuotaCellValuesMapStringString(projectId, quotaName);
            } else if (_inner != null)
            {
                return ((IQuotaInfoService)_inner).GellQuotaCellValuesMap(projectId, quotaName);
            }

            return default(Dictionary<string, string>);
        }

    }
}