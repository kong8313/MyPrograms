namespace Confirmit.CATI.Common.WcfTools
{
    public interface IMessageHeaderAccessor
    {
        T GetValueFromHeader<T>(string headerName, string ns); 
    }
}