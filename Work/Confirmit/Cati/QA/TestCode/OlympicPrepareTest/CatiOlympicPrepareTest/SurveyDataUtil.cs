using System;
using System.Collections;
using CatiOlympicPrepareTest.wsSurveyData;

namespace CatiOlympicPrepareTest
{
    public class SurveyDataUtil
    {
        public static string RespondentTableName = "respondent";

        public static RespondentSystemVariables NewRespondentSystemVariables(
            bool includeRespId,
            bool includeUserId,
            bool includeLastUpdated,
            bool includeNoOfEmailsSent,
            bool includeSid)
        {
            var respondentSystemVariables = new RespondentSystemVariables
            {
                IncludeRespId = includeRespId,
                IncludeUserId = includeUserId,
                IncludeLastUpdated = includeLastUpdated,
                IncludeNoOfEmailsSent = includeNoOfEmailsSent,
                IncludeSID = includeSid
            };
            return respondentSystemVariables;
        }

        public static RespondentTransferDef NewRespondentTransferDef(
            bool allFields,
            bool onlySchema,
            string projectId)
        {
            var respondentTransferDef = new RespondentTransferDef
            {
                AllFields = allFields,
                ProjectId = projectId,
                OnlySchema = onlySchema
            };
            return respondentTransferDef;
        }

        public static ExpressionList NewExpressionList()
        {
            return new ExpressionList();
        }

        public static SelectClause NewSelectClause()
        {
            return new SelectClause();
        }

        public static CategoryTotal NewCategoryTotal()
        {
            return new CategoryTotal();
        }

        public static FieldCategorization NewFieldCategorization(string alias)
        {
            var fieldCategorization = new FieldCategorization
            {
                Alias = alias
            };
            return fieldCategorization;
        }

        public static Categorization NewCategorization(string alias)
        {
            var categorization = new Categorization
            {
                Alias = alias
            };
            return categorization;
        }

        public static TimeseriesDimension NewTimeseriesDimension(LocaleString name)
        {
            var timeseriesDimension = new TimeseriesDimension
            {
                DayOfWeek = wsSurveyData.DayOfWeek.Sunday,
                SuffixTotalCatWithParent = true,
                GroupBy = true,
                Name = name
            };
            return timeseriesDimension;
        }

        public static Case NewCase()
        {
            return new Case();
        }

        public static SystemVariables NewSystemVariables(
            bool includeInterviewEnd,
            bool includeInterviewStart,
            bool includeIterationId,
            bool includeLastComplete,
            bool includeRespId,
            bool includeRowguid,
            bool includeStatus)
        {
            var systemVariables = new SystemVariables
            {
                IncludeInterviewEnd = includeInterviewEnd,
                IncludeInterviewStart = includeInterviewStart,
                IncludeIterationId = includeIterationId,
                IncludeLastComplete = includeLastComplete,
                IncludeRespId = includeRespId,
                IncludeRowguid = includeRowguid,
                IncludeStatus = includeStatus
            };
            return systemVariables;
        }

        public static GenericSqlFunction NewGenericSqlFunction(GenericSqlFunctionType type)
        {
            var genericSqlFunction = new GenericSqlFunction
            {
                Type = type
            };
            return genericSqlFunction;
        }

        public static SelectStatement NewSelectStatement()
        {
            var selectStatement = new SelectStatement
            {
                UnionAll = true,
                TopN = -1
            };
            return selectStatement;
        }

        public static TransferDef NewTransferDef(string projectId, bool allChildLevels, DatabaseType dbType)
        {
            var transferDef = new TransferDef
            {
                ProjectId = projectId,
                AllChildrenLevels = allChildLevels,
                DbType = dbType,
                Key = "responseid"
            };
            return transferDef;
        }

        public static TransferLevel NewTransferLevel(string loopId, bool allChildForms)
        {
            var transferLevel = new TransferLevel
            {
                AllChildrenForms = allChildForms,
                LoopId = loopId
            };
            return transferLevel;
        }

        public static TransferForm NewTransferForm(string formName, bool allChildFields)
        {
            var transferForm = new TransferForm
            {
                AllChildrenFields = allChildFields,
                Name = formName
            };
            return transferForm;
        }

        public static TransferField NewTransferField(string fieldName)
        {
            var transferField = new TransferField
            {
                Name = fieldName
            };
            return transferField;
        }

        public static QueryProject NewQueryProject(string projectId, DatabaseType dbType)
        {
            var queryProject = new QueryProject
            {
                ProjectId = projectId,
                DBType = dbType
            };
            return queryProject;
        }

        public static QueryForm NewQueryForm(string name)
        {
            var queryForm = new QueryForm
            {
                Name = name,
                OnlyBasicFields = true
            };
            return queryForm;
        }

        public static LocaleStringSimple NewLocaleStringSimple(int lang, string text)
        {
            var localeStringSimple = new LocaleStringSimple();
            var languageString = new LanguageString
            {
                Language = lang,
                Value = text
            };
            localeStringSimple.Strings = new [] { languageString };
            return localeStringSimple;
        }

