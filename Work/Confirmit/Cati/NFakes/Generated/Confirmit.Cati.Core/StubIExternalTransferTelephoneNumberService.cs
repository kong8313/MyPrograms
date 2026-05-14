using System;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIExternalTransferTelephoneNumberService : IExternalTransferTelephoneNumberService 
    {
        private IExternalTransferTelephoneNumberService _inner;

        public StubIExternalTransferTelephoneNumberService()
        {
            _inner = null;
        }

        public IExternalTransferTelephoneNumberService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InsertNumberStringStringBooleanArrayOfInt32Delegate(string telephoneNumber, string description, bool isHidden, int[] assignedSurveysIds);
        public InsertNumberStringStringBooleanArrayOfInt32Delegate InsertNumberStringStringBooleanArrayOfInt32;

        void IExternalTransferTelephoneNumberService.InsertNumber(string telephoneNumber, string description, bool isHidden, int[] assignedSurveysIds)
        {

            if (InsertNumberStringStringBooleanArrayOfInt32 != null)
            {
                InsertNumberStringStringBooleanArrayOfInt32(telephoneNumber, description, isHidden, assignedSurveysIds);
            } else if (_inner != null)
            {
                ((IExternalTransferTelephoneNumberService)_inner).InsertNumber(telephoneNumber, description, isHidden, assignedSurveysIds);
            }
        }

        public delegate void UpdateNumberInt32StringStringBooleanArrayOfInt32Delegate(int id, string telephoneNumber, string description, bool isHidden, int[] assignedSurveysIds);
        public UpdateNumberInt32StringStringBooleanArrayOfInt32Delegate UpdateNumberInt32StringStringBooleanArrayOfInt32;

        void IExternalTransferTelephoneNumberService.UpdateNumber(int id, string telephoneNumber, string description, bool isHidden, int[] assignedSurveysIds)
        {

            if (UpdateNumberInt32StringStringBooleanArrayOfInt32 != null)
            {
                UpdateNumberInt32StringStringBooleanArrayOfInt32(id, telephoneNumber, description, isHidden, assignedSurveysIds);
            } else if (_inner != null)
            {
                ((IExternalTransferTelephoneNumberService)_inner).UpdateNumber(id, telephoneNumber, description, isHidden, assignedSurveysIds);
            }
        }

        public delegate void DeleteNumbersArrayOfInt32Delegate(int[] ids);
        public DeleteNumbersArrayOfInt32Delegate DeleteNumbersArrayOfInt32;

        void IExternalTransferTelephoneNumberService.DeleteNumbers(int[] ids)
        {

            if (DeleteNumbersArrayOfInt32 != null)
            {
                DeleteNumbersArrayOfInt32(ids);
            } else if (_inner != null)
            {
                ((IExternalTransferTelephoneNumberService)_inner).DeleteNumbers(ids);
            }
        }

        public delegate int[] GetAssignedSurveyIdsInt32Delegate(int externalTransferTelephoneNumberId);
        public GetAssignedSurveyIdsInt32Delegate GetAssignedSurveyIdsInt32;

        int[] IExternalTransferTelephoneNumberService.GetAssignedSurveyIds(int externalTransferTelephoneNumberId)
        {


            if (GetAssignedSurveyIdsInt32 != null)
            {
                return GetAssignedSurveyIdsInt32(externalTransferTelephoneNumberId);
            } else if (_inner != null)
            {
                return ((IExternalTransferTelephoneNumberService)_inner).GetAssignedSurveyIds(externalTransferTelephoneNumberId);
            }

            return default(int[]);
        }

        public delegate void SetAssignedSurveyIdsInt32ArrayOfInt32Delegate(int externalTransferTelephoneNumberId, int[] surveyIds);
        public SetAssignedSurveyIdsInt32ArrayOfInt32Delegate SetAssignedSurveyIdsInt32ArrayOfInt32;

        void IExternalTransferTelephoneNumberService.SetAssignedSurveyIds(int externalTransferTelephoneNumberId, int[] surveyIds)
        {

            if (SetAssignedSurveyIdsInt32ArrayOfInt32 != null)
            {
                SetAssignedSurveyIdsInt32ArrayOfInt32(externalTransferTelephoneNumberId, surveyIds);
            } else if (_inner != null)
            {
                ((IExternalTransferTelephoneNumberService)_inner).SetAssignedSurveyIds(externalTransferTelephoneNumberId, surveyIds);
            }
        }

    }
}