using System;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation.Fakes
{
    public class StubISampleRecordProcessorFactory : ISampleRecordProcessorFactory 
    {
        private ISampleRecordProcessorFactory _inner;

        public StubISampleRecordProcessorFactory()
        {
            _inner = null;
        }

        public ISampleRecordProcessorFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate ISampleRecordProcessor CreateSampleContextDelegate(SampleContext context);
        public CreateSampleContextDelegate CreateSampleContext;

        ISampleRecordProcessor ISampleRecordProcessorFactory.Create(SampleContext context)
        {


            if (CreateSampleContext != null)
            {
                return CreateSampleContext(context);
            } else if (_inner != null)
            {
                return ((ISampleRecordProcessorFactory)_inner).Create(context);
            }

            return default(ISampleRecordProcessor);
        }

    }
}