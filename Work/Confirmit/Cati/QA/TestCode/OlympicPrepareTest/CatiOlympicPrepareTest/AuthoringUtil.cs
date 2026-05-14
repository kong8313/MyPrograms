using System;
using System.Collections;
using CatiOlympicPrepareTest.wsAuthoring;

namespace CatiOlympicPrepareTest
{
    public class AuthoringUtil
    {
        internal static Hashtable Array2HashTable(object[] array)
        {
            var hashtable = new Hashtable();
            for (int i = 0; i < array.Length; i++)
            {
                object obj = array.GetValue(i);
                int tmpLanguage;
                if (obj.GetType() == typeof(AnswerText))
                    tmpLanguage = ((AnswerText)obj).Language;
                else if (obj.GetType() == typeof(GridAnswerText))
                    tmpLanguage = ((GridAnswerText)obj).Language;
                else if (obj.GetType() == typeof(LanguageString))
                    tmpLanguage = ((LanguageString)obj).Language;
                else
                    throw new Exception("Unknown datatype");
                hashtable.Add(tmpLanguage, obj);
            }

            return hashtable;
        }

        internal static object[] HashTable2Array(Hashtable hashtable, Type datatype)
        {
            Array tmpArray = Array.CreateInstance(datatype, hashtable.Count);
            hashtable.Values.CopyTo(tmpArray, 0);
            return (object[])tmpArray;
        }

        internal static ArrayList Array2ArrayList(object[] array)
        {
            return new ArrayList(array);
        }

        internal static object[] ArrayList2Array(ArrayList arrayList, Type datatype)
        {
            return (object[])arrayList.ToArray(datatype);
        }

        public static LanguageString NewLanguageString(string text, int language)
        {
            var ls = new LanguageString
            {
                Language = language,
                Value = text
            };
            return ls;
        }

        public static FormReference NewFormReference(string name)
        {
            var fr = new FormReference
            {
                Name = name
            };
            return fr;
        }

        public static ProjectInfo NewProjectInfo(string name, string description)
        {
            var pi = new ProjectInfo
            {
                Name = name,
                Description = description
            };
            return pi;
        }

        public static Stop NewStop(StopDataType t)
        {
            var s = new Stop
            {
                StopType = t
            };
            return s;
        }

        public static InfoForm NewInfoForm(string name)
        {
            var i = new InfoForm
            {
                Name = name
            };
            return i;
        }

        public static Questionnaire NewQuestionnaire()
        {
            return new Questionnaire();
        }

        public static SingleForm NewSingleForm(string name)
        {
            var singleForm = new SingleForm
            {
                Name = name,
                FieldWidth = IntegerNull(),
                ListRows = IntegerNull(),
                ListColumns = IntegerNull(),
                Level = IntegerNull(),
                AnswerlistOrder = TraverseOrderType.InOrder,
                VariableType = VariableDataType.Normal,
                AnswerListType = AnswerListType.Normal
            };
            return singleForm;
        }

        public static MultiForm NewMultiForm(string name)
        {
            var multiForm = new MultiForm
            {
                Name = name,
                FieldWidth = IntegerNull(),
                Columns = IntegerNull(),
                Precision = 2,
                Scale = 0,
                LowerLimit = DoubleNull(),
                UpperLimit = DoubleNull(),
                ListRows = IntegerNull(),
                ListColumns = IntegerNull(),
                Level = IntegerNull(),
                AnswerlistOrder = TraverseOrderType.InOrder,
                LowerLimitType = LowerLimitDataType.Disabled,
                UpperLimitType = UpperLimitDataType.Disabled,
                VariableType = VariableDataType.Normal
            };
            return multiForm;
        }

        public static GridForm NewGridForm(string name)
        {
            var gridForm = new GridForm
            {
                Name = name,
                FieldWidth = IntegerNull(),
                AnswerlistOrder = TraverseOrderType.InOrder,
                ScaleOrder = TraverseOrderType.InOrder,
                ListColumns = IntegerNull(),
                ListRows = IntegerNull()
            };
            return gridForm;
        }

        public static Grid3DForm NewGrid3DForm(string name)
        {
            var grid3DForm = new Grid3DForm
            {
                Name = name,
                AnswerlistOrder = TraverseOrderType.InOrder,
                ScaleOrder = TraverseOrderType.InOrder
            };
            return grid3DForm;
        }

        public static OpenForm NewOpenForm(string name)
        {
            var openForm = new OpenForm
            {
                Name = name,
                FieldWidth = IntegerNull(),
                Columns = IntegerNull(),
                Precision = 2,
                Scale = 0,
                LowerLimit = DoubleNull(),
                UpperLimit = DoubleNull(),
                Rows = IntegerNull(),
                Level = IntegerNull(),
                LowerLimitType = LowerLimitDataType.Disabled,
                UpperLimitType = UpperLimitDataType.Disabled,
                VariableType = VariableDataType.Normal
            };
            return openForm;
        }

