using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace BvDotNetScript.ScriptObjects.Cache
{
    public abstract class FormDescBase
    {
        public bool CODED = false;
        public bool DICHOTOMY = false;
        public bool COMPOUND = false;
        public bool OPEN = false;
        public bool DATE = false;
        public bool BOOL = false;
        public bool EXTERNAL = false;
        public bool NUMERIC = false;

        public string Label = null;
        public string Text = null;
        public string Instruction = null;
        public VariableDataType VariableType = VariableDataType.Normal;
        public bool IsReplicated = false;


        public string ProjectId { get; protected set; }
        public int SurveyId { get; protected set; }


        public double MaxValue = Double.MaxValue;
        public bool MaxValueAllow = true;
        public double MinValue = Double.MinValue;
        public bool MinValueAllow = true;
        public int FieldWidth = -1;
        public SurveyDatabaseFormInfo DbFormInfo;

        public string FormName { get; protected set; }
        public string[] LoopPath { get; set; }
        public string FormLevel { get; set; }

        public List<string> Categories { get; private set; }

        public ValidationData ValidationData { get; protected set; }

        internal FormDescBase()
        {
        }

        internal FormDescBase(int surveyId, string projectId, FormBase form, SurveyDatabaseFormInfo dbFormInfo)
        {
            SurveyId = surveyId;
            FormName = form.Name;
            FormLevel = dbFormInfo.LoopPath[dbFormInfo.LoopPath.Length - 1];
            ProjectId = projectId;
            VariableType = form.VariableType;
            DbFormInfo = dbFormInfo;
            FieldWidth = form.FieldWidth;

            LoopPath = dbFormInfo.LoopPath.ToArray();

            if (form.FormTexts.Length > 0)
            {
                Label = form.FormTexts[0].Title;
                Text = form.FormTexts[0].Text;
                Instruction = form.FormTexts[0].Instruction;
            }

            Categories = new List<string>();
        }

        public SurveyDatabaseFieldInfo GetFormFieldByCategory(string category)
        {
            if (category == null)
            {
                return DbFormInfo.Fields.Single();
            }
            else
            {
                var fieldName = string.Format("{0}_{1}", FormName, category);
                return DbFormInfo.Fields.Single(x => x.FieldName == fieldName);
            }
        }

        internal static FormDescBase CreateInstance(int surveyId, string projectId, FormBase form, SurveyDatabaseFormInfo dbFormInfo)
        {
            if (form is SingleForm)
                return new SingleFormDesc(surveyId, projectId, form as SingleForm, dbFormInfo);
            else if(form is OpenForm)
                return new OpenFormDesc(surveyId, projectId, form as OpenForm, dbFormInfo);
            else if (form is MultiForm)
                return new MultiFormDesc(surveyId, projectId, form as MultiForm, dbFormInfo);
            else
                throw new Exception(String.Format("Question in survey '{0}' has not supported type '{1}'.",
                     projectId, form.GetType()));
        }

        public override string ToString()
        {
            return String.Format("Form '{0}' {1}", FormName,
                LoopPath.Length > 0 ? " in " + String.Join("\\", LoopPath) : "");
        }
    }
}
