using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using BvDotNetScript.ScriptObjects.Cache;
using BvDotNetScript.SurveyDataApiWS.Util;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.SurveyDataService;
using Confirmit.CATI.Core.WcfServices.Clients;

namespace Confirmit.CATI.Core.Services.Survey.Data
{
    public class InterviewFormDataWebSourceService : IInterviewFormDataWebSourceService
    {
        private static readonly SystemVariables SystemVariablesDef = SurveyDataUtil.NewSystemVariables(
            false, false, false, false, true, false, false);

        private static readonly HashSet<string> SystemVariables = new HashSet<string> { "respid" };
        private readonly ISurveyDataService _surveyDataService;

        public int SurveyId { get; private set; }
        public int InterviewId { get; private set; }

        public InterviewFormDataWebSourceService(ISurveyDataService surveyDataService)
        {
            _surveyDataService = surveyDataService;
        }

        public void Initialize(int surveyId, int interviewId)
        {
            SurveyId = surveyId;
            InterviewId = interviewId;
        }

        public void Commit()
        {
        }

        public string GetFormValue(FormDescBase formDesc, string category, string[] loopQualifyer)
        {
            try
            {
                return GetFormValueInternal(formDesc, category, loopQualifyer);
            }
            catch (CommunicationException communicationException)
            {
                Trace.TraceWarning($"InterviewFormDataWebSourceService.GetFormValue.CommunicationException: {communicationException}. Retrying update.");
                return GetFormValueInternal(formDesc, category, loopQualifyer);
            }
        }

        private string GetFormValueInternal(FormDescBase formDesc, string category, string[] loopQualifyer)
        {
            var transferDef = CreateTransferDef(formDesc, InterviewId, loopQualifyer);

            var result = _surveyDataService.GetData(transferDef, null);

            EventDetailsScope.Current.AddTiming("InterviewFormDataWebSourceService.GetFormValue.GetData");

            DataTable dt = result.Result.Tables[formDesc.FormLevel];

            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else if (dt.Rows.Count != 1)
            {
                throw new Exception(
                    String.Format("get method: The Count of rows({0}) does not equal 1", dt.Rows.Count));
            }

            var field = formDesc.GetFormFieldByCategory(category);
            var val = dt.Rows[0][field.FieldName];

            return val == DBNull.Value ? null : val.ToString();
        }

        public void SetFormValue(FormDescBase formDesc, string category, string[] loopQualifier, string value)
        {
            try
            {
                SetFormValueInternal(formDesc, category, loopQualifier, value);
            }
            catch (CommunicationException communicationException)
            {
                Trace.TraceWarning($"InterviewFormDataWebSourceService.SetFormValue.CommunicationException: {communicationException}. Retrying update.");
                SetFormValueInternal(formDesc, category, loopQualifier, value);
            }
        }

        private void SetFormValueInternal(FormDescBase formDesc, string category, string[] loopQualifier, string value)
        {
            var transferDef = CreateTransferDef(formDesc, InterviewId, loopQualifier);

            var result = _surveyDataService.GetData(transferDef, null);

            EventDetailsScope.Current.AddTiming("InterviewFormDataWebSourceService.SetFormValue.GetData");

            int responseId = 0;
            if (formDesc.FormLevel != "responseid")
            {
                DataTable rootTable = result.Result.Tables["responseid"];
                if (rootTable.Rows.Count >= 1)
                    responseId = (int)rootTable.Rows[0]["responseId"];
                else
                {
                    var row = rootTable.NewRow();
                    row["responseId"] = responseId;
                    row["respid"] = InterviewId;
                    rootTable.Rows.Add(row);
                }
            }

            DataTable dt = result.Result.Tables[formDesc.FormLevel];
            if (dt.Rows.Count == 0)
            {
                DataRow newRow = dt.NewRow();

                newRow["responseid"] = responseId;
                if (dt.Columns.Contains("respid"))
                {
                    newRow["respid"] = InterviewId;
                }

                for (int i = 1; i < formDesc.LoopPath.Length; i++)
                {
                    newRow[formDesc.LoopPath[i]] = loopQualifier[i - 1];
                }

                dt.Rows.Add(newRow);
            }
            if (dt.Rows.Count != 1)
            {
                throw new Exception(
                    $"set method: The Count of rows({dt.Rows.Count}) does not equal 1");
            }

            var field = formDesc.GetFormFieldByCategory(category);
            DataColumn formColumn = dt.Columns.Cast<DataColumn>().SingleOrDefault(x => string.Equals(x.ColumnName, field.FieldName, StringComparison.InvariantCultureIgnoreCase));

            if (formColumn == null)
                throw new Exception("set method: The form column not found");

            if (value != null)
                dt.Rows[0][formColumn] = Convert.ChangeType(value, formColumn.DataType);
            else
                dt.Rows[0][formColumn] = DBNull.Value;

            _surveyDataService.UpdateData(transferDef, result.Result, false, false, -1);

            EventDetailsScope.Current.AddTiming("InterviewFormDataWebSourceService.SetFormValue.UpdateData");
        }

