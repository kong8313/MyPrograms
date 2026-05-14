using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.SurveyDataService;
using BvDotNetScript.SurveyDataApiWS.Util;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.WcfServices.Clients;

namespace Confirmit.CATI.Core.Services.Survey.Data
{
    public class SurveyDataRowsWebServiceUpdater : ISurveyDataRowsWebServiceUpdater
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISurveyDataService _surveyDataService;
        private readonly ISurveyMetadataCacheService _surveyMetadataCacheService;

        private static readonly SystemVariables SystemVariablesDef = SurveyDataUtil.NewSystemVariables(
           false, false, false, false, true, false, false);

        public SurveyDataRowsWebServiceUpdater(
            ISurveyRepository surveyRepository,
            ISurveyDataService surveyDataService,
            ISurveyMetadataCacheService surveyMetadataCacheService)
        {
            _surveyRepository = surveyRepository;
            _surveyDataService = surveyDataService;
            _surveyMetadataCacheService = surveyMetadataCacheService;
        }

        public void Update(int surveyId, int interviewId, SurveyDataRowCache[] rows)
        {
            try
            {
                UpdateInternal(surveyId, interviewId, rows);
            }
            catch (TimeoutException timeoutException)
            {
                Trace.TraceError($"SurveyDataRowsWebServiceUpdater.Update.TimeoutException: {timeoutException}.");
            }
            catch (CommunicationException communicationException)
            {
                Trace.TraceWarning($"SurveyDataRowsWebServiceUpdater.Update.CommunicationException: {communicationException}. Retrying update.");
                UpdateInternal(surveyId, interviewId, rows);
            }
        }

        private void UpdateInternal(int surveyId, int interviewId, SurveyDataRowCache[] rows)
        {
            if (rows.Length == 0)
            {
                return;
            }

            var survey = _surveyRepository.GetById(surveyId);

            var surveyMetadataCache = _surveyMetadataCacheService.Get(surveyId);

            var levels2forms = rows.SelectMany(x => x.ChangedForms).Distinct().Select(surveyMetadataCache.GetFormDesc)
                .GroupBy(y => y.FormLevel).ToArray();

            var transferDef = CreateTransferDef(survey.ProjectId, interviewId, levels2forms);

            var result = _surveyDataService.GetData(transferDef, null);

            EventDetailsScope.Current.AddTiming("SurveyDataRowsWebServiceUpdater.Update.GetData");

            int responseId = 0;

            DataTable rootTable = result.Result.Tables["responseid"];
            if (rootTable.Rows.Count >= 1)
            {
                responseId = (int)rootTable.Rows[0]["responseId"];
            }
            else
            {
                var row = rootTable.NewRow();
                row["responseId"] = responseId;
                row["respid"] = interviewId;
                rootTable.Rows.Add(row);
            }

            foreach (var cacheRow in rows.Where(x => x.IsChanged))
            {
                DataTable dt = result.Result.Tables[cacheRow.LoopLevel];

                var row = GetOrCreateRowByQualifier(dt, responseId, interviewId, cacheRow.LoopPath, cacheRow.LoopQualifyer);

                foreach (var fieldName in cacheRow.ChangedColumns)
                {
                    var column = dt.Columns.Cast<DataColumn>().Single(x => x.ColumnName == fieldName);

                    var value = cacheRow.GetFieldValue(fieldName);

                    if (value != null)
                        row[column] = Convert.ChangeType(value, column.DataType);
                    else
                        row[column] = DBNull.Value;
                }
            }

            _surveyDataService.UpdateData(transferDef, result.Result, false, false, -1);

            EventDetailsScope.Current.AddTiming("SurveyDataRowsWebServiceUpdater.Update.UpdateData");
        }

        private DataRow GetOrCreateRowByQualifier(DataTable dt, int responseId, int interviewId, string[] loopPath, string[] loopQualifyer)
        {
            var keys = loopPath.Select((x, i) => new { KeyName = x, KeyValue = loopQualifyer[i] }).ToArray();

            foreach (DataRow row in dt.Rows)
            {
                if (keys.All(x => row[x.KeyName].ToString() == x.KeyValue))
                    return row;
            }

            var newRow = dt.NewRow();

            newRow["responseid"] = responseId;
            if (dt.Columns.Contains("respid"))
            {
                newRow["respid"] = interviewId;
            }

            foreach (var key in keys)
            {
                newRow[key.KeyName] = key.KeyValue;
            }

            dt.Rows.Add(newRow);

            return newRow;
        }

        private TransferDef CreateTransferDef(string projectId, int interviewId, IGrouping<string, FormDescBase>[] levels2forms)
        {
            TransferDef transferDef = SurveyDataUtil.NewTransferDef(projectId, false, DatabaseType.Production);
            transferDef.SystemVariables = SystemVariablesDef;

            TransferLevel rootTransferLevel = SurveyDataUtil.NewTransferLevel("responseid", true);

            rootTransferLevel.Where = SurveyDataUtil.NewWhereClause(
                    SurveyDataUtil.NewBinaryComparison(
                        ComparisonType.Equal,
                        SurveyDataUtil.NewQueryField("respid"),
                        SurveyDataUtil.NewQueryConstant(ConfirmitDbType.Integer, interviewId)));

            /*
             * We should request all root level( See fololowing codeSurveyDataUtil.NewTransferLevel("responseid", ***true***) ), 
             *  otherwise we expect problem with save multi answers in loops. In this case root response and response control 
             *  reconrds are not created and responseId is set to 0 for loop record. So that is reason why code is commented.
               
            var rootForms = levels2forms.FirstOrDefault(x => x.Key == "responseid");
            if(rootForms != null)
            {
                foreach (var rootForm in rootForms)
                {
                    var transferForm = SurveyDataUtil.NewTransferForm(rootForm.FormName, true);
                    rootTransferLevel.Forms = (TransferForm[])SurveyDataUtil.Add(
                        rootTransferLevel.Forms, transferForm, typeof(TransferForm));
                }
            }*/

            transferDef.Levels = (TransferLevel[])SurveyDataUtil.Add(
                transferDef.Levels, rootTransferLevel, typeof(TransferLevel));

            foreach (var level in levels2forms.Where(x => x.Key != "responseid"))
            {
                TransferLevel levelTransferLevel = SurveyDataUtil.NewTransferLevel(level.Key, false);

                foreach (var levelForm in level)
                {
                    var transferForm = SurveyDataUtil.NewTransferForm(levelForm.FormName, true);
                    levelTransferLevel.Forms = (TransferForm[])SurveyDataUtil.Add(
                        levelTransferLevel.Forms, transferForm, typeof(TransferForm));
                }

                levelTransferLevel.Where = SurveyDataUtil.NewWhereClause(
                    SurveyDataUtil.NewBinaryComparison(
                        ComparisonType.Equal,
                        SurveyDataUtil.NewQueryField("respid"),
                        SurveyDataUtil.NewQueryConstant(ConfirmitDbType.Integer, interviewId)));

                transferDef.Levels = (TransferLevel[])SurveyDataUtil.Add(
                    transferDef.Levels, levelTransferLevel, typeof(TransferLevel));
            }

            return transferDef;
        }
    }
}
