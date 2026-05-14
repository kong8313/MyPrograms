using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.CATI.Supervisor.Core.BlackList
{
    public class BlackListService : IBlackListService
    {
        private readonly ICallCenterProvider _callCenterProvider;
        private readonly ITelephoneBlacklistRepository _telephoneBlacklistRepository;
        private readonly ISupervisorNameProvider _supervisorNameProvider;
        private readonly IAsyncOperationQueue _asyncOperationQueue;

        public BlackListService(ICallCenterProvider callCenterProvider,
            ITelephoneBlacklistRepository telephoneBlacklistRepository,
            ISupervisorNameProvider supervisorNameProvider,
            IAsyncOperationQueue asyncOperationQueue)
        {
            _callCenterProvider = callCenterProvider;
            _telephoneBlacklistRepository = telephoneBlacklistRepository;
            _supervisorNameProvider = supervisorNameProvider;
            _asyncOperationQueue = asyncOperationQueue;
        }

        public void AddNumber(BvTelephoneBlacklistEntity entity)
        {
            var evt = new AddTelephoneNumberToBlacklistEvent(entity.TelephoneNumber);
            entity.DisplayPattern = entity.TelephoneNumber;
            entity.Timestamp = DateTime.UtcNow;
            
            _telephoneBlacklistRepository.Insert(entity);

            evt.Finish();
        }

        public void UpdateNumber(string oldNumber, BvTelephoneBlacklistEntity entity)
        {
            var numberEntity = _telephoneBlacklistRepository.GetByDisplayPattern(oldNumber);
            var evt = new UpdateTelephoneNumberInBlacklistEvent(numberEntity.Id, entity.TelephoneNumber);

            numberEntity.DisplayPattern = entity.TelephoneNumber;
            numberEntity.Timestamp = DateTime.UtcNow;
            numberEntity.Comment = entity.Comment;
            _telephoneBlacklistRepository.Update(numberEntity);

            evt.Finish();
        }

        public void ImportNumbers(IEnumerable<string> numbers)
        {
            var entityNumbers = numbers.Select(number => new BvTelephoneBlacklistEntity
                                    {
                                        DisplayPattern = number
                                    }).ToList();

            _telephoneBlacklistRepository.Import(entityNumbers);
        }
    }
}
