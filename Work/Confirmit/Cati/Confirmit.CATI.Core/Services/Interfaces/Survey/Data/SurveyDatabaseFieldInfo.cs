namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public enum OtherFieldType
    {
        HasNoOtherField = 0,
        HasOtherField = 1,
        IsOtherField = 2,

    }

    public class SurveyDatabaseFieldInfo
    {
        public string FieldName;
        public string TableName;
        public OtherFieldType OtherType;
    }
}