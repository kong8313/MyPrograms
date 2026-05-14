using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public interface ISampleRecordProcessorFactory
    {
        ISampleRecordProcessor Create(SampleContext context);
    }

    public class SampleRecordProcessorFactory : ISampleRecordProcessorFactory
    {
        private readonly IFCDSettings _fcdSettings;

        public SampleRecordProcessorFactory(IFCDSettings fcdSettings)
        {
            _fcdSettings = fcdSettings;
        }

        public ISampleRecordProcessor Create(SampleContext context)
        {
            switch (context.SchedulingMode)
            {
                case SchedulingMode.Full:
                    return new FullSchedulingSampleRecordProcessor(context, _fcdSettings);
                case SchedulingMode.Simple:
                    return new SimpleSchedulingSampleRecordProcessor(context, _fcdSettings);
                default:
                    throw new NotImplementedException("Unexpected sample scheduling mode");
            }
        }
    }
}