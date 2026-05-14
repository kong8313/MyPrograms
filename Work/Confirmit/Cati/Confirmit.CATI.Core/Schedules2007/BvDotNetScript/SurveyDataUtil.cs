using System.Collections.Generic;
using System.Collections;
using BvDotNetScript.SurveyDataApiWS;
using Confirmit.CATI.Core.SurveyDataService;
using DayOfWeek=System.DayOfWeek;

namespace BvDotNetScript.SurveyDataApiWS.Util
{
	/// <summary>
	/// Wrapper-class for use of SurveyData
	/// in web-services.  Implements virtual constructors
	/// and support collection-functionality
	/// 
	/// This class is copied and pasted from the
	/// API documentation
	/// </summary>
	public class SurveyDataUtil
	{
		public SurveyDataUtil()
		{
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
			CategoryTotal c = new CategoryTotal();
			return c;
		}

		public static FieldCategorization NewFieldCategorization(string alias)
		{
			FieldCategorization f = new FieldCategorization();
			f.Alias = alias;
			return f;
		}

		public static Categorization NewCategorization(string alias)
		{
			Categorization c = new Categorization();
			c.Alias = alias;
			return c;
		}

		public static TimeseriesDimension NewTimeseriesDimension(LocaleString name)
		{
			TimeseriesDimension t = new TimeseriesDimension();
			t.DayOfWeek = Confirmit.CATI.Core.SurveyDataService.DayOfWeek.Sunday;
			t.SuffixTotalCatWithParent = true;
			t.GroupBy = true;
			t.Name = name;
			return t;
		}

		public static Case NewCase()
		{
			Case c = new Case();
			return c;
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
			SystemVariables s = new SystemVariables();
			s.IncludeInterviewEnd = includeInterviewEnd;
			s.IncludeInterviewStart = includeInterviewStart;
			s.IncludeIterationId = includeIterationId;
			s.IncludeLastComplete = includeLastComplete;
			s.IncludeRespId = includeRespId;
			s.IncludeRowguid = includeRowguid;
			s.IncludeStatus = includeStatus;
			return s;
		}

		public static GenericSqlFunction NewGenericSqlFunction(GenericSqlFunctionType type)
		{
			GenericSqlFunction g = new GenericSqlFunction();
			g.Type = type;
			return g;
		}

		public static SelectStatement NewSelectStatement()
		{
			SelectStatement s = new SelectStatement();
			s.UnionAll = true;
			s.TopN = -1;
			return s;
		}

		public static TransferDef NewTransferDef(string projectID,bool allChildLevels,DatabaseType dbType)
		{
			TransferDef t = new TransferDef();
			t.ProjectId = projectID;
			t.AllChildrenLevels = allChildLevels;
			t.DbType = dbType;
			t.Key = "responseid";
			return t;
		}

		public static TransferLevel NewTransferLevel(string loopId,bool allChildForms)
		{
			TransferLevel t = new TransferLevel();
			t.AllChildrenForms = allChildForms;
			t.LoopId = loopId;
			return t;
		}

		public static TransferForm NewTransferForm(string formName,bool allChildFields)
		{
			TransferForm t = new TransferForm();
			t.AllChildrenFields = allChildFields;
			t.Name = formName;
			return t;
		}

		public static TransferField NewTransferField(string fieldName)
		{
			TransferField t = new TransferField();
			t.Name = fieldName;
			return t;
		}

		public static QueryProject NewQueryProject(string projectID,DatabaseType dbType)
		{
			QueryProject qp = new QueryProject();
			qp.ProjectId = projectID;
			qp.DBType = dbType;
			return qp;
		}

		public static QueryForm NewQueryForm(string name)
		{
			QueryForm qf = new QueryForm();
			qf.Name = name;
			qf.OnlyBasicFields = true;
			return qf;
		}

		public static LocaleStringSimple NewLocaleStringSimple(int lang,string text)
		{
			// @exstart(LocaleString)
			LocaleStringSimple lss = new LocaleStringSimple();
			LanguageString ls = new LanguageString();
			ls.Language = lang;
			ls.Value = text;
			lss.Strings = new LanguageString[1] { ls };
			return lss;
			// @exend(LocaleString)
		}

		public static CategoryField NewCategoryField(int language,string name)
		{
			CategoryField cf = new CategoryField();
			cf.Name = NewLocaleStringSimple(language,name);
			return cf;
		}

