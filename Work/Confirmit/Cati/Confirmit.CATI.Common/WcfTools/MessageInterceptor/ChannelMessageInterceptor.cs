using System.ServiceModel.Channels;

namespace Confirmit.CATI.Common.WcfTools.MessageInterceptor
{
    /// <summary>
    /// Allows the derived classes to intercept all incoming and outgoing WCF messages at a channel level.
    /// Implementation based on the MSDN sample Custom Message Interceptor:
    /// http://msdn.microsoft.com/en-us/library/ms751495.aspx
    /// </summary>
    /// <remarks>
    /// This sample demonstrates the use of the channel extensibility model.
    /// In particular, it shows how to implement a custom binding
    /// element that creates channel factories and channel listeners to intercept
    /// all incoming and outgoing messages at a particular point in the run-time stack.
    /// </remarks>
    public abstract class ChannelMessageInterceptor
    {
        /// <summary>
        /// Called when outgoing Message is ready to be sent.
        /// </summary>
        /// <param name="message">The message going to be sent.</param>
        public virtual void OnSend(ref Message message){}

        /// <summary>
        /// Called when incoming message has been received.
        /// </summary>
        /// <param name="message">The message just received from the other party.</param>
        public virtual void OnReceive(ref Message message){}

        public abstract ChannelMessageInterceptor Clone();
    }
}