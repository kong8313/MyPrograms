namespace Confirmit.CATI.Supervisor.Classes
{
    public interface IFileToBrowserSender
    {
        void Send(BaseForm page, byte[] buffer, string fileName, bool sendInline);
    }
}