        public static Loop NewLoop(string name)
        {
            var loop = new Loop
            {
                Name = name,
                FieldWidth = IntegerNull(),
                AnswerlistOrder = TraverseOrderType.InOrder
            };
            return loop;
        }

        public static Condition NewCondition(bool elseEnabled, string expression)
        {
            var condition = new Condition
            {
                ElseEnabled = elseEnabled,
                Expression = expression
            };
            return condition;
        }

        public static Folder NewFolder(string name)
        {
            var folder = new Folder
            {
                Name = name
            };
            return folder;
        }

        public static PredefinedList NewPredefinedList(string name)
        {
            var predefinedList = new PredefinedList
            {
                Name = name
            };
            return predefinedList;
        }

        public static Quota NewQuota(string name, string email)
        {
            var quota = new Quota
            {
                Name = name,
                EmailAddress = email
            };
            return quota;
        }

        public static Directive NewDirective(DirectiveDataType t)
        {
            var directive = new Directive
            {
                DirectiveType = t
            };
            return directive;
        }

        public static Script NewScript(string name, string code)
        {
            var script = new Script
            {
                Name = name,
                ScriptCode = code
            };
            return script;
        }

        public static Predefined NewPredefined(string name)
        {
            var predefined = new Predefined
            {
                Name = name
            };
            return predefined;
        }

        internal static ReadFilterSimple NewReadFilterSimple(
            bool includeAllLanguages,
            bool expandAnswers,
            bool projectSpecific)
        {
            // From ReadFilterSimple
            var readFilterSimple = new ReadFilterSimple
            {
                IncludeAllLanguages = includeAllLanguages,
                ExpandAnswers = expandAnswers,
                ProjectSpecific = projectSpecific
            };
            return readFilterSimple;
        }

        internal static AnswerText NewAnswerText(string text, int language)
        {
            var answerText = new AnswerText
            {
                Language = language,
                Value = text
            };
            return answerText;
        }

        internal static GridAnswerText NewGridAnswerText(string text, int language)
        {
            var gridAnswerText = new GridAnswerText
            {
                Language = language,
                Value = text
            };
            return gridAnswerText;
        }

        internal static Answer NewAnswer(string precode)
        {
            var answer = new Answer
            {
                Precode = precode,
                ColumnWidth = IntegerNull(),
                Weight = IntegerNull(),
                Punch = PunchType.Undefined
            };
            return answer;
        }

        internal static GridAnswer NewGridAnswer(string precode)
        {
            var gridAnswer = new GridAnswer
            {
                Precode = precode
            };
            return gridAnswer;
        }

        internal static LoopMember NewLoopMember(string precode)
        {
            var loopMember = new LoopMember
            {
                Precode = precode,
                Active = true
            };
            return loopMember;
        }

        internal static FormText NewFormText(string text, string title, string instruction, int language)
        {
            var formText = new FormText
            {
                Text = text,
                Instruction = instruction,
                Title = title,
                Language = language
            };
            return formText;
        }

        internal static ReadFilter NewReadFilter(
            bool includeAllLanguages,
            bool expandAnswers,
            bool projectSpecific,
            bool includeAllForms)
        {
            var readFilter = new ReadFilter
            {
                // From ReadFilterSimple
                IncludeAllLanguages = includeAllLanguages,
                ExpandAnswers = expandAnswers,
                ProjectSpecific = projectSpecific,
                // From ReadFilter
                IncludeAllForms = includeAllForms,
                IncludeHiddenVariable = true,
                IncludeBackgroundVariable = true,
                IncludeRecodedVariable = true
            };
            return readFilter;
        }

        internal static PoetReadFilter NewPoetReadFilter(
            bool includeAllLanguages,
            bool expandAnswers,
            bool projectSpecific,
            bool includeAllForms,
            bool includeAllNodeTypes)
        {
            var poetReadFilter = new PoetReadFilter
            {
                // From ReadFilterSimple
                IncludeAllLanguages = includeAllLanguages,
                ExpandAnswers = expandAnswers,
                ProjectSpecific = projectSpecific,
                // From ReadFilter
                IncludeAllForms = includeAllForms,
                IncludeHiddenVariable = true,
                IncludeBackgroundVariable = true,
                IncludeRecodedVariable = true,
                // From PoetReadFilter
                IncludeChildren = true,
                IncludeText = true,
                IncludeAnswers = true,
                IncludeDeletedNodes = false,
                IncludeProperties = true,
                IncludeAllNodeTypes = includeAllNodeTypes
            };
            return poetReadFilter;
        }

        internal static int IntegerNull()
        {
            return -1;
        }

        internal static double DoubleNull()
        {
            return -9999999999;
        }
    }
}
