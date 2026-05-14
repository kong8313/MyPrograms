namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data
{
    public class QuotaInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Table { get; set; }
        public string[] Fields { get; set; }
    }
}