		public static BinaryArithmetic NewBinaryArithmetic(object leftArgument,object rightArgument,ArithmeticType type)
		{
			BinaryArithmetic b = new BinaryArithmetic();
			b.Item = leftArgument;
			b.Item1 = rightArgument;
			b.Type = type;
			return b;
		}

		public static QueryField NewQueryField(string name)
		{
			QueryField qf = new QueryField();
			qf.Name = name;
			return qf;
		}

		public static BinaryComparison NewBinaryComparison(ComparisonType type,object leftArgument,object rightArgument)
		{
			BinaryComparison bc = new BinaryComparison();
			bc.Type = type;
			bc.Item = leftArgument;
			bc.Item1 = rightArgument;
			return bc;
		}

		public static WhereClause NewWhereClause(object expression)
		{
			WhereClause w = new WhereClause();
			w.Item = expression;
			return w;
		}

		public static SnowflakeDimension NewSnowflakeDimension(LocaleString name)
		{
			SnowflakeDimension s = new SnowflakeDimension();
			s.SuffixTotalCatWithParent = true;
			s.GroupBy = true;
			s.Name = name;
			return s;
		}

		public static SimpleDimension NewSimpleDimension(object selectExpression)
		{
			SimpleDimension sd = new SimpleDimension();
			sd.Item = selectExpression;
			sd.SuffixTotalCatWithParent = true;
			sd.GroupBy = true;
			return sd;
		}

		public static CategoryFormElements NewCategoryFormElements(int language,string name,ListType type)
		{
			CategoryFormElements c = new CategoryFormElements();
			c.Name = NewLocaleStringSimple(language,name);
			c.List = type;
			return c;
		}

		public static FunctionCategorization NewFunctionCategorization(string alias,UnaryType[] functions)
		{
			FunctionCategorization f = new FunctionCategorization();
			f.Alias = alias;
			foreach (UnaryType function in functions)
				f.Categories =
					(Category[])Add(f.Categories,NewCategoryFunction(function),typeof(Category));
			return f;
		}

		public static CategoryFunction NewCategoryFunction(UnaryType function)
		{
			CategoryFunction cf = new CategoryFunction();
			cf.Function = function;
			return cf;
		}

		public static CategoryAnswer NewCategoryAnswer(string formName,string code)
		{
			CategoryAnswer ca = new CategoryAnswer();
			ca.Code = code;
			ca.FormName = formName;
			return ca;
		}

		public static TextForCategory NewTextForCategory(string code,LocaleStringSimple text)
		{
			TextForCategory tfc = new TextForCategory();
			tfc.Text = text;
			tfc.Code = code;
			return tfc;
		}

		public static Unary NewUnary(UnaryType type, string alias,object argument)
		{
			Unary u = new Unary();
			u.Type = type;
			u.Item = argument;
			u.Alias = alias;
			return u;
		}

		public static Category NewCategory(int language,string name)
		{
			Category c = new Category();
			c.Name = NewLocaleStringSimple(language,name);
			return c;
		}

		public static CodeMask NewCodeMask(bool exclusive,string[] code)
		{
			CodeMask cm = new CodeMask();
			cm.Exclusive = exclusive;
			cm.Code = code;
			return cm;
		}

		public static BinaryLogic NewBinaryLogic(LogicType type,object leftArgument,object rightArgument)
		{
			BinaryLogic bl = new BinaryLogic();
			bl.Type = type;
			bl.Item = leftArgument;
			bl.Item1 = rightArgument;
			return bl;
		}

		public static CaseWhenThen NewCaseWhenThen(object whenExpression,object thenExpression)
		{
			CaseWhenThen cwt = new CaseWhenThen();
			cwt.Item = whenExpression;
			cwt.Item1 = thenExpression;
			return cwt;
		}

		public static QueryConstant NewQueryConstant(ConfirmitDbType type,object val)
		{
			QueryConstant qc = new QueryConstant();
			qc.Type = type;
			qc.Value = val;
			return qc;
		}

		public static SurveyQuery NewSurveyQuery(string alias,QueryProject queryProject)
		{
			SurveyQuery sq = new SurveyQuery();
			sq.DefaultProject = queryProject;
			sq.Alias = alias;
			return sq;
		}

		public static System.Array Add(object[] existingArray,object newItem,System.Type t)
		{
			ArrayList al;
			if (existingArray!=null)
				al = new ArrayList((System.Array)existingArray);
			else
				al = new ArrayList();
			al.Add(newItem);
			return al.ToArray(t);
		}

	}
}
