using System.ServiceModel;

namespace Confirmit.CATI.Common.WcfTools
{
    public class MessageHeaderAccessor : IMessageHeaderAccessor
    {
        public T GetValueFromHeader<T>(string headerName, string ns)
        {
            T result = default(T);
            if (OperationContext.Current != null)
            {
                var headers = OperationContext.Current.IncomingMessageHeaders;
                int headerIndex = headers.FindHeader(headerName, ns);
                if (headerIndex >= 0)
                {
                    result = headers.GetHeader<T>(headerIndex);
                }
            }

            return result;
        }
    }
}
