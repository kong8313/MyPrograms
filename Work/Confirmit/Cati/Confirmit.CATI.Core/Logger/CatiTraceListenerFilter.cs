using System.Linq;
using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Common.WcfTools.ErrorContextHandler;

namespace Confirmit.CATI.Core.Logger
{
    /// <summary>
    /// Helper class for internal filtering of <see cref="CatiTraceListener"/> messages.
    /// </summary>
    public class CatiTraceListenerFilter
    {
        /// <summary>
        /// Trace listener excludes messages that contains these strings.
        /// </summary>
        private static readonly string[][] ExcludeMessages =
            {
                // Exclude WCF exceptions when invalid username or password is used to access web service.

                #region Whole message example

                // <TraceRecord xmlns="http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord" Severity="Error">
                // <TraceIdentifier>http://msdn.microsoft.com/nb-NO/library/System.ServiceModel.Diagnostics.ThrowingException.aspx</TraceIdentifier>
                // <Description>Throwing an exception.</Description><AppDomain>Confirmit.CATI.Backend.exe</AppDomain>
                // <Exception><ExceptionType>System.ServiceModel.Security.MessageSecurityException, System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</ExceptionType>
                // <Message>Message security verification failed.</Message>
                // <StackTrace>   
                //   at System.ServiceModel.Security.TransportSecurityProtocol.VerifyIncomingMessage(Message&amp;amp; message, TimeSpan timeout)
                //   at System.ServiceModel.Security.SecurityProtocol.VerifyIncomingMessage(Message&amp;amp; message, TimeSpan timeout, SecurityProtocolCorrelationState[] correlationStates)
                //   at System.ServiceModel.Channels.SecurityChannelListener`1.ServerSecurityChannel`1.VerifyIncomingMessage(Message&amp;amp; message, TimeSpan timeout, SecurityProtocolCorrelationState[] correlationState)
                //   at System.ServiceModel.Channels.SecurityChannelListener`1.SecurityReplyChannel.ProcessReceivedRequest(RequestContext requestContext, TimeSpan timeout)
                //   at System.ServiceModel.Channels.SecurityChannelListener`1.ReceiveRequestAndVerifySecurityAsyncResult.ProcessInnerItem(RequestContext innerItem, TimeSpan timeout)
                //   at System.ServiceModel.Channels.SecurityChannelListener`1.ReceiveItemAndVerifySecurityAsyncResult`2.OnInnerReceiveDone()
                //   at System.ServiceModel.Channels.SecurityChannelListener`1.ReceiveItemAndVerifySecurityAsyncResult`2.InnerTryReceiveCompletedCallback(IAsyncResult result)
                //   at System.ServiceModel.Diagnostics.Utility.AsyncThunk.UnhandledExceptionFrame(IAsyncResult result)
                //   at System.ServiceModel.AsyncResult.Complete(Boolean completedSynchronously)
                //   at System.ServiceModel.Channels.InputQueue`1.AsyncQueueReader.Set(Item item)
                //   at System.ServiceModel.Channels.InputQueue`1.EnqueueAndDispatch(Item item, Boolean canDispatchOnThisThread)
                //   at System.ServiceModel.Channels.InputQueue`1.EnqueueAndDispatch(T item, ItemDequeuedCallback dequeuedCallback, Boolean canDispatchOnThisThread)
                //   at System.ServiceModel.Channels.InputQueueChannel`1.EnqueueAndDispatch(TDisposable item, ItemDequeuedCallback dequeuedCallback, Boolean canDispatchOnThisThread)
                //   at System.ServiceModel.Channels.SingletonChannelAcceptor`3.Enqueue(QueueItemType item, ItemDequeuedCallback dequeuedCallback, Boolean canDispatchOnThisThread)
                //   at System.ServiceModel.Channels.SingletonChannelAcceptor`3.Enqueue(QueueItemType item, ItemDequeuedCallback dequeuedCallback)
                //   at System.ServiceModel.Channels.HttpChannelListener.HttpContextReceived(HttpRequestContext context, ItemDequeuedCallback callback)
                //   at System.ServiceModel.Channels.SharedHttpTransportManager.OnGetContextCore(IAsyncResult result)
                //   at System.ServiceModel.Diagnostics.Utility.AsyncThunk.UnhandledExceptionFrame(IAsyncResult result)
                //   at System.Net.LazyAsyncResult.Complete(IntPtr userToken)
                //   at System.Net.LazyAsyncResult.ProtectedInvokeCallback(Object result, IntPtr userToken)
                //   at System.Net.ListenerAsyncResult.WaitCallback(UInt32 errorCode, UInt32 numBytes, NativeOverlapped* nativeOverlapped)
                //   at System.Threading._IOCompletionCallback.PerformIOCompletionCallback(UInt32 errorCode, UInt32 numBytes, NativeOverlapped* pOVERLAP)
                // </StackTrace>
                // <ExceptionString>System.ServiceModel.Security.MessageSecurityException: Message security verification failed. ---&amp;gt; System.ServiceModel.FaultException: Invalid user name or password.
                //   at Confirmit.CATI.Backend.WcfServices.External.CustomUserNameValidator.Validate(String userName, String password) in c:\Projects\Units\Confirmit.CATI.Backend\WcfServices\External\CustomUserNameValidator.cs:line 51
                //   at System.IdentityModel.Selectors.CustomUserNameSecurityTokenAuthenticator.ValidateUserNamePasswordCore(String userName, String password)
                //   at System.IdentityModel.Selectors.SecurityTokenAuthenticator.ValidateToken(SecurityToken token)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.ReadToken(XmlReader reader, SecurityTokenResolver tokenResolver, IList`1 allowedTokenAuthenticators, SecurityTokenAuthenticator&amp;amp; usedTokenAuthenticator)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.ReadToken(XmlDictionaryReader reader, Int32 position, Byte[] decryptedBuffer, SecurityToken encryptionToken, String idInEncryptedForm, TimeSpan timeout)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.ExecuteFullPass(XmlDictionaryReader reader)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.Process(TimeSpan timeout)
                //   at System.ServiceModel.Security.TransportSecurityProtocol.VerifyIncomingMessageCore(Message&amp;amp; message, TimeSpan timeout)
                //   at System.ServiceModel.Security.TransportSecurityProtocol.VerifyIncomingMessage(Message&amp;amp; message, TimeSpan timeout)
                //   --- End of inner exception stack trace ---
                // </ExceptionString><InnerException><ExceptionType>System.ServiceModel.FaultException, System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</ExceptionType>
                // <Message>Invalid user name or password.</Message>
                // <StackTrace>   at Confirmit.CATI.Backend.WcfServices.External.CustomUserNameValidator.Validate(String userName, String password) in c:\Projects\Units\Confirmit.CATI.Backend\WcfServices\External\CustomUserNameValidator.cs:line 51
                //   at System.IdentityModel.Selectors.CustomUserNameSecurityTokenAuthenticator.ValidateUserNamePasswordCore(String userName, String password)
                //   at System.IdentityModel.Selectors.SecurityTokenAuthenticator.ValidateToken(SecurityToken token)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.ReadToken(XmlReader reader, SecurityTokenResolver tokenResolver, IList`1 allowedTokenAuthenticators, SecurityTokenAuthenticator&amp;amp; usedTokenAuthenticator)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.ReadToken(XmlDictionaryReader reader, Int32 position, Byte[] decryptedBuffer, SecurityToken encryptionToken, String idInEncryptedForm, TimeSpan timeout)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.ExecuteFullPass(XmlDictionaryReader reader)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.Process(TimeSpan timeout)
                //   at System.ServiceModel.Security.TransportSecurityProtocol.VerifyIncomingMessageCore(Message&amp;amp; message, TimeSpan timeout)
                //   at System.ServiceModel.Security.TransportSecurityProtocol.VerifyIncomingMessage(Message&amp;amp; message, TimeSpan timeout)</StackTrace><ExceptionString>System.ServiceModel.FaultException: Invalid user name or password.
                //   at Confirmit.CATI.Backend.WcfServices.External.CustomUserNameValidator.Validate(String userName, String password) in c:\Projects\Units\Confirmit.CATI.Backend\WcfServices\External\CustomUserNameValidator.cs:line 51
                //   at System.IdentityModel.Selectors.CustomUserNameSecurityTokenAuthenticator.ValidateUserNamePasswordCore(String userName, String password)
                //   at System.IdentityModel.Selectors.SecurityTokenAuthenticator.ValidateToken(SecurityToken token)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.ReadToken(XmlReader reader, SecurityTokenResolver tokenResolver, IList`1 allowedTokenAuthenticators, SecurityTokenAuthenticator&amp;amp; usedTokenAuthenticator)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.ReadToken(XmlDictionaryReader reader, Int32 position, Byte[] decryptedBuffer, SecurityToken encryptionToken, String idInEncryptedForm, TimeSpan timeout)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.ExecuteFullPass(XmlDictionaryReader reader)
                //   at System.ServiceModel.Security.ReceiveSecurityHeader.Process(TimeSpan timeout)
                //   at System.ServiceModel.Security.TransportSecurityProtocol.VerifyIncomingMessageCore(Message&amp;amp; message, TimeSpan timeout)
                //   at System.ServiceModel.Security.TransportSecurityProtocol.VerifyIncomingMessage(Message&amp;amp; message, TimeSpan timeout)</ExceptionString></InnerException></Exception></TraceRecord>

                #endregion
                new[]
                {
                    "<Exception><ExceptionType>System.ServiceModel.Security.MessageSecurityException",
                    "Confirmit.CATI.Backend.WcfServices.External.CustomUserNameValidator.Validate"
                },

                #region Whole message example

                // <TraceRecord xmlns="http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord" Severity="Warning">
                // <TraceIdentifier>http://msdn.microsoft.com/nb-NO/library/System.ServiceModel.Security.SecurityBindingVerifyIncomingMessageFailure.aspx</TraceIdentifier>
                // <Description>The security protocol cannot verify the incoming message.</Description>
                // <AppDomain>Confirmit.CATI.Backend.exe</AppDomain>
                // <ExtendedData xmlns="http://schemas.microsoft.com/2006/08/ServiceModel/SecurityProtocolTraceRecord">
                // <SecurityProtocol>System.ServiceModel.Security.TransportSecurityProtocol</SecurityProtocol>
                // <Action>http://www.confirmit.com/ConsoleService/04/24/2009/ConsoleService/Login</Action>
                // <To>urn://localhost/MultimodeInstance2</To>
                // <EndpointReference xmlns="http://www.w3.org/2005/08/addressing">
                // <Address>http://www.w3.org/2005/08/addressing/anonymous</Address></EndpointReference>
                // <MessageId>urn:uuid:75c43da0-ee6a-4fe5-90f9-b5d48ad111fb</MessageId></ExtendedData></TraceRecord>

                #endregion
                new[]
                {
                    "/library/System.ServiceModel.Security.SecurityBindingVerifyIncomingMessageFailure.aspx</TraceIdentifier>",
                    "/Login</Action>"
                },

                // Exclude WCF error traces for the following exceptions.
                #region Whole message example

                // <TraceRecord xmlns="http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord" Severity="Error">
                // <TraceIdentifier>http://msdn.microsoft.com/en-US/library/System.ServiceModel.Diagnostics.TraceHandledException.aspx</TraceIdentifier>
                // <Description>Handling an exception.</Description><AppDomain>Confirmit.CATI.Backend.exe</AppDomain>
                // <Exception><ExceptionType>Confirmit.CATI.Common.Exceptions.PredictiveSurveyWithoutDialerException, Confirmit.CATI.Common, Version=15.0.694.0, Culture=neutral, PublicKeyToken=8134450e5a05c0c1</ExceptionType>
                // <Message>Can not work with predictive dialing surveys without dialer in non manual mode.</Message>
                // <StackTrace>   
                //  at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.ConsoleService.StartInterview(String surveyId, Int32 interviewId) in c:\TFSBuild\Confirmit\CATI_15.0_REL\Sources\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\ConsoleService.cs:line 818
                //  at SyncInvokeStartInterview(Object , Object[] , Object[] )
                //  at System.ServiceModel.Dispatcher.SyncMethodInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp;amp; outputs)
                //  at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage4(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)
                // </StackTrace>
                // <ExceptionString>Confirmit.CATI.Common.Exceptions.PredictiveSurveyWithoutDialerException: Can not work with predictive dialing surveys without dialer in non manual mode.
                //  at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.ConsoleService.StartInterview(String surveyId, Int32 interviewId) in c:\TFSBuild\Confirmit\CATI_15.0_REL\Sources\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\ConsoleService.cs:line 818
                //  at SyncInvokeStartInterview(Object , Object[] , Object[] )
                //  at System.ServiceModel.Dispatcher.SyncMethodInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp;amp; outputs)
                //  at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage4(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)</ExceptionString></Exception>
                // </TraceRecord>

                #endregion
                //new[]
                //{
                //    "<Exception><ExceptionType>" + typeof (PredictiveSurveyWithoutDialerException).FullName
                //},

                #region Whole message example

                // <TraceRecord xmlns="http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord" Severity="Error"><TraceIdentifier>http://msdn.microsoft.com/nb-NO/library/System.ServiceModel.Diagnostics.TraceHandledException.aspx</TraceIdentifier><Description>Handling an exception.</Description><AppDomain>Confirmit.CATI.Backend.exe</AppDomain><Exception><ExceptionType>Confirmit.CATI.Common.Exceptions.InvalidInterviewerCredentialsException, Confirmit.CATI.Common, Version=15.0.0.0, Culture=neutral, PublicKeyToken=8134450e5a05c0c1</ExceptionType><Message>Invalid user name or password.</Message><StackTrace>   at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.LoginPasswordValidationBehaviorAttribute.ValidateLoginPassword(String login, String password) in C:\Projects\Units\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\LoginPasswordValidationBehaviorAttribute.cs:line 183
                //   at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.LoginPasswordValidationBehaviorAttribute.System.ServiceModel.Dispatcher.IOperationInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp;amp; outputs) in C:\Projects\Units\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\LoginPasswordValidationBehaviorAttribute.cs:line 118
                //   at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp;amp; rpc)
                //   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp;amp; rpc)
                //   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage4(MessageRpc&amp;amp; rpc)
                //   at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)</StackTrace><ExceptionString>Confirmit.CATI.Common.Exceptions.InvalidInterviewerCredentialsException: Invalid user name or password.
                //   at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.LoginPasswordValidationBehaviorAttribute.ValidateLoginPassword(String login, String password) in C:\Projects\Units\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\LoginPasswordValidationBehaviorAttribute.cs:line 183
                //   at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.LoginPasswordValidationBehaviorAttribute.System.ServiceModel.Dispatcher.IOperationInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp;amp; outputs) in C:\Projects\Units\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\LoginPasswordValidationBehaviorAttribute.cs:line 118
                //   at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp;amp; rpc)
                //   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp;amp; rpc)
                //   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage4(MessageRpc&amp;amp; rpc)
                //   at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)</ExceptionString></Exception></TraceRecord>

                #endregion
                //new[]
                //{
                //    "<Exception><ExceptionType>" + typeof (InvalidInterviewerCredentialsException).FullName
                //},

                #region Whole message example

                // <TraceRecord xmlns="http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord" Severity="Error"><TraceIdentifier>http://msdn.microsoft.com/en-US/library/System.ServiceModel.Diagnostics.TraceHandledException.aspx</TraceIdentifier><Description>Handling an exception.</Description><AppDomain>Confirmit.CATI.Backend.exe</AppDomain><Exception><ExceptionType>Confirmit.CATI.Common.Exceptions.StateServiceSessionExpiredException, Confirmit.CATI.Common, Version=15.0.0.0, Culture=neutral, PublicKeyToken=8134450e5a05c0c1</ExceptionType><Message>Exception of type 'Confirmit.CATI.Common.Exceptions.StateServiceSessionExpiredException' was thrown.</Message><StackTrace>   at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.InterviewerValidationBehaviorAttribute.CheckSessionExpiration(BvTasksEntity task) in C:\dev\FoundstoneCati\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\InterviewerValidationBehaviorAttribute.cs:line 123
                // at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.InterviewerValidationBehaviorAttribute.System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(Message&amp;amp; request, IClientChannel channel, InstanceContext instanceContext) in C:\dev\FoundstoneCati\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\InterviewerValidationBehaviorAttribute.cs:line 78
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.AfterReceiveRequestCore(MessageRpc&amp;amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage2(MessageRpc&amp;amp; rpc)
                // at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)</StackTrace><ExceptionString>Confirmit.CATI.Common.Exceptions.StateServiceSessionExpiredException: Exception of type 'Confirmit.CATI.Common.Exceptions.StateServiceSessionExpiredException' was thrown.
                // at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.InterviewerValidationBehaviorAttribute.CheckSessionExpiration(BvTasksEntity task) in C:\dev\FoundstoneCati\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\InterviewerValidationBehaviorAttribute.cs:line 123
                // at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.InterviewerValidationBehaviorAttribute.System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(Message&amp;amp; request, IClientChannel channel, InstanceContext instanceContext) in C:\dev\FoundstoneCati\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\InterviewerValidationBehaviorAttribute.cs:line 78
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.AfterReceiveRequestCore(MessageRpc&amp;amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage2(MessageRpc&amp;amp; rpc)
                // at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)</ExceptionString></Exception></TraceRecord>

                #endregion
                //new[]
                //{
                //    "<Exception><ExceptionType>" + typeof (StateServiceSessionExpiredException).FullName
                //},

                #region Whole message example

                // <TraceRecord xmlns="http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord" Severity="Error">
                // <TraceIdentifier>http://msdn.microsoft.com/nb-NO/library/System.ServiceModel.Diagnostics.TraceHandledException.aspx</TraceIdentifier>
                // <Description>Handling an exception.</Description><AppDomain>Confirmit.CATI.Backend.exe</AppDomain>
                // <Exception><ExceptionType>Confirmit.CATI.Common.Exceptions.UserMessageException, Confirmit.CATI.Common, Version=15.0.0.0, Culture=neutral, PublicKeyToken=8134450e5a05c0c1</ExceptionType>
                // <Message>Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 7.</Message>
                // <StackTrace>   
                //  at Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter.BvClr_CallQueue_ActivateCallsAdapter.ExecuteNonQuery(Nullable`1 surveySid, Nullable`1 classId, Nullable`1 mode, Nullable`1 batchId, Nullable`1 priority, Nullable`1 roleId, Nullable`1 personSid, Nullable`1 shiftTypeId, Nullable`1 timeToCall, String sql, Nullable`1 commandExecutionTimeout, Int32&amp;amp; returnValue) in C:\Projects\Rel\Confirmit.CATI.Core\DAL\Generated\Adapter\Procedure\BvClr_CallQueue_ActivateCallsAdapter.cs:line 328
                //  at Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter.BvClr_CallQueue_ActivateCallsAdapter.ExecuteNonQuery(Nullable`1 surveySid, Nullable`1 classId, Nullable`1 mode, Nullable`1 batchId, Nullable`1 priority, Nullable`1 roleId, Nullable`1 personSid, Nullable`1 shiftTypeId, Nullable`1 timeToCall, String sql, Int32&amp;amp; returnValue) in C:\Projects\Rel\Confirmit.CATI.Core\DAL\Generated\Adapter\Procedure\BvClr_CallQueue_ActivateCallsAdapter.cs:line 259
                //  at Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter.BvClr_CallQueue_ActivateCallsAdapter.ExecuteNonQuery(Nullable`1 surveySid, Nullable`1 classId, Nullable`1 mode, Nullable`1 batchId, Nullable`1 priority, Nullable`1 roleId, Nullable`1 personSid, Nullable`1 shiftTypeId, Nullable`1 timeToCall, String sql) in C:\Projects\Rel\Confirmit.CATI.Core\DAL\Generated\Adapter\Procedure\BvClr_CallQueue_ActivateCallsAdapter.cs:line 232
                //  at Confirmit.CATI.Core.Services.CallQueueService.StartAsyncActivateCalls(Int32 surveyId, Nullable`1 filterId, Int32[] interviewIds, Int32 priority, FilterGenerateMode filterMode, Int32 personOrGroupId, Int32 shiftTypeId, Nullable`1 timeToCall) in C:\Projects\Rel\Confirmit.CATI.Core\Services\CallQueueService.cs:line 280
                //  at Confirmit.CATI.Backend.WcfServices.Internal.SupervisorService.SupervisorService.StartAsyncActivateCalls(Int32 surveyId, Nullable`1 filterId, Int32[] interviewIds, Int32 priority, FilterGenerateMode filterMode, Int32 personOrGroupId, Int32 shiftTypeId, Nullable`1 timeToCall) in C:\Projects\Rel\Confirmit.CATI.Backend\WcfServices\Internal\SupervisorService\SupervisorService.cs:line 244
                //  at SyncInvokeActivateCalls(Object , Object[] , Object[] )
                //  at System.ServiceModel.Dispatcher.SyncMethodInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp;amp; outputs)
                //  at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage4(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)</StackTrace><ExceptionString>Confirmit.CATI.Common.Exceptions.UserMessageException: Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 7. ---&amp;gt; System.Data.SqlClient.SqlException: Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 7.
                //  at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection)
                //  at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj)
                //  at System.Data.SqlClient.TdsParser.Run(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj)
                //  at System.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString)
                //  at System.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean async)
                //  at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method, DbAsyncResult result)
                //  at System.Data.SqlClient.SqlCommand.InternalExecuteNonQuery(DbAsyncResult result, String methodName, Boolean sendToPipe)
                //  at System.Data.SqlClient.SqlCommand.ExecuteNonQuery()
                //  at Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter.BvClr_CallQueue_ActivateCallsAdapter.ExecuteNonQuery(Nullable`1 surveySid, Nullable`1 classId, Nullable`1 mode, Nullable`1 batchId, Nullable`1 priority, Nullable`1 roleId, Nullable`1 personSid, Nullable`1 shiftTypeId, Nullable`1 timeToCall, String sql, Nullable`1 commandExecutionTimeout, Int32&amp;amp; returnValue) in C:\Projects\Rel\Confirmit.CATI.Core\DAL\Generated\Adapter\Procedure\BvClr_CallQueue_ActivateCallsAdapter.cs:line 319
                //  --- End of inner exception stack trace ---
                //  at Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter.BvClr_CallQueue_ActivateCallsAdapter.ExecuteNonQuery(Nullable`1 surveySid, Nullable`1 classId, Nullable`1 mode, Nullable`1 batchId, Nullable`1 priority, Nullable`1 roleId, Nullable`1 personSid, Nullable`1 shiftTypeId, Nullable`1 timeToCall, String sql, Nullable`1 commandExecutionTimeout, Int32&amp;amp; returnValue) in C:\Projects\Rel\Confirmit.CATI.Core\DAL\Generated\Adapter\Procedure\BvClr_CallQueue_ActivateCallsAdapter.cs:line 328
                //  at Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter.BvClr_CallQueue_ActivateCallsAdapter.ExecuteNonQuery(Nullable`1 surveySid, Nullable`1 classId, Nullable`1 mode, Nullable`1 batchId, Nullable`1 priority, Nullable`1 roleId, Nullable`1 personSid, Nullable`1 shiftTypeId, Nullable`1 timeToCall, String sql, Int32&amp;amp; returnValue) in C:\Projects\Rel\Confirmit.CATI.Core\DAL\Generated\Adapter\Procedure\BvClr_CallQueue_ActivateCallsAdapter.cs:line 259
                //  at Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter.BvClr_CallQueue_ActivateCallsAdapter.ExecuteNonQuery(Nullable`1 surveySid, Nullable`1 classId, Nullable`1 mode, Nullable`1 batchId, Nullable`1 priority, Nullable`1 roleId, Nullable`1 personSid, Nullable`1 shiftTypeId, Nullable`1 timeToCall, String sql) in C:\Projects\Rel\Confirmit.CATI.Core\DAL\Generated\Adapter\Procedure\BvClr_CallQueue_ActivateCallsAdapter.cs:line 232
                //  at Confirmit.CATI.Core.Services.CallQueueService.StartAsyncActivateCalls(Int32 surveyId, Nullable`1 filterId, Int32[] interviewIds, Int32 priority, FilterGenerateMode filterMode, Int32 personOrGroupId, Int32 shiftTypeId, Nullable`1 timeToCall) in C:\Projects\Rel\Confirmit.CATI.Core\Services\CallQueueService.cs:line 280
                //  at Confirmit.CATI.Backend.WcfServices.Internal.SupervisorService.SupervisorService.StartAsyncActivateCalls(Int32 surveyId, Nullable`1 filterId, Int32[] interviewIds, Int32 priority, FilterGenerateMode filterMode, Int32 personOrGroupId, Int32 shiftTypeId, Nullable`1 timeToCall) in C:\Projects\Rel\Confirmit.CATI.Backend\WcfServices\Internal\SupervisorService\SupervisorService.cs:line 244
                //  at SyncInvokeActivateCalls(Object , Object[] , Object[] )
                //  at System.ServiceModel.Dispatcher.SyncMethodInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp;amp; outputs)
                //  at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage4(MessageRpc&amp;amp; rpc)
                //  at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)</ExceptionString><InnerException><ExceptionType>System.Data.SqlClient.SqlException, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</ExceptionType><Message>Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 7.</Message><StackTrace>   at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection)
                //  at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj)
                //  at System.Data.SqlClient.TdsParser.Run(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj)
                //   System.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString)
                //  at System.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean async)
                //  at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method, DbAsyncResult result)
                //  at System.Data.SqlClient.SqlCommand.InternalExecuteNonQuery(DbAsyncResult result, String methodName, Boolean sendToPipe)
                //  at System.Data.SqlClient.SqlCommand.ExecuteNonQuery()
                //  at Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter.BvClr_CallQueue_ActivateCallsAdapter.ExecuteNonQuery(Nullable`1 surveySid, Nullable`1 classId, Nullable`1 mode, Nullable`1 batchId, Nullable`1 priority, Nullable`1 roleId, Nullable`1 personSid, Nullable`1 shiftTypeId, Nullable`1 timeToCall, String sql, Nullable`1 commandExecutionTimeout, Int32&amp;amp; returnValue) in C:\Projects\Rel\Confirmit.CATI.Core\DAL\Generated\Adapter\Procedure\BvClr_CallQueue_ActivateCallsAdapter.cs:line 319
                // </StackTrace>
                // <ExceptionString>System.Data.SqlClient.SqlException: Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 7.
                //  at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection)
                //  at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj)
                //  at System.Data.SqlClient.TdsParser.Run(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj)
                //  at System.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString)
                //  at System.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean async)
                //  at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method, DbAsyncResult result)
                //  at System.Data.SqlClient.SqlCommand.InternalExecuteNonQuery(DbAsyncResult result, String methodName, Boolean sendToPipe)
                //  at System.Data.SqlClient.SqlCommand.ExecuteNonQuery()
                //  at Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter.BvClr_CallQueue_ActivateCallsAdapter.ExecuteNonQuery(Nullable`1 surveySid, Nullable`1 classId, Nullable`1 mode, Nullable`1 batchId, Nullable`1 priority, Nullable`1 roleId, Nullable`1 personSid, Nullable`1 shiftTypeId, Nullable`1 timeToCall, String sql, Nullable`1 commandExecutionTimeout, Int32&amp;amp; returnValue) in C:\Projects\Rel\Confirmit.CATI.Core\DAL\Generated\Adapter\Procedure\BvClr_CallQueue_ActivateCallsAdapter.cs:line 319</ExceptionString>
                // <DataItems><Data><Key>HelpLink.ProdName</Key><Value>Microsoft SQL Server</Value></Data><Data><Key>HelpLink.ProdVer</Key><Value>10.00.2714</Value></Data><Data><Key>HelpLink.EvtSrc</Key><Value>MSSQLServer</Value></Data><Data><Key>HelpLink.EvtID</Key><Value>50000</Value></Data><Data><Key>HelpLink.BaseHelpUrl</Key><Value>http://go.microsoft.com/fwlink</Value></Data><Data><Key>HelpLink.LinkId</Key><Value>20476</Value></Data></DataItems></InnerException></Exception></TraceRecord>

                #endregion
                //new[]
                //{
                //    "<Exception><ExceptionType>" + typeof (UserMessageException).FullName
                //},

                new[]
                {
                    "<TraceIdentifier>http://msdn.microsoft.com/en-US/library/System.ServiceModel.Diagnostics.TraceHandledException.aspx</TraceIdentifier>",
                    typeof(ErrorHandlingInvoker).FullName+".Invoke"
                },

                #region Whole message example

                //http://msdn.microsoft.com/en-GB/library/System.ServiceModel.Diagnostics.TraceHandledException.aspxHandling an exception. Exception details: 
                //  System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.InterviewerNotLoggedInExceptionDetails]:
                //  Interviewer 'jrawlins' is not logged in (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.InterviewerNotLoggedInExceptionDetails).Confirmit.CATI.Backend.exeSystem.ServiceModel.FaultException`1[[Confirmit.CATI.Common.Exceptions.InterviewerNotLoggedInExceptionDetails, 
                //  Confirmit.CATI.Common, Version=19.0.10266.0, Culture=neutral, PublicKeyToken=8134450e5a05c0c1]], 
                //  System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Interviewer 'jrawlins' is not logged in  
                //   at Confirmit.CATI.Common.WcfTools.ErrorContextHandler.ErrorHandlingInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp; outputs) in w:\BuildAgent\work\ba101fd0ee3bd726\Confirmit.CATI.Common\WcfTools\ErrorContextHandler\ErrorHandlingInvoker.cs:line 58
                //   at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp; rpc)
                //   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp; rpc)
                //   at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage31(MessageRpc&amp; rpc)
                //   at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.InterviewerNotLoggedInExceptionDetails]: Interviewer 'jrawlins' is not logged in (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.InterviewerNotLoggedInExceptionDetails).
                #endregion
                new []
                {
                    "System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.InterviewerNotLoggedInExceptionDetails]:"
                },

                #region Whole message example

                // http://msdn.microsoft.com/en-US/library/System.ServiceModel.Diagnostics.TraceHandledException.aspxHandling an exception. Exception details: System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.NotSupportedOsExceptionDetails]: Wrong OS version (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.NotSupportedOsExceptionDetails).Confirmit.CATI.Backend.exeSystem.ServiceModel.FaultException`1[[Confirmit.CATI.Common.Exceptions.NotSupportedOsExceptionDetails, Confirmit.CATI.Common, Version=20.0.0.0, Culture=neutral, PublicKeyToken=8134450e5a05c0c1]], System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Wrong OS version   at Confirmit.CATI.Common.WcfTools.ErrorContextHandler.ErrorHandlingInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp; outputs) in c:\Git\Rel\Confirmit.CATI.Common\WcfTools\ErrorContextHandler\ErrorHandlingInvoker.cs:line 58
                // at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage11(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.NotSupportedOsExceptionDetails]: Wrong OS version (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.NotSupportedOsExceptionDetails).
                #endregion
                new []
                {
                    "System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.NotSupportedOsExceptionDetails]: Wrong OS version"
                }
            };

        private static readonly string[][] ErrorsToTraceAsWarningMessages =
            {
                #region Whole message example
                // <TraceRecord xmlns="http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord" Severity="Error">
                // <TraceIdentifier>http://msdn.microsoft.com/en-US/library/System.ServiceModel.Diagnostics.TraceHandledException.aspx
                // </TraceIdentifier><Description>Handling an exception.</Description><AppDomain>Confirmit.CATI.Backend.exe</AppDomain><Exception>
                // <ExceptionType>Confirmit.CATI.Common.Exceptions.InterviewerNotLoggedInException, Confirmit.CATI.Common, Version=16.0.2933.39066,
                // Culture=neutral, PublicKeyToken=8134450e5a05c0c1</ExceptionType><Message>Interviewer 'umarhayat' is not logged in</Message>
                // <StackTrace>
                // at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.InterviewerValidationBehaviorAttribute.System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(Message&amp;amp; request, IClientChannel channel, InstanceContext instanceContext) in c:\TFSBuild\Confirmit\CATI_Boomer_PRL\Sources\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\InterviewerValidationBehaviorAttribute.cs:line 81
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.AfterReceiveRequestCore(MessageRpc&amp;amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage2(MessageRpc&amp;amp; rpc)
                // at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)</StackTrace><ExceptionString>Confirmit.CATI.Common.Exceptions.InterviewerNotLoggedInException: Interviewer 'umarhayat' is not logged in
                // at Confirmit.CATI.Backend.WcfServices.External.ConsoleService.InterviewerValidationBehaviorAttribute.System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(Message&amp;amp; request, IClientChannel channel, InstanceContext instanceContext) in c:\TFSBuild\Confirmit\CATI_Boomer_PRL\Sources\Confirmit.CATI.Backend\WcfServices\External\ConsoleService\InterviewerValidationBehaviorAttribute.cs:line 81
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.AfterReceiveRequestCore(MessageRpc&amp;amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage2(MessageRpc&amp;amp; rpc)
                // at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)</ExceptionString></Exception></TraceRecord>
                // ----------
                // Server time: 3/19/2011 11:16:10 AM
                // UTC time: 3/19/2011 11:16:10 AM
                // Process: Confirmit.CATI.Backend (2424)
                // Thread: 21 from ThreadPool
                // Company: Facts International Ltd (465)
                // CATI version: 16.0.2933.39066
                #endregion
                new[]
                {
                    "<Exception><ExceptionType>Confirmit.CATI.Common.Exceptions.InterviewerNotLoggedInException",
                    "Confirmit.CATI.Common.Exceptions.InterviewerNotLoggedInException: Interviewer",
                    "' is not logged in"
                },

                #region Whole message example
                // <TraceRecord xmlns="http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord" Severity="Error"><TraceIdentifier>http://msdn.microsoft.com/en-US/library/System.ServiceModel.Diagnostics.TraceHandledException.aspx</TraceIdentifier><Description>Handling an exception.</Description><AppDomain>Confirmit.CATI.Backend.exe</AppDomain><Exception><ExceptionType>System.ServiceModel.CommunicationException, System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</ExceptionType><Message>An operation was attempted on a nonexistent network connection</Message><StackTrace>   at System.ServiceModel.Channels.HttpOutput.ListenerResponseHttpOutput.ListenerResponseOutputStream.Close()
                // at System.ServiceModel.Channels.HttpOutput.Close()
                // at System.ServiceModel.Channels.HttpRequestContext.OnReply(Message message, TimeSpan timeout)
                // at System.ServiceModel.Channels.RequestContextBase.Reply(Message message, TimeSpan timeout)
                // at System.ServiceModel.Channels.HttpRequestContext.SendResponseAndClose(HttpStatusCode statusCode, String statusDescription)
                // at System.ServiceModel.Channels.HttpChannelListener.HttpContextReceived(HttpRequestContext context, ItemDequeuedCallback callback)</StackTrace><ExceptionString>System.ServiceModel.CommunicationException: An operation was attempted on a nonexistent network connection ---&amp;gt; System.Net.HttpListenerException: An operation was attempted on a nonexistent network connection
                // at System.Net.HttpResponseStream.Dispose(Boolean disposing)
                // at System.IO.Stream.Close()
                // at System.ServiceModel.Channels.DelegatingStream.Close()
                // at System.ServiceModel.Channels.HttpOutput.ListenerResponseHttpOutput.ListenerResponseOutputStream.Close()
                // --- End of inner exception stack trace ---
                // at System.ServiceModel.Channels.HttpOutput.ListenerResponseHttpOutput.ListenerResponseOutputStream.Close()
                // at System.ServiceModel.Channels.HttpOutput.Close()
                // at System.ServiceModel.Channels.HttpRequestContext.OnReply(Message message, TimeSpan timeout)
                // at System.ServiceModel.Channels.RequestContextBase.Reply(Message message, TimeSpan timeout)
                // at System.ServiceModel.Channels.HttpRequestContext.SendResponseAndClose(HttpStatusCode statusCode, String statusDescription)
                // at System.ServiceModel.Channels.HttpChannelListener.HttpContextReceived(HttpRequestContext context, ItemDequeuedCallback callback)</ExceptionString><InnerException><ExceptionType>System.Net.HttpListenerException, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</ExceptionType><Message>An operation was attempted on a nonexistent network connection</Message><StackTrace>   at System.Net.HttpResponseStream.Dispose(Boolean disposing)
                // at System.IO.Stream.Close()
                // at System.ServiceModel.Channels.DelegatingStream.Close()
                // at System.ServiceModel.Channels.HttpOutput.ListenerResponseHttpOutput.ListenerResponseOutputStream.Close()</StackTrace><ExceptionString>System.Net.HttpListenerException: An operation was attempted on a nonexistent network connection
                // at System.Net.HttpResponseStream.Dispose(Boolean disposing)
                // at System.IO.Stream.Close()
                // at System.ServiceModel.Channels.DelegatingStream.Close()
                // at System.ServiceModel.Channels.HttpOutput.ListenerResponseHttpOutput.ListenerResponseOutputStream.Close()</ExceptionString><NativeErrorCode>4CD</NativeErrorCode></InnerException></Exception></TraceRecord>
                // ----------
                // Server time: 4/9/2011 1:45:42 PM
                // UTC time: 4/9/2011 12:45:42 PM
                // Process: Confirmit.CATI.Backend (5144)
                // Thread: 32 from ThreadPool
                // Company: Facts International Ltd (465)
                // CATI version: 16.0.3022.39851
                #endregion
                new[]
                {
                    "<Exception><ExceptionType>System.ServiceModel.CommunicationException, System.ServiceModel",
                    "<Message>An operation was attempted on a nonexistent network connection</Message>"
                },

                #region Whole message example
                // <TraceRecord xmlns="http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord" Severity="Error"><TraceIdentifier>http://msdn.microsoft.com/en-US/library/System.ServiceModel.Diagnostics.ThrowingException.aspx</TraceIdentifier><Description>Throwing an exception.</Description><AppDomain>Confirmit.CATI.Backend.exe</AppDomain><Exception><ExceptionType>System.ServiceModel.CommunicationException, System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</ExceptionType><Message>The I/O operation has been aborted because of either a thread exit or an application request</Message><StackTrace>   at System.ServiceModel.Channels.HttpRequestContext.ListenerHttpContext.ListenerContextHttpInput.ListenerContextInputStream.Read(Byte[] buffer, Int32 offset, Int32 count)
                // at System.ServiceModel.Channels.HttpInput.ReadBufferedMessage(Stream inputStream)
                // at System.ServiceModel.Channels.HttpInput.ParseIncomingMessage(Exception&amp;amp; requestException)
                // at System.ServiceModel.Channels.HttpRequestContext.CreateMessage()
                // at System.ServiceModel.Channels.HttpChannelListener.HttpContextReceived(HttpRequestContext context, ItemDequeuedCallback callback)
                // at System.ServiceModel.Channels.SharedHttpTransportManager.OnGetContextCore(IAsyncResult result)
                // at System.ServiceModel.Diagnostics.Utility.AsyncThunk.UnhandledExceptionFrame(IAsyncResult result)
                // at System.Net.LazyAsyncResult.Complete(IntPtr userToken)
                // at System.Net.LazyAsyncResult.ProtectedInvokeCallback(Object result, IntPtr userToken)
                // at System.Net.ListenerAsyncResult.WaitCallback(UInt32 errorCode, UInt32 numBytes, NativeOverlapped* nativeOverlapped)
                // at System.Threading._IOCompletionCallback.PerformIOCompletionCallback(UInt32 errorCode, UInt32 numBytes, NativeOverlapped* pOVERLAP)
                // </StackTrace><ExceptionString>System.ServiceModel.CommunicationException: The I/O operation has been aborted because of either a thread exit or an application request ---&amp;gt; System.Net.HttpListenerException: The I/O operation has been aborted because of either a thread exit or an application request
                // at System.Net.HttpRequestStream.Read(Byte[] buffer, Int32 offset, Int32 size)
                // at System.ServiceModel.Channels.DelegatingStream.Read(Byte[] buffer, Int32 offset, Int32 count)
                // at System.ServiceModel.Channels.DetectEofStream.Read(Byte[] buffer, Int32 offset, Int32 count)
                // at System.ServiceModel.Channels.HttpRequestContext.ListenerHttpContext.ListenerContextHttpInput.ListenerContextInputStream.Read(Byte[] buffer, Int32 offset, Int32 count)
                // --- End of inner exception stack trace ---</ExceptionString><InnerException><ExceptionType>System.Net.HttpListenerException, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</ExceptionType><Message>The I/O operation has been aborted because of either a thread exit or an application request</Message><StackTrace>   at System.Net.HttpRequestStream.Read(Byte[] buffer, Int32 offset, Int32 size)
                // at System.ServiceModel.Channels.DelegatingStream.Read(Byte[] buffer, Int32 offset, Int32 count)
                // at System.ServiceModel.Channels.DetectEofStream.Read(Byte[] buffer, Int32 offset, Int32 count)
                // at System.ServiceModel.Channels.HttpRequestContext.ListenerHttpContext.ListenerContextHttpInput.ListenerContextInputStream.Read(Byte[] buffer, Int32 offset, Int32 count)</StackTrace><ExceptionString>System.Net.HttpListenerException: The I/O operation has been aborted because of either a thread exit or an application request
                // at System.Net.HttpRequestStream.Read(Byte[] buffer, Int32 offset, Int32 size)
                // at System.ServiceModel.Channels.DelegatingStream.Read(Byte[] buffer, Int32 offset, Int32 count)
                // at System.ServiceModel.Channels.DetectEofStream.Read(Byte[] buffer, Int32 offset, Int32 count)
                // at System.ServiceModel.Channels.HttpRequestContext.ListenerHttpContext.ListenerContextHttpInput.ListenerContextInputStream.Read(Byte[] buffer, Int32 offset, Int32 count)</ExceptionString><NativeErrorCode>3E3</NativeErrorCode></InnerException></Exception></TraceRecord>
                // ----------
                // Server time: 4/9/2011 1:45:42 PM
                // UTC time: 4/9/2011 12:45:42 PM
                // Process: Confirmit.CATI.Backend (5144)
                // Thread: 32 from ThreadPool
                // Company: Facts International Ltd (465)
                // CATI version: 16.0.3022.39851
                #endregion
                new[]
                {
                    "<Exception><ExceptionType>System.ServiceModel.CommunicationException, System.ServiceModel",
                    "<Message>The I/O operation has been aborted because of either a thread exit or an application request</Message>"
                },

                #region Whole message example
                //System.Web.HttpException: The client disconnected. ---> System.Web.UI.ViewStateException: Invalid viewstate. 
                //    Client IP: 93.157.223.22
                //    Port: 1802
                //    User-Agent: Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)
                //    ViewState: /wEPDwUKLTYwOTg2NzMwMg9kFgJmD2QWAmYPZBYCAgUPZBYCAgEPZBYCZg9kFgJmD2QWDGYPFgIeB1Zpc2libGVoZAIBD2QWAmYPZBYCAgEPDxYCHgRUZXh0BUBTdXJ2ZXkgOTI2IFJCUyBCcmFuY2ggMjAxMiBQaWxvdCAtIExJVkUgKHA3NzEwOTg4MjYpIGluZm9ybWF0aW9uZGQCAg9kFgICAQ9kFgJmDw8WAh4DaWdEAgMUKwABFglkD2YUKwAGFgIeA1RfRgUHR2VuZXJhbGRkZGQWAh8AaA8CARQrAAYWAh8DBQdTdW1tYXJ5ZGRkZBYCHwBoDwICFCsABhYCHwMFC0Fzc2lnbm1lbnRzZGRkZBYCHwBoDwIDPCsABgEAFgIfAwUGUXVvdGFzDwIEFCsABhYCHwMFEkludGVydmlld2VyIFNlYXJjaGRkZGQWAh8AaA8CBRQrAAYWAh8DBRVTY2hlZHVsaW5nIFBhcmFtZXRlcnNkZGRkFgIfAGgPAgYUKwAGFgIfAwUHRmlsdGVyc2RkZGQWAh8AaA8CBxQrAAYWBB8DBQ9EaWFsZXIgU2V0dGluZ3MeA1RfS2dkZGRkFgIfAGgWEGYPFgIfAGgWAgIBD2QWBgIDDxBkZBYAZAIGD2QWAgIBD2QWAgIBDxBkZBYAZAILD2QWAgIBDxBkZBYAZAIBDxYCHwBoFgICAQ9kFgJmDw8WBh4TSGlkZU9wdGlvbnNNZW51SXRlbWgeE1NpbXBsaWZpZWRQYWdlck1vZGVoHg1Tb3J0SW5kaWNhdG9yCymhAU...
                //   --- End of inner exception stack trace ---
                //   at System.Web.UI.ViewStateException.ThrowError(Exception inner, String persistedState, String errorPageMessage, Boolean macValidationError)
                //   at System.Web.UI.ObjectStateFormatter.Deserialize(String inputString)
                //   at System.Web.UI.Util.DeserializeWithAssert(IStateFormatter formatter, String serializedState)
                //   at System.Web.UI.HiddenFieldPageStatePersister.Load()
                //   at System.Web.UI.Page.LoadPageStateFromPersistenceMedium()
                //   at System.Web.UI.Page.LoadAllState()
                //   at System.Web.UI.Page.ProcessRequestMain(Boolean includeStagesBeforeAsyncPoint, Boolean includeStagesAfterAsyncPoint)
                //----------
                //Server time: 28/10/2011 14:07:04
                //UTC time: 28/10/2011 13:07:04
                //Process: w3wp (12672)
                //Thread: 3 from ThreadPool
                //User name: Twynter
                //URL: http://cati.euro.confirmit.com:81/Supervisor/Survey.aspx?ID=77122
                //Referrer: https://cati.euro.confirmit.com/Supervisor/Survey.aspx?ID=77122
                //User agent: Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)
                //User host: 93.157.223.22
                //Form variables: 
                //__LASTFOCUS = ctl00xContentxSrvInfoxdialogxgbTabsxtabsCtrlxxctl3xSrvInfoQuotasxmxgridxTopToolbar_Item_11
                //__EVENTTARGET = ctl00$Content$SrvInfo$dialog$gbTabs$tabsCtrl$_ctl3$SrvInfoQuotas$m_grid
                //__EVENTARGUMENT = __command_Update
                //Company: Facts International Ltd (465)
                //CATI version: 16.5.3731.47249
                #endregion
                new[]
                {
                    "System.Web.UI.ViewStateException: Invalid viewstate."
                },

                #region Whole message example
                //System.Web.HttpException: This is an invalid script resource request.
                //   at System.Web.Handlers.ScriptResourceHandler.ProcessRequest(HttpContext context)
                //   at System.Web.HttpApplication.CallHandlerExecutionStep.System.Web.HttpApplication.IExecutionStep.Execute()
                //   at System.Web.HttpApplication.ExecuteStep(IExecutionStep step, Boolean& completedSynchronously)
                //----------
                //Server time: 31/10/2011 19:43:13
                //UTC time: 31/10/2011 19:43:13
                //Process: w3wp (4968)
                //Thread: 3 from ThreadPool
                //User name: Twynter
                //URL: http://cati.euro.confirmit.com:81/Supervisor/ScriptResource.axd?d=xyPN4EfWE-dw9Gg3jNkmZqzC5EjYN-DL57MbjYD5ZZ3Pve_Tjj_Iu-wBB04uTWdxcGAdxqiMgQFrJZUa0ad                          <table id=
                //Referrer: https://cati.euro.confirmit.com/Supervisor/Messaging/SendMessageView.aspx
                //User agent: Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)
                //User host: 93.157.223.22
                //Company: Facts International Ltd (465)
                //CATI version: 16.5.3731.47249
                #endregion
                new[]
                {
                    "System.Web.HttpException: This is an invalid script resource request.",
                    "ScriptResource.axd?d="
                },

                #region Whole message example
                //System.Web.HttpException: This is an invalid webresource request.
                //   at System.Web.Handlers.AssemblyResourceLoader.System.Web.IHttpHandler.ProcessRequest(HttpContext context)
                //   at System.Web.HttpApplication.CallHandlerExecutionStep.System.Web.HttpApplication.IExecutionStep.Execute()
                //   at System.Web.HttpApplication.ExecuteStep(IExecutionStep step, Boolean& completedSynchronously)
                //----------
                //Server time: 31/10/2011 17:16:49
                //UTC time: 31/10/2011 17:16:49
                //Process: w3wp (4968)
                //Thread: 9 from ThreadPool
                //User name: Twynter
                //URL: http://cati.euro.confirmit.com:81/Supervisor/WebResource.axd?d=9dHODT0VT59CpnyfJeVlpDpxoXZ-3PfF5IKQ85HL1pqk2gG9oLMVlIDDVPSYfm7iCI4Chtte so once the prize board has been prepared I will ask them to choose from itWell done!2 an hour everyone !!</textarea></td></tr>            </table>                                    </div>                        </td>                    </tr>                </table>            </td>        </tr>        <tr style=
                //Referrer: https://cati.euro.confirmit.com/Supervisor/Messaging/SendMessageView.aspx
                //User agent: Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)
                //User host: 93.157.223.22
                //Company: Facts International Ltd (465)
                //CATI version: 16.5.3731.47249
                #endregion
                new[]
                {
                    "System.Web.HttpException: This is an invalid webresource request.",
                    "WebResource.axd?d="
                },

                #region Whole message example
                //System.Web.HttpException: Request timed out.
                //----------
                //Server time: 14/11/2011 10:00:39
                //UTC time: 14/11/2011 10:00:39
                //Process: w3wp (6208)
                //Thread: 159 from ThreadPool
                //User name: jody
                //URL: http://cati.euro.confirmit.com:81/Supervisor/FileExport.ashx?filename=CallList-1111140906.xlsx
                //Referrer: 
                //User agent: Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; CMDTDF; InfoPath.2; .NET4.0C; .NET4.0E)
                //User host: 212.250.142.205
                //Company: Fieldworks (195)
                //CATI version: 16.5.3803.48113
                #endregion
                new[]
                {
                    "System.Web.HttpException: Request timed out.",
                    "FileExport.ashx?"
                },

                #region Whole message example
                //Confirmit.CATI.Common.Exceptions.InternalErrorException: An authorization error has occured because of incorrect security client key passed in url.
                //   at Confirmit.CATI.Supervisor.Global.Application_AuthenticateRequest(Object sender, EventArgs e) in c:\dev\Confirmit\CATI_Release_CapricaSix_PRL\Sources\Supervisor\Confirmit.CATI.Supervisor\Global.asax.cs:line 98
                //   at System.Web.HttpApplication.SyncEventExecutionStep.System.Web.HttpApplication.IExecutionStep.Execute()
                //   at System.Web.HttpApplication.ExecuteStep(IExecutionStep step, Boolean& completedSynchronously)
                //----------
                //Server time: 11/14/2011 10:24:26 AM
                //UTC time: 11/14/2011 10:24:26 AM
                //Process: w3wp (12760)
                //Thread: 3 from ThreadPool
                //URL: http://cati.euro.confirmit.com/Supervisor/ErrorPage.aspx?Message=Internal+server+error
                //Referrer: 
                //User agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.04506.30; InfoPath.2; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)
                //User host: 212.250.142.205
                //Company: Unknown Company (0)
                //CATI version: 16.5.3803.48113
                #endregion
                new[]
                {
                    "An authorization error has occured because of incorrect security client key passed in url.",
                    "Global.Application_AuthenticateRequest("
                },

                #region Whole message example
                // http://msdn.microsoft.com/en-GB/library/System.ServiceModel.Diagnostics.TraceHandledException.aspxHandling an exception. Exception details: System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.SurveyInManualDialingModeExceptionDetails]: LoginToDialer. This survey is set to be dialed manually so it is not possible to log into the dialer. (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.SurveyInManualDialingModeExceptionDetails).Confirmit.CATI.Backend.exeSystem.ServiceModel.FaultException`1[[Confirmit.CATI.Common.Exceptions.SurveyInManualDialingModeExceptionDetails, Confirmit.CATI.Common, Version=18.5.9731.0, Culture=neutral, PublicKeyToken=8134450e5a05c0c1]], System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089LoginToDialer. This survey is set to be dialed manually so it is not possible to log into the dialer.   at Confirmit.CATI.Common.WcfTools.ErrorContextHandler.ErrorHandlingInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp; outputs) in w:\BuildAgent\work\23d0805ab669d0d8\Confirmit.CATI.Common\WcfTools\ErrorContextHandler\ErrorHandlingInvoker.cs:line 58
                // at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage31(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.SurveyInManualDialingModeExceptionDetails]: LoginToDialer. This survey is set to be dialed manually so it is not possible to log into the dialer. (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.SurveyInManualDialingModeExceptionDetails).
                // ----------
                // Server time: 22/04/2015 14:01:01
                // UTC time: 22/04/2015 13:01:01
                // Process: Confirmit.CATI.Backend (1792)
                // Thread: 33 from ThreadPool
                // Company: ICM Direct (17)
                // CATI version: 18.5.9731.0
                // CS: 65536
                #endregion
                new[]
                {
                    "This survey is set to be dialed manually so it is not possible to log into the dialer."
                },

                #region Whole message example
                // http://msdn.microsoft.com/en-GB/library/System.ServiceModel.Diagnostics.TraceHandledException.aspxHandling an exception. Exception details: System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.UserMessageExceptionDetails]: Appointment with date 10/04/2015 19:30:00 is out of shifts (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.UserMessageExceptionDetails).Confirmit.CATI.Backend.exeSystem.ServiceModel.FaultException`1[[Confirmit.CATI.Common.Exceptions.UserMessageExceptionDetails, Confirmit.CATI.Common, Version=18.5.9684.0, Culture=neutral, PublicKeyToken=8134450e5a05c0c1]], System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Appointment with date 10/04/2015 19:30:00 is out of shifts   at Confirmit.CATI.Common.WcfTools.ErrorContextHandler.ErrorHandlingInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp; outputs) in w:\BuildAgent\work\23d0805ab669d0d8\Confirmit.CATI.Common\WcfTools\ErrorContextHandler\ErrorHandlingInvoker.cs:line 58
                // at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage31(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.UserMessageExceptionDetails]: Appointment with date 10/04/2015 19:30:00 is out of shifts (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.UserMessageExceptionDetails).
                // ----------
                // Server time: 10/04/2015 04:51:38
                // UTC time: 10/04/2015 03:51:38
                // Process: Confirmit.CATI.Backend (13052)
                // Thread: 30 from ThreadPool
                // Company: Ronin (600)
                // CATI version: 18.5.9684.0
                // CS: 65536
                #endregion
                new []
                {
                    "is out of shifts"
                },

                #region Whole message example
                // http://msdn.microsoft.com/en-GB/library/System.ServiceModel.Diagnostics.TraceHandledException.aspxHandling an exception. Exception details: System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.InvalidInterviewerCredentialsExceptionDetails]: Invalid user name or password. (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.InvalidInterviewerCredentialsExceptionDetails).Confirmit.CATI.Backend.exeSystem.ServiceModel.FaultException`1[[Confirmit.CATI.Common.Exceptions.InvalidInterviewerCredentialsExceptionDetails, Confirmit.CATI.Common, Version=18.5.9731.0, Culture=neutral, PublicKeyToken=8134450e5a05c0c1]], System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Invalid user name or password.   at Confirmit.CATI.Common.WcfTools.ErrorContextHandler.ErrorHandlingInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp; outputs) in w:\BuildAgent\work\23d0805ab669d0d8\Confirmit.CATI.Common\WcfTools\ErrorContextHandler\ErrorHandlingInvoker.cs:line 58
                // at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage31(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.InvalidInterviewerCredentialsExceptionDetails]: Invalid user name or password. (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.InvalidInterviewerCredentialsExceptionDetails).
                // ----------
                // Server time: 23/04/2015 12:46:51
                // UTC time: 23/04/2015 11:46:51
                // Process: Confirmit.CATI.Backend (1452)
                // Thread: 12 from ThreadPool
                // Company: Default instance (0)
                // CATI version: 18.5.9731.0
                // CS: 65536
                #endregion
                new []
                {
                    "Invalid user name or password."
                },

                #region Whole message example
                // InterviewRecordingManager.StopRecording: surveyName = p1687286440, interviewId = 259101: Unknown StopRecordingMode 'PowerProbe', StopRecordingMode.Both will be used. 
                //----------
                // Server time: 18/11/2015 14:12:22
                // UTC time: 18/11/2015 14:12:22
                // Process: Confirmit.CATI.Backend (21804)
                // Thread: 23 from ThreadPool
                // Company: Facts International Ltd (465) 
                // CATI version: 19.0.10266.0
                // CS: 65536 
                #endregion
                new []
                {
                    ": Unknown StopRecordingMode '"
                },

                #region Whole message example
                //http://msdn.microsoft.com/en-GB/library/System.ServiceModel.Diagnostics.TraceHandledException.aspxHandling an exception. Exception details: System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.PredictiveSurveyWithoutDialerExceptionDetails]: Predictive dialing surveys without a dialer are only available in manual selection mode. (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.PredictiveSurveyWithoutDialerExceptionDetails).Confirmit.CATI.Backend.exeSystem.ServiceModel.FaultException`1[[Confirmit.CATI.Common.Exceptions.PredictiveSurveyWithoutDialerExceptionDetails, Confirmit.CATI.Common, Version=19.0.10296.0, Culture=neutral, PublicKeyToken=8134450e5a05c0c1]], System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Predictive dialing surveys without a dialer are only available in manual selection mode.   at Confirmit.CATI.Common.WcfTools.ErrorContextHandler.ErrorHandlingInvoker.Invoke(Object instance, Object[] inputs, Object[]&amp; outputs) in w:\BuildAgent\work\ba101fd0ee3bd726\Confirmit.CATI.Common\WcfTools\ErrorContextHandler\ErrorHandlingInvoker.cs:line 58
                // at System.ServiceModel.Dispatcher.DispatchOperationRuntime.InvokeBegin(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage5(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.ImmutableDispatchRuntime.ProcessMessage31(MessageRpc&amp; rpc)
                // at System.ServiceModel.Dispatcher.MessageRpc.Process(Boolean isOperationContextSet)System.ServiceModel.FaultException`1[Confirmit.CATI.Common.Exceptions.PredictiveSurveyWithoutDialerExceptionDetails]: Predictive dialing surveys without a dialer are only available in manual selection mode. (Fault Detail is equal to Confirmit.CATI.Common.Exceptions.PredictiveSurveyWithoutDialerExceptionDetails).
                //----------
                // Server time: 25/11/2015 09:00:15
                // UTC time: 25/11/2015 09:00:15
                // Process: Confirmit.CATI.Backend (2032)
                // Thread: 28 from ThreadPool
                // Company: BMG Research (517)
                // CATI version: 19.0.10296.0
                // CS: 65536
                #endregion
                new []
                {
                    "Predictive dialing surveys without a dialer are only available in manual selection mode."
                },

                //Set dialing mode isn't supported for survey with dialing mode = Preview----------
                new []
                {
                    "Set dialing mode isn't supported for survey with dialing mode"
                },

                // Next 2 records are generated by old Invade dialer that is used by BMG on EURO and Dimark on US.
                // These errors are generated if a second Hangup is issued for the same call and do not cause any problems.

                new []
                {
                    "<Hangup>",
                    "UnknownError",
                    "tenantId=1430"
                },
                #region Whole message example
                //IDialerAPI.Int32 <Hangup>b__0(). Call is failed with error code: UnknownError /// dialerId=1, dialerId=1, tenantId=517, campaignId=1875168152, agentId=932266
                //----------
                //Server time: 22/08/2019 15:30:46
                //UTC time: 22/08/2019 14:30:46
                //Process: Confirmit.CATI.Backend (9944)
                //Thread: 28 from ThreadPool
                //Company: BMG Research (517)
                //CATI version: 25.0.3620.0
                //CS: 65536
                #endregion
                new []
                {
                    "<Hangup>",
                    "UnknownError",
                    "tenantId=517"
                },
            };

        /// <summary>
        /// Some kind of internal trace filter.
        /// </summary>
        /// <param name="message">The trace message.</param>
        /// <returns>true to trace the specified event; otherwise, false.</returns>
        public static bool ShouldTrace(string message)
        {
            return !ExcludeMessages.Any(x => x.All(message.Contains));
        }

        public static bool ShouldTraceErrorAsWarning(string message)
        {
            return ErrorsToTraceAsWarningMessages.Any(x => x.All(message.Contains));
        }
    }
}
