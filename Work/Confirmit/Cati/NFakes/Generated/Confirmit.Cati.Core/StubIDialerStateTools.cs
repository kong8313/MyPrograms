using System;
using BvCallHandlerLibrary;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace BvCallHandlerLibrary.Fakes
{
    public class StubIDialerStateTools : IDialerStateTools 
    {
        private IDialerStateTools _inner;

        public StubIDialerStateTools()
        {
            _inner = null;
        }

        public IDialerStateTools Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string BvDialerStateToStringInt32Delegate(int dialerId);
        public BvDialerStateToStringInt32Delegate BvDialerStateToStringInt32;

        string IDialerStateTools.BvDialerStateToString(int dialerId)
        {


            if (BvDialerStateToStringInt32 != null)
            {
                return BvDialerStateToStringInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerStateTools)_inner).BvDialerStateToString(dialerId);
            }

            return default(string);
        }

        public delegate void SetDialerNotificationExpirationTimeInt32DateTimeDelegate(int dialerId, DateTime expirationTime);
        public SetDialerNotificationExpirationTimeInt32DateTimeDelegate SetDialerNotificationExpirationTimeInt32DateTime;

        void IDialerStateTools.SetDialerNotificationExpirationTime(int dialerId, DateTime expirationTime)
        {

            if (SetDialerNotificationExpirationTimeInt32DateTime != null)
            {
                SetDialerNotificationExpirationTimeInt32DateTime(dialerId, expirationTime);
            } else if (_inner != null)
            {
                ((IDialerStateTools)_inner).SetDialerNotificationExpirationTime(dialerId, expirationTime);
            }
        }

        public delegate void UpdateDialerStateNotificationTimeInt32Delegate(int dialerId);
        public UpdateDialerStateNotificationTimeInt32Delegate UpdateDialerStateNotificationTimeInt32;

        void IDialerStateTools.UpdateDialerStateNotificationTime(int dialerId)
        {

            if (UpdateDialerStateNotificationTimeInt32 != null)
            {
                UpdateDialerStateNotificationTimeInt32(dialerId);
            } else if (_inner != null)
            {
                ((IDialerStateTools)_inner).UpdateDialerStateNotificationTime(dialerId);
            }
        }

        public delegate void UpdateGetStateTimeInt32BooleanDelegate(int dialerId, bool isGetStateSuccessful);
        public UpdateGetStateTimeInt32BooleanDelegate UpdateGetStateTimeInt32Boolean;

        void IDialerStateTools.UpdateGetStateTime(int dialerId, bool isGetStateSuccessful)
        {

            if (UpdateGetStateTimeInt32Boolean != null)
            {
                UpdateGetStateTimeInt32Boolean(dialerId, isGetStateSuccessful);
            } else if (_inner != null)
            {
                ((IDialerStateTools)_inner).UpdateGetStateTime(dialerId, isGetStateSuccessful);
            }
        }

        public delegate bool IsGetStateTimeoutElapsedInt32DateTimeOutDelegate(int dialerId, out DateTime lastSuccessfulGetState);
        public IsGetStateTimeoutElapsedInt32DateTimeOutDelegate IsGetStateTimeoutElapsedInt32DateTimeOut;

        bool IDialerStateTools.IsGetStateTimeoutElapsed(int dialerId, out DateTime lastSuccessfulGetState)
        {
            lastSuccessfulGetState = default(DateTime);


            if (IsGetStateTimeoutElapsedInt32DateTimeOut != null)
            {
                return IsGetStateTimeoutElapsedInt32DateTimeOut(dialerId, out lastSuccessfulGetState);
            } else if (_inner != null)
            {
                return ((IDialerStateTools)_inner).IsGetStateTimeoutElapsed(dialerId, out lastSuccessfulGetState);
            }

            return default(bool);
        }

        public delegate bool IsReconnectTimeoutElapsedBvDialersEntityDelegate(BvDialersEntity dialer);
        public IsReconnectTimeoutElapsedBvDialersEntityDelegate IsReconnectTimeoutElapsedBvDialersEntity;

        bool IDialerStateTools.IsReconnectTimeoutElapsed(BvDialersEntity dialer)
        {


            if (IsReconnectTimeoutElapsedBvDialersEntity != null)
            {
                return IsReconnectTimeoutElapsedBvDialersEntity(dialer);
            } else if (_inner != null)
            {
                return ((IDialerStateTools)_inner).IsReconnectTimeoutElapsed(dialer);
            }

            return default(bool);
        }

    }
}