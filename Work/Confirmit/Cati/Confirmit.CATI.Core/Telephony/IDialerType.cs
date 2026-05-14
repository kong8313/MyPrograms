namespace Confirmit.CATI.Core.Telephony
{
    public interface IDialerType
    {
        T CreateInstance<T>();
    }
}