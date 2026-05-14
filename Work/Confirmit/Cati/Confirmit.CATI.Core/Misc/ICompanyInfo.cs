namespace Confirmit.CATI.Core.Misc
{
    public interface ICompanyInfo
    {
        int CompanyId { get; }
        string CompanyName { get; }
        string CompanyAlias { get; }
        int GetCompanyId(int id, string source);
    }
}