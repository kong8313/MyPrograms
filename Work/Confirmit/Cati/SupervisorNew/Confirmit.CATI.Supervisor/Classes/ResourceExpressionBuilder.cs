using System.CodeDom;
using System.Web.Compilation;
using System.Web.UI;

namespace Confirmit.CATI.Supervisor.Classes
{
	public class ResourceExpressionBuilder : ExpressionBuilder
	{
		public override CodeExpression GetCodeExpression( BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context )
		{
			string param = entry.Expression;
			return new CodePrimitiveExpression( Confirmit.CATI.Supervisor.Core.Common.ResourceWrapper.Instance.GetString( param ) );			
		}
	}
}