        private TransferDef CreateTransferDef(FormDescBase formDesc, int respId, params string[] loopQualifier)
        {
            if (formDesc.LoopPath.Count() > 1)
            {
                return CreateLoopTransferDef(formDesc, respId, loopQualifier);
            }
            else
            {
                return CreateRootTransferDef(formDesc, respId);
            }
        }

        private static TransferDef CreateLoopTransferDef(FormDescBase formDesc, int respid, params string[] loopQualifier)
        {
            TransferDef transferDef = SurveyDataUtil.NewTransferDef(formDesc.ProjectId, false, DatabaseType.Production);

            transferDef.SystemVariables = SurveyDataUtil.NewSystemVariables(
                false, false, false, false, false, false, false);

            TransferLevel transferLevel = SurveyDataUtil.NewTransferLevel(formDesc.FormLevel, true);

            TransferLevel transferRootLevel = SurveyDataUtil.NewTransferLevel("responseid", true);
            transferRootLevel.IsTopLevel = true;
            transferRootLevel.Where = SurveyDataUtil.NewWhereClause(SurveyDataUtil.NewBinaryComparison(
                ComparisonType.Equal,
                SurveyDataUtil.NewQueryField("respid"),
                SurveyDataUtil.NewQueryConstant(ConfirmitDbType.Integer, respid)));


            transferRootLevel.Forms = (TransferForm[])SurveyDataUtil.Add(
                    transferLevel.Forms, new TransferForm() { Name = "respid", AllChildrenFields = true, IsSystemVariable = true }, typeof(TransferForm));

            transferDef.Levels = (TransferLevel[])SurveyDataUtil.Add(
                    transferDef.Levels, transferRootLevel, typeof(TransferLevel));

            transferDef.Levels = (TransferLevel[])SurveyDataUtil.Add(
                    transferDef.Levels, transferLevel, typeof(TransferLevel));

            var binaryComparisons = new List<BinaryComparison>
            {
                SurveyDataUtil.NewBinaryComparison(
                    ComparisonType.Equal,
                    SurveyDataUtil.NewQueryField("respid"),
                    SurveyDataUtil.NewQueryConstant(ConfirmitDbType.Integer, respid))
            };

            for (int i = 1; i < formDesc.LoopPath.Length; i++)
            {
                binaryComparisons.Add(
                    SurveyDataUtil.NewBinaryComparison(
                        ComparisonType.Equal,
                        SurveyDataUtil.NewQueryField(formDesc.LoopPath[i]),
                        SurveyDataUtil.NewQueryConstant(ConfirmitDbType.VarChar, loopQualifier[i - 1])));
            }

            if (binaryComparisons.Count == 1)
            {
                transferLevel.Where = SurveyDataUtil.NewWhereClause(binaryComparisons[0]);
            }
            else
            {
                var binaryLogics = SurveyDataUtil.NewBinaryLogic(LogicType.AND, binaryComparisons[0],
                    binaryComparisons[1]);

                for (int i = 2; i < binaryComparisons.Count; i++)
                {
                    binaryLogics = SurveyDataUtil.NewBinaryLogic(LogicType.AND, binaryLogics, binaryComparisons[i]);
                }
                transferLevel.Where = SurveyDataUtil.NewWhereClause(binaryLogics);
            }

            return transferDef;
        }


        private static TransferDef CreateRootTransferDef(FormDescBase formDesc, int respId)
        {
            TransferDef transferDef = SurveyDataUtil.NewTransferDef(formDesc.ProjectId, false, DatabaseType.Production);
            TransferLevel transferLevel = SurveyDataUtil.NewTransferLevel(formDesc.FormLevel, false);
            TransferForm transferForm = SurveyDataUtil.NewTransferForm(formDesc.FormName, true);

            transferDef.SystemVariables = SurveyDataUtil.NewSystemVariables(
                false, false, false, false, true, false, false);

            transferLevel.Forms = (TransferForm[])SurveyDataUtil.Add(
                    transferLevel.Forms, transferForm, typeof(TransferForm));

            transferDef.Levels = (TransferLevel[])SurveyDataUtil.Add(
                    transferDef.Levels, transferLevel, typeof(TransferLevel));

            transferLevel.Where = SurveyDataUtil.NewWhereClause(SurveyDataUtil.NewBinaryComparison(
                            ComparisonType.Equal,
                            SurveyDataUtil.NewQueryField("respid"),
                            SurveyDataUtil.NewQueryConstant(ConfirmitDbType.Integer, respId)));

            return transferDef;
        }
    }
}
