using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public class SampleDataStorageRepository : ISampleDataStorageRepository
    {
        private ISampleDataStorageFactory _sampleDataStorageFactory;

        private readonly Dictionary<int, ISampleDataStorage> SampleDataStorages = new Dictionary<int, ISampleDataStorage>();

        public SampleDataStorageRepository(ISampleDataStorageFactory sampleDataStorageFactory)
        {
            _sampleDataStorageFactory = sampleDataStorageFactory;
        }

        public ISampleDataStorage Create(BvSurveyEntity survey, int operationId)
        {
            var storage = Create(0, survey, 0, false);
            storage.OperationId = operationId;
            return storage;
        }

        public ISampleDataStorage Create(int batchID, BvSurveyEntity survey, int startRangeOfInterviewId, bool isUpdateMode)
        {
            var storage = _sampleDataStorageFactory.Create(batchID, survey, startRangeOfInterviewId, isUpdateMode);

            lock (SampleDataStorages)
            {
                if (SampleDataStorages.ContainsKey(batchID))
                    throw new InternalErrorException(String.Format(
                        "The SampleDataStorage with batchID = {0} already exists", batchID));
                SampleDataStorages.Add(batchID, storage);
            }

            return storage;
        }

        /// <summary>
        /// this method creates SampleDataStorage object for coresponding batch and survey
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public ISampleDataStorage Get(int batchId)
        {
            lock (SampleDataStorages)
            {
                if (SampleDataStorages.ContainsKey(batchId))
                    return SampleDataStorages[batchId];
            }

            // Exception IMHO needed
            return null;
        }

        public void Delete(int batchId)
        {
            lock (SampleDataStorages)
            {
                if (SampleDataStorages.ContainsKey(batchId))
                    SampleDataStorages.Remove(batchId);
            }
        }
    }
}