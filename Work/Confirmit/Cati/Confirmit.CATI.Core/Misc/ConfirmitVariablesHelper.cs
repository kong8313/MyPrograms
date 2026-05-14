namespace Confirmit.CATI.Core.Misc
{
    public class ConfirmitVariablesHelper
    {
        private static string _confirmitVariableAliasPrefix = "Var";

        /// <summary>
        /// Determines if given alias is confirmit variable alias.
        /// </summary>
        /// <param name="variableAlias">Variable alias.</param>
        /// <returns>true, if it is confirmit variable alias; otherwise false.</returns>
        public static bool IsComfirmitVariableAlias(string variableAlias)
        {
            return variableAlias.StartsWith(_confirmitVariableAliasPrefix);
        }

        /// <summary>
        /// Extracts confirmit variable name from confirmit variable alias.
        /// This function doesn't check if given variable alias is confirmit
        /// variable alias.
        /// </summary>
        /// <param name="variableAlias">Confirmit variable alias.</param>
        /// <returns>Confirmit variable name.</returns>
        public static string ExtractNameFromConfirmitVariableAlias(string variableAlias)
        {
            return variableAlias.Substring(_confirmitVariableAliasPrefix.Length);
        }

        /// <summary>
        /// Gets the confirmit variable alias. 
        /// Alias is used instead of name to avoid problems when variable name is the same as other column's key.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        public static string GetConfirmitVariableAlias(string variableName)
        {
            return _confirmitVariableAliasPrefix + variableName;
        }
    }
}
