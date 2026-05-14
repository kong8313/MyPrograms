namespace Confirmit.CATI.Core.Services.Survey.Quota
{
    public class QuotaCellData
    {
        public QuotaCellFieldValue [] FieldValues { get; set; }
    }

    public class QuotaCellFieldValue
    {
        public string Field { get; set; }

        public string Value { get; set; }
    }
}