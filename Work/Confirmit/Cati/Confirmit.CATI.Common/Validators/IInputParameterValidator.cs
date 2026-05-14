namespace Confirmit.CATI.Common.Validators
{
    public interface IInputParameterValidator
    {
        string InvalidSymbols { get; }
        string ValidStringMask { get; }

        bool IsValid(string value);
        bool IsValidEmail(string emailString);
        bool IsValidQuestionId(string questionId);
    }
}
