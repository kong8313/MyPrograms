using System;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerAvailabilityManager : IDialerAvailabilityManager 
    {
        private IDialerAvailabilityManager _inner;

        public StubIDialerAvailabilityManager()
        {
            _inner = null;
        }

        public IDialerAvailabilityManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void EnableDialerInt32Delegate(int dialerId);
        public EnableDialerInt32Delegate EnableDialerInt32;

        void IDialerAvailabilityManager.EnableDialer(int dialerId)
        {

            if (EnableDialerInt32 != null)
            {
                EnableDialerInt32(dialerId);
            } else if (_inner != null)
            {
                ((IDialerAvailabilityManager)_inner).EnableDialer(dialerId);
            }
        }

        public delegate void EnableDialerInt32BooleanDelegate(int dialerId, bool needToSendNotification);
        public EnableDialerInt32BooleanDelegate EnableDialerInt32Boolean;

        void IDialerAvailabilityManager.EnableDialer(int dialerId, bool needToSendNotification)
        {

            if (EnableDialerInt32Boolean != null)
            {
                EnableDialerInt32Boolean(dialerId, needToSendNotification);
            } else if (_inner != null)
            {
                ((IDialerAvailabilityManager)_inner).EnableDialer(dialerId, needToSendNotification);
            }
        }

        public delegate bool DisableDialerInt32BooleanDelegate(int dialerId, bool withReconnection);
        public DisableDialerInt32BooleanDelegate DisableDialerInt32Boolean;

        bool IDialerAvailabilityManager.DisableDialer(int dialerId, bool withReconnection)
        {


            if (DisableDialerInt32Boolean != null)
            {
                return DisableDialerInt32Boolean(dialerId, withReconnection);
            } else if (_inner != null)
            {
                return ((IDialerAvailabilityManager)_inner).DisableDialer(dialerId, withReconnection);
            }

            return default(bool);
        }

        public delegate bool IsDialerNotificationStateOperationalInt32Delegate(int dialerId);
        public IsDialerNotificationStateOperationalInt32Delegate IsDialerNotificationStateOperationalInt32;

        bool IDialerAvailabilityManager.IsDialerNotificationStateOperational(int dialerId)
        {


            if (IsDialerNotificationStateOperationalInt32 != null)
            {
                return IsDialerNotificationStateOperationalInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerAvailabilityManager)_inner).IsDialerNotificationStateOperational(dialerId);
            }

            return default(bool);
        }

        public delegate bool IsDialerInitializedAndAvaialbleInt32Delegate(int dialerId);
        public IsDialerInitializedAndAvaialbleInt32Delegate IsDialerInitializedAndAvaialbleInt32;

        bool IDialerAvailabilityManager.IsDialerInitializedAndAvaialble(int dialerId)
        {


            if (IsDialerInitializedAndAvaialbleInt32 != null)
            {
                return IsDialerInitializedAndAvaialbleInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerAvailabilityManager)_inner).IsDialerInitializedAndAvaialble(dialerId);
            }

            return default(bool);
        }

        public delegate bool IsConnectedToDialerDialTypeInt32Delegate(DialType dialType, int dialerId);
        public IsConnectedToDialerDialTypeInt32Delegate IsConnectedToDialerDialTypeInt32;

        bool IDialerAvailabilityManager.IsConnectedToDialer(DialType dialType, int dialerId)
        {


            if (IsConnectedToDialerDialTypeInt32 != null)
            {
                return IsConnectedToDialerDialTypeInt32(dialType, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerAvailabilityManager)_inner).IsConnectedToDialer(dialType, dialerId);
            }

            return default(bool);
        }

        public delegate bool ActivateDialerInt32Delegate(int dialerId);
        public ActivateDialerInt32Delegate ActivateDialerInt32;

        bool IDialerAvailabilityManager.ActivateDialer(int dialerId)
        {


            if (ActivateDialerInt32 != null)
            {
                return ActivateDialerInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerAvailabilityManager)_inner).ActivateDialer(dialerId);
            }

            return default(bool);
        }

        public delegate bool DeactivateDialerInt32Delegate(int dialerId);
        public DeactivateDialerInt32Delegate DeactivateDialerInt32;

        bool IDialerAvailabilityManager.DeactivateDialer(int dialerId)
        {


            if (DeactivateDialerInt32 != null)
            {
                return DeactivateDialerInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerAvailabilityManager)_inner).DeactivateDialer(dialerId);
            }

            return default(bool);
        }

        public delegate bool ReconnectDialerInt32Delegate(int dialerId);
        public ReconnectDialerInt32Delegate ReconnectDialerInt32;

        bool IDialerAvailabilityManager.ReconnectDialer(int dialerId)
        {


            if (ReconnectDialerInt32 != null)
            {
                return ReconnectDialerInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerAvailabilityManager)_inner).ReconnectDialer(dialerId);
            }

            return default(bool);
        }

        public delegate bool StopReconnectingDialerInt32Delegate(int dialerId);
        public StopReconnectingDialerInt32Delegate StopReconnectingDialerInt32;

        bool IDialerAvailabilityManager.StopReconnectingDialer(int dialerId)
        {


            if (StopReconnectingDialerInt32 != null)
            {
                return StopReconnectingDialerInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerAvailabilityManager)_inner).StopReconnectingDialer(dialerId);
            }

            return default(bool);
        }

    }
}