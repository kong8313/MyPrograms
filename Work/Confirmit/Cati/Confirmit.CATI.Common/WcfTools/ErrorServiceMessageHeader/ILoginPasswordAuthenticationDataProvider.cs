namespace Confirmit.CATI.Common.WcfTools.ErrorServiceMessageHeader
{
    public interface ILoginPasswordAuthenticationDataProvider
    {
        string Login { get;}
        string Password { get;}
        int CompanyId { get;}
    }
}
