using System;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Classes.Fakes
{
    public class StubIFileToBrowserSender : IFileToBrowserSender 
    {
        private IFileToBrowserSender _inner;

        public StubIFileToBrowserSender()
        {
            _inner = null;
        }

        public IFileToBrowserSender Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendBaseFormArrayOfByteStringBooleanDelegate(BaseForm page, byte[] buffer, string fileName, bool sendInline);
        public SendBaseFormArrayOfByteStringBooleanDelegate SendBaseFormArrayOfByteStringBoolean;

        void IFileToBrowserSender.Send(BaseForm page, byte[] buffer, string fileName, bool sendInline)
        {

            if (SendBaseFormArrayOfByteStringBoolean != null)
            {
                SendBaseFormArrayOfByteStringBoolean(page, buffer, fileName, sendInline);
            } else if (_inner != null)
            {
                ((IFileToBrowserSender)_inner).Send(page, buffer, fileName, sendInline);
            }
        }

    }
}