        public static CategoryField NewCategoryField(int language, string name)
        {
            var categoryField = new CategoryField
            {
                Name = NewLocaleStringSimple(language, name)
            };
            return categoryField;
        }

        public static BinaryArithmetic NewBinaryArithmetic(object leftArgument, object rightArgument, ArithmeticType type)
        {
            var binaryArithmetic = new BinaryArithmetic
            {
                Item = leftArgument,
                Item1 = rightArgument,
                Type = type
            };
            return binaryArithmetic;
        }

        public static QueryField NewQueryField(string name)
        {
            var queryField = new QueryField
            {
                Name = name
            };
            return queryField;
        }

        public static BinaryComparison NewBinaryComparison(ComparisonType type, object leftArgument, object rightArgument)
        {
            var binaryComparison = new BinaryComparison
            {
                Type = type,
                Item = leftArgument,
                Item1 = rightArgument
            };
            return binaryComparison;
        }

        public static WhereClause NewWhereClause(object expression)
        {
            var whereClause = new WhereClause
            {
                Item = expression
            };
            return whereClause;
        }

        public static SnowflakeDimension NewSnowflakeDimension(LocaleString name)
        {
            var snowflakeDimension = new SnowflakeDimension
            {
                SuffixTotalCatWithParent = true,
                GroupBy = true,
                Name = name
            };
            return snowflakeDimension;
        }

        public static SimpleDimension NewSimpleDimension(object selectExpression)
        {
            var simpleDimension = new SimpleDimension
            {
                Item = selectExpression,
                SuffixTotalCatWithParent = true,
                GroupBy = true
            };
            return simpleDimension;
        }

        public static CategoryFormElements NewCategoryFormElements(int language, string name, ListType type)
        {
            var categoryFormElements = new CategoryFormElements
            {
                Name = NewLocaleStringSimple(language, name),
                List = type
            };
            return categoryFormElements;
        }

        public static FunctionCategorization NewFunctionCategorization(string alias, UnaryType[] functions)
        {
            var functionCategorization = new FunctionCategorization
            {
                Alias = alias
            };
            foreach (UnaryType function in functions)
            {
                // ReSharper disable once CoVariantArrayConversion
                functionCategorization.Categories = (Category[])Add(functionCategorization.Categories, NewCategoryFunction(function), typeof(Category));
            }

            return functionCategorization;
        }

        public static CategoryFunction NewCategoryFunction(UnaryType function)
        {
            var categoryFunction = new CategoryFunction
            {
                Function = function
            };
            return categoryFunction;
        }

        public static CategoryAnswer NewCategoryAnswer(string formName, string code)
        {
            var categoryAnswer = new CategoryAnswer
            {
                Code = code,
                FormName = formName
            };
            return categoryAnswer;
        }

        public static TextForCategory NewTextForCategory(string code, LocaleStringSimple text)
        {
            var textForCategory = new TextForCategory
            {
                Text = text,
                Code = code
            };
            return textForCategory;
        }

        public static Unary NewUnary(UnaryType type, string alias, object argument)
        {
            var unary = new Unary
            {
                Type = type,
                Item = argument,
                Alias = alias
            };
            return unary;
        }

        public static Category NewCategory(int language, string name)
        {
            var category = new Category
            {
                Name = NewLocaleStringSimple(language, name)
            };
            return category;
        }

        public static CodeMask NewCodeMask(bool exclusive, string[] code)
        {
            var codeMask = new CodeMask
            {
                Exclusive = exclusive,
                Code = code
            };
            return codeMask;
        }

        public static BinaryLogic NewBinaryLogic(LogicType type, object leftArgument, object rightArgument)
        {
            var binaryLogic = new BinaryLogic
            {
                Type = type,
                Item = leftArgument,
                Item1 = rightArgument
            };
            return binaryLogic;
        }

        public static CaseWhenThen NewCaseWhenThen(object whenExpression, object thenExpression)
        {
            var caseWhenThen = new CaseWhenThen
            {
                Item = whenExpression,
                Item1 = thenExpression
            };
            return caseWhenThen;
        }

        public static QueryConstant NewQueryConstant(ConfirmitDbType type, object val)
        {
            var queryConstant = new QueryConstant
            {
                Type = type,
                Value = val
            };
            return queryConstant;
        }

        public static SurveyQuery NewSurveyQuery(string alias, QueryProject queryProject)
        {
            var surveyQuery = new SurveyQuery
            {
                DefaultProject = queryProject,
                Alias = alias
            };
            return surveyQuery;
        }

        public static Array Add(object[] existingArray, object newItem, Type t)
        {
            ArrayList arrayList = existingArray != null ? new ArrayList(existingArray) : new ArrayList();
            arrayList.Add(newItem);
            return arrayList.ToArray(t);
        }
    }
}
