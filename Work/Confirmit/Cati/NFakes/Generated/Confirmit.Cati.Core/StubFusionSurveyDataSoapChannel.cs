using System;
using Confirmit.CATI.Core.SurveyDataService;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Confirmit.CATI.Core.SurveyDataService.Fakes
{
    public class StubFusionSurveyDataSoapChannel : FusionSurveyDataSoapChannel 
    {
        private FusionSurveyDataSoapChannel _inner;

        public StubFusionSurveyDataSoapChannel()
        {
            _inner = null;
        }

        public FusionSurveyDataSoapChannel Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate TransferResult GetDataStringTransferDefBaseResponseTokenDelegate(string key, TransferDefBase transferDef, ResponseToken token);
        public GetDataStringTransferDefBaseResponseTokenDelegate GetDataStringTransferDefBaseResponseToken;

        TransferResult FusionSurveyDataSoap.GetData(string key, TransferDefBase transferDef, ResponseToken token)
        {


            if (GetDataStringTransferDefBaseResponseToken != null)
            {
                return GetDataStringTransferDefBaseResponseToken(key, transferDef, token);
            } else if (_inner != null)
            {
                return ((FusionSurveyDataSoap)_inner).GetData(key, transferDef, token);
            }

            return default(TransferResult);
        }

        public delegate ErrorMessage[] UpdateDataStringTransferDefDataSetBooleanBooleanInt32Delegate(string key, TransferDef transferDef, DataSet ds, bool applyRules, bool inTransaction, int transactionKey);
        public UpdateDataStringTransferDefDataSetBooleanBooleanInt32Delegate UpdateDataStringTransferDefDataSetBooleanBooleanInt32;

        ErrorMessage[] FusionSurveyDataSoap.UpdateData(string key, TransferDef transferDef, DataSet ds, bool applyRules, bool inTransaction, int transactionKey)
        {


            if (UpdateDataStringTransferDefDataSetBooleanBooleanInt32 != null)
            {
                return UpdateDataStringTransferDefDataSetBooleanBooleanInt32(key, transferDef, ds, applyRules, inTransaction, transactionKey);
            } else if (_inner != null)
            {
                return ((FusionSurveyDataSoap)_inner).UpdateData(key, transferDef, ds, applyRules, inTransaction, transactionKey);
            }

            return default(ErrorMessage[]);
        }

        public delegate void DisplayInitializationUIDelegate();
        public DisplayInitializationUIDelegate DisplayInitializationUI;

        void IClientChannel.DisplayInitializationUI()
        {

            if (DisplayInitializationUI != null)
            {
                DisplayInitializationUI();
            } else if (_inner != null)
            {
                ((IClientChannel)_inner).DisplayInitializationUI();
            }
        }

        public delegate IAsyncResult BeginDisplayInitializationUIAsyncCallbackObjectDelegate(AsyncCallback callback, Object state);
        public BeginDisplayInitializationUIAsyncCallbackObjectDelegate BeginDisplayInitializationUIAsyncCallbackObject;

        IAsyncResult IClientChannel.BeginDisplayInitializationUI(AsyncCallback callback, Object state)
        {


            if (BeginDisplayInitializationUIAsyncCallbackObject != null)
            {
                return BeginDisplayInitializationUIAsyncCallbackObject(callback, state);
            } else if (_inner != null)
            {
                return ((IClientChannel)_inner).BeginDisplayInitializationUI(callback, state);
            }

            return default(IAsyncResult);
        }

        public delegate void EndDisplayInitializationUIIAsyncResultDelegate(IAsyncResult result);
        public EndDisplayInitializationUIIAsyncResultDelegate EndDisplayInitializationUIIAsyncResult;

        void IClientChannel.EndDisplayInitializationUI(IAsyncResult result)
        {

            if (EndDisplayInitializationUIIAsyncResult != null)
            {
                EndDisplayInitializationUIIAsyncResult(result);
            } else if (_inner != null)
            {
                ((IClientChannel)_inner).EndDisplayInitializationUI(result);
            }
        }

        T IChannel.GetProperty<T>()
        {


            return default(T);
        }

        public delegate void AbortDelegate();
        public AbortDelegate Abort;

        void ICommunicationObject.Abort()
        {

            if (Abort != null)
            {
                Abort();
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).Abort();
            }
        }

        public delegate IAsyncResult BeginCloseAsyncCallbackObjectDelegate(AsyncCallback callback, Object state);
        public BeginCloseAsyncCallbackObjectDelegate BeginCloseAsyncCallbackObject;

        IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, Object state)
        {


            if (BeginCloseAsyncCallbackObject != null)
            {
                return BeginCloseAsyncCallbackObject(callback, state);
            } else if (_inner != null)
            {
                return ((ICommunicationObject)_inner).BeginClose(callback, state);
            }

            return default(IAsyncResult);
        }

        public delegate IAsyncResult BeginCloseTimeSpanAsyncCallbackObjectDelegate(TimeSpan timeout, AsyncCallback callback, Object state);
        public BeginCloseTimeSpanAsyncCallbackObjectDelegate BeginCloseTimeSpanAsyncCallbackObject;

        IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, Object state)
        {


            if (BeginCloseTimeSpanAsyncCallbackObject != null)
            {
                return BeginCloseTimeSpanAsyncCallbackObject(timeout, callback, state);
            } else if (_inner != null)
            {
                return ((ICommunicationObject)_inner).BeginClose(timeout, callback, state);
            }

            return default(IAsyncResult);
        }

        public delegate void EndCloseIAsyncResultDelegate(IAsyncResult result);
        public EndCloseIAsyncResultDelegate EndCloseIAsyncResult;

        void ICommunicationObject.EndClose(IAsyncResult result)
        {

            if (EndCloseIAsyncResult != null)
            {
                EndCloseIAsyncResult(result);
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).EndClose(result);
            }
        }

        public delegate void OpenDelegate();
        public OpenDelegate Open;

        void ICommunicationObject.Open()
        {

            if (Open != null)
            {
                Open();
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).Open();
            }
        }

        public delegate void OpenTimeSpanDelegate(TimeSpan timeout);
        public OpenTimeSpanDelegate OpenTimeSpan;

        void ICommunicationObject.Open(TimeSpan timeout)
        {

            if (OpenTimeSpan != null)
            {
                OpenTimeSpan(timeout);
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).Open(timeout);
            }
        }

        public delegate IAsyncResult BeginOpenAsyncCallbackObjectDelegate(AsyncCallback callback, Object state);
        public BeginOpenAsyncCallbackObjectDelegate BeginOpenAsyncCallbackObject;

        IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, Object state)
        {


            if (BeginOpenAsyncCallbackObject != null)
            {
                return BeginOpenAsyncCallbackObject(callback, state);
            } else if (_inner != null)
            {
                return ((ICommunicationObject)_inner).BeginOpen(callback, state);
            }

            return default(IAsyncResult);
        }

        public delegate IAsyncResult BeginOpenTimeSpanAsyncCallbackObjectDelegate(TimeSpan timeout, AsyncCallback callback, Object state);
        public BeginOpenTimeSpanAsyncCallbackObjectDelegate BeginOpenTimeSpanAsyncCallbackObject;

        IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, Object state)
        {


            if (BeginOpenTimeSpanAsyncCallbackObject != null)
            {
                return BeginOpenTimeSpanAsyncCallbackObject(timeout, callback, state);
            } else if (_inner != null)
            {
                return ((ICommunicationObject)_inner).BeginOpen(timeout, callback, state);
            }

            return default(IAsyncResult);
        }

        public delegate void EndOpenIAsyncResultDelegate(IAsyncResult result);
        public EndOpenIAsyncResultDelegate EndOpenIAsyncResult;

        void ICommunicationObject.EndOpen(IAsyncResult result)
        {

            if (EndOpenIAsyncResult != null)
            {
                EndOpenIAsyncResult(result);
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).EndOpen(result);
            }
        }

        public delegate void CloseDelegate();
        public CloseDelegate Close;

        void ICommunicationObject.Close()
        {

            if (Close != null)
            {
                Close();
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).Close();
            }
        }

        public delegate void CloseTimeSpanDelegate(TimeSpan timeout);
        public CloseTimeSpanDelegate CloseTimeSpan;

        void ICommunicationObject.Close(TimeSpan timeout)
        {

            if (CloseTimeSpan != null)
            {
                CloseTimeSpan(timeout);
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).Close(timeout);
            }
        }

        public delegate void DisposeDelegate();
        public DisposeDelegate Dispose;

        void IDisposable.Dispose()
        {

            if (Dispose != null)
            {
                Dispose();
            } else if (_inner != null)
            {
                ((IDisposable)_inner).Dispose();
            }
        }

        private bool _AllowInitializationUI;
        public Func<bool> AllowInitializationUIGet;
        public Action<bool> AllowInitializationUISetBoolean;

        bool IClientChannel.AllowInitializationUI
        {
            get
            {
                if (AllowInitializationUIGet != null)
                {
                    return AllowInitializationUIGet();
                } else if (_inner != null)
                {
                    return ((IClientChannel)_inner).AllowInitializationUI;
                }

                if (AllowInitializationUISetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AllowInitializationUI;
                }

                return default(bool);
            }

            set
            {
                if (AllowInitializationUISetBoolean != null)
                {
                    AllowInitializationUISetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IClientChannel)_inner).AllowInitializationUI = value;
                    return;
                }

                if (AllowInitializationUIGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AllowInitializationUI = value;
                }

            }
        }

        private bool _DidInteractiveInitialization;
        public Func<bool> DidInteractiveInitializationGet;
        public Action<bool> DidInteractiveInitializationSetBoolean;

        bool IClientChannel.DidInteractiveInitialization
        {
            get
            {
                if (DidInteractiveInitializationGet != null)
                {
                    return DidInteractiveInitializationGet();
                } else if (_inner != null)
                {
                    return ((IClientChannel)_inner).DidInteractiveInitialization;
                }

                if (DidInteractiveInitializationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DidInteractiveInitialization;
                }

                return default(bool);
            }

        }

        private Uri _Via;
        public Func<Uri> ViaGet;
        public Action<Uri> ViaSetUri;

        Uri IClientChannel.Via
        {
            get
            {
                if (ViaGet != null)
                {
                    return ViaGet();
                } else if (_inner != null)
                {
                    return ((IClientChannel)_inner).Via;
                }

                if (ViaSetUri == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Via;
                }

                return default(Uri);
            }

        }

        private bool _AllowOutputBatching;
        public Func<bool> AllowOutputBatchingGet;
        public Action<bool> AllowOutputBatchingSetBoolean;

        bool IContextChannel.AllowOutputBatching
        {
            get
            {
                if (AllowOutputBatchingGet != null)
                {
                    return AllowOutputBatchingGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).AllowOutputBatching;
                }

                if (AllowOutputBatchingSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AllowOutputBatching;
                }

                return default(bool);
            }

            set
            {
                if (AllowOutputBatchingSetBoolean != null)
                {
                    AllowOutputBatchingSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IContextChannel)_inner).AllowOutputBatching = value;
                    return;
                }

                if (AllowOutputBatchingGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AllowOutputBatching = value;
                }

            }
        }

        private IInputSession _InputSession;
        public Func<IInputSession> InputSessionGet;
        public Action<IInputSession> InputSessionSetIInputSession;

        IInputSession IContextChannel.InputSession
        {
            get
            {
                if (InputSessionGet != null)
                {
                    return InputSessionGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).InputSession;
                }

                if (InputSessionSetIInputSession == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InputSession;
                }

                return default(IInputSession);
            }

        }

        private EndpointAddress _LocalAddress;
        public Func<EndpointAddress> LocalAddressGet;
        public Action<EndpointAddress> LocalAddressSetEndpointAddress;

        EndpointAddress IContextChannel.LocalAddress
        {
            get
            {
                if (LocalAddressGet != null)
                {
                    return LocalAddressGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).LocalAddress;
                }

                if (LocalAddressSetEndpointAddress == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LocalAddress;
                }

                return default(EndpointAddress);
            }

        }

        private TimeSpan _OperationTimeout;
        public Func<TimeSpan> OperationTimeoutGet;
        public Action<TimeSpan> OperationTimeoutSetTimeSpan;

        TimeSpan IContextChannel.OperationTimeout
        {
            get
            {
                if (OperationTimeoutGet != null)
                {
                    return OperationTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).OperationTimeout;
                }

                if (OperationTimeoutSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OperationTimeout;
                }

                return default(TimeSpan);
            }

            set
            {
                if (OperationTimeoutSetTimeSpan != null)
                {
                    OperationTimeoutSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IContextChannel)_inner).OperationTimeout = value;
                    return;
                }

                if (OperationTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _OperationTimeout = value;
                }

            }
        }

        private IOutputSession _OutputSession;
        public Func<IOutputSession> OutputSessionGet;
        public Action<IOutputSession> OutputSessionSetIOutputSession;

        IOutputSession IContextChannel.OutputSession
        {
            get
            {
                if (OutputSessionGet != null)
                {
                    return OutputSessionGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).OutputSession;
                }

                if (OutputSessionSetIOutputSession == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OutputSession;
                }

                return default(IOutputSession);
            }

        }

        private EndpointAddress _RemoteAddress;
        public Func<EndpointAddress> RemoteAddressGet;
        public Action<EndpointAddress> RemoteAddressSetEndpointAddress;

        EndpointAddress IContextChannel.RemoteAddress
        {
            get
            {
                if (RemoteAddressGet != null)
                {
                    return RemoteAddressGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).RemoteAddress;
                }

                if (RemoteAddressSetEndpointAddress == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RemoteAddress;
                }

                return default(EndpointAddress);
            }

        }

        private string _SessionId;
        public Func<string> SessionIdGet;
        public Action<string> SessionIdSetString;

        string IContextChannel.SessionId
        {
            get
            {
                if (SessionIdGet != null)
                {
                    return SessionIdGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).SessionId;
                }

                if (SessionIdSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SessionId;
                }

                return default(string);
            }

        }

        private CommunicationState _State;
        public Func<CommunicationState> StateGet;
        public Action<CommunicationState> StateSetCommunicationState;

        CommunicationState ICommunicationObject.State
        {
            get
            {
                if (StateGet != null)
                {
                    return StateGet();
                } else if (_inner != null)
                {
                    return ((ICommunicationObject)_inner).State;
                }

                if (StateSetCommunicationState == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _State;
                }

                return default(CommunicationState);
            }

        }

        private IExtensionCollection<IContextChannel> _Extensions;
        public Func<IExtensionCollection<IContextChannel>> ExtensionsGet;
        public Action<IExtensionCollection<IContextChannel>> ExtensionsSetIExtensionCollectionOfIContextChannel;

        IExtensionCollection<IContextChannel> IExtensibleObject<IContextChannel>.Extensions
        {
            get
            {
                if (ExtensionsGet != null)
                {
                    return ExtensionsGet();
                } else if (_inner != null)
                {
                    return ((IExtensibleObject<IContextChannel>)_inner).Extensions;
                }

                if (ExtensionsSetIExtensionCollectionOfIContextChannel == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Extensions;
                }

                return default(IExtensionCollection<IContextChannel>);
            }

        }

        public event EventHandler<UnknownMessageReceivedEventArgs> UnknownMessageReceived;
        public void OnUnknownMessageReceived(UnknownMessageReceivedEventArgs args)
        {
            if (UnknownMessageReceived != null)
            {
                UnknownMessageReceived(this, args);
            }
        }

        public event EventHandler Closed;
        public void OnClosed(EventArgs args)
        {
            if (Closed != null)
            {
                Closed(this, args);
            }
        }

        public event EventHandler Closing;
        public void OnClosing(EventArgs args)
        {
            if (Closing != null)
            {
                Closing(this, args);
            }
        }

        public event EventHandler Faulted;
        public void OnFaulted(EventArgs args)
        {
            if (Faulted != null)
            {
                Faulted(this, args);
            }
        }

        public event EventHandler Opened;
        public void OnOpened(EventArgs args)
        {
            if (Opened != null)
            {
                Opened(this, args);
            }
        }

        public event EventHandler Opening;
        public void OnOpening(EventArgs args)
        {
            if (Opening != null)
            {
                Opening(this, args);
            }
        }

    }
}