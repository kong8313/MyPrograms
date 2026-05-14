namespace Confirmit.CATI.Installation.Common.Interfaces
{
    public interface IIsAliveHtmEngine
    {
        void VerifyAccesToPageByUrl(string urlAddress);

        bool BackupIsAliveHtmFile(string isAlivePageUrl);

        void RestoreIsAliveHtmFile(string isAlivePageUrl);
    }
}