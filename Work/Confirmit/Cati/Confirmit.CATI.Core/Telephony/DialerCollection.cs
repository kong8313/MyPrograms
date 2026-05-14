using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.BvCallHandlerLibrary;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Telephony;

namespace Confirmit.CATI.Core.Telephony
{
    public class DialerCollection : IDialerCollection
    {
        private readonly IDialersRepository _dialersRepository;
        private readonly IDialerInstanceFactory _dialerInstanceFactory;

        public DialerCollection(IDialersRepository dialersRepository, IDialerInstanceFactory dialerInstanceFactory)
        {
            _dialersRepository = dialersRepository;
            _dialerInstanceFactory = dialerInstanceFactory;
        }

        private Dictionary<int, IDialerInstance> _dialers;

        public int[] GetDialerIds(DialType dialType)
        {
            VerifyInitialized();
            return _dialers.Where(x=> x.Value.DialType == dialType).Select(d => d.Key).ToArray();
        }

        private void VerifyInitialized()
        {
            if (!CollectionIsInitialized())
            {
                throw new InternalErrorException("DialerCollection is not initialized.");
            }
        }

        public void InitializeCollection()
        {
            var newDialers = _dialersRepository.GetAll().ToDictionary(
                dialerEntity => dialerEntity.Id, 
                dialerEntity => _dialerInstanceFactory.Create(dialerEntity));

            Interlocked.Exchange(ref _dialers, newDialers);
        }

        private bool CollectionIsInitialized()
        {
            return (_dialers != null);
        }

        public IEnumerable<IDialerInstance> GetDialers()
        {
            VerifyInitialized();
            return DialerInstances();
        }

        public IEnumerable<IDialerInstance> GetDialers(DialType dialType)
        {
            VerifyInitialized();
            return DialerInstances().Where(x => x.DialType == dialType).ToArray();
        }

        public IEnumerable<IDialerInstance> GetInitializedDialers(DialType dialType)
        {
            VerifyInitialized();
            return DialerInstances().Where(x => x.DialType == dialType && IsDialerInitialized(x.DialerId)).ToArray();
        }

        private IEnumerable<IDialerInstance> DialerInstances()
        {
            return _dialers.Values;
        }

        public IDialerInstance GetDialerById(int dialerId)
        {
            VerifyInitialized();

            if (!_dialers.TryGetValue(dialerId, out IDialerInstance dialerInstance))
            {
                throw new DialerNotFoundException(dialerId);
            }

            return dialerInstance;
        }

        public IDialerAPI FirstLoadedDialerApi
        {
            get
            {
                var dialerInstance = GetDialers().FirstOrDefault(x => x.Api != null);

                return (dialerInstance == null) ? null : dialerInstance.Api;
            }
        }

        public IDialerInstance GetFirstInitializedDialer(DialType dialType)
        {
            VerifyInitialized();

            return FirstInitializedDialerInstance(dialType);
        }

        /// <returns> Initialized dialer instance or null </returns>
        private IDialerInstance FirstInitializedDialerInstance(DialType dialType)
        {
            if (!CollectionIsInitialized())
            {
                return null;
            }

            return DialerInstances().FirstOrDefault(
                dialerInstance => dialerInstance.DialType == dialType && IsDialerInitialized(dialerInstance.DialerId));
        }

        public bool IsDialerInitialized(int dialerId)
        {
            if (!CollectionIsInitialized())
            {
                return false;
            }

            try
            {
                return GetDialerById(dialerId).IsDialerInitialized;
            }
            catch (Exception)
            {
                // TODO: Why we catch ALL exceptions here???
                //       Must be refactored ideally to avoid catching exception at all or at least should catch just expected exception
                return false;
            }
        }

        public bool InitializedDialerExists()
        {
            return CollectionIsInitialized()
                && DialerInstances().Any(dialerInstance => IsDialerInitialized(dialerInstance.DialerId));
        }

        public bool InitializedDialerExists(DialType dialType)
        {
            return (FirstInitializedDialerInstance(dialType) != null);
        }
    }
}
