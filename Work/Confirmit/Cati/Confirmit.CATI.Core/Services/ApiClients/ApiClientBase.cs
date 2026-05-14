using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.Identity.Sdk.Clients;
using Confirmit.Identity.Sdk.Tokens;

namespace Confirmit.CATI.Core.Services.ApiClients
{
    public abstract class ApiClientBase
    {
        protected ITokenCacheService _cacheService;

        protected Uri CombineUrl(Uri baseAddress, string relativeUrl)
        {
            return new Uri(baseAddress.AbsoluteUri.TrimEnd('/') + "/" + relativeUrl.TrimStart('/'));
        }

        protected async Task<HttpResponseMessage> MakeHttpRequestWithCachedToken(string scopes, Func<string, Task<HttpResponseMessage>> requestFunction)
        {
            try
            {
                var response = await CallFunctionWithCachedToken(scopes, requestFunction);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _cacheService.Remove(scopes);

                    response = await CallFunctionWithCachedToken(scopes, requestFunction);
                }

                return response;
            }
            catch (Exception ex)
            {
                HealthStateExceptionHandler<SocketException>.OnException(ex);
                throw;
            }
        }


        protected async Task<T> CallFunctionWithCachedToken<T>(string scopes, Func<string, Task<T>> function)
        {
            return await WithSocketExceptionHandling(async() =>
            {
                var cachedToken = _cacheService.Get(scopes);
                if (cachedToken == null)
                {
                    return await ServiceClient.InvokeAsync(
                        await ServiceClientFactory.CreateClientAsync(scopes,
                            new TrustedSubsystemClientSecretProvider()),
                        async () =>
                        {
                            var accessToken = TokenStore.GetStoredToken();
                            _cacheService.Set(scopes, accessToken);
                            return await function(accessToken);
                        });
                }

                return await function(cachedToken);
            });
        }

        protected async Task<TResult> WithSocketExceptionHandling<TResult>(Func<Task<TResult>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                HealthStateExceptionHandler<SocketException>.OnException(ex);
                throw;
            }
        }
    }
}