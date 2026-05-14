namespace Confirmit.CATI.Core.Services.Database.Interfaces
{
    public interface IDatabaseIdentifierService
    {
        string GetEscapedIdentifier(string identifier);
    }
}