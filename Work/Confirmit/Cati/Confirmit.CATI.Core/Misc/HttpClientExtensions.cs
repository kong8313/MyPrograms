using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Misc
{
    public static class HttpClientExtensions
    {
        public static async Task<string> GetStringAsync(this HttpClient httpClient, string uri, string bearerToken)
        {
            return await GetStringAsync(httpClient, new Uri(uri, UriKind.RelativeOrAbsolute), bearerToken).ConfigureAwait(false);
        }

        public static async Task<string> GetStringAsync(this HttpClient httpClient, Uri uri, string bearerToken)
        {
            var response = await httpClient.GetResponseAsync(uri, bearerToken).ConfigureAwait(false);

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> GetResponseAsync(this HttpClient httpClient, Uri uri, string bearerToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            return await httpClient.SendAsync(request).ConfigureAwait(false);
        }


        public static async Task<T> GetModelAsync<T>(this HttpClient httpClient, string uri, string bearerToken)
        {
            return await GetModelAsync<T>(httpClient, new Uri(uri, UriKind.RelativeOrAbsolute), bearerToken).ConfigureAwait(false);
        }

        public static async Task<T> GetModelAsync<T>(this HttpClient httpClient, Uri uri, string bearerToken)
        {
            var response = await GetStringAsync(httpClient, uri, bearerToken).ConfigureAwait(false);

            var deserializationSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new Iso8601TimeSpanConverter()
                }
            };

            return JsonConvert.DeserializeObject<T>(response, deserializationSettings);
        }

        public static async Task<T> GetModelAsync<T>(this HttpClient httpClient, Uri uri, string bearerToken, JsonConverter jsonConverter)
        {
            var response = await GetStringAsync(httpClient, uri, bearerToken).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(response, jsonConverter);
        }

        public static async Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, string uri, string bearerToken, HttpContent content)
        {
            return await PostAsync(httpClient, new Uri(uri, UriKind.RelativeOrAbsolute), bearerToken, content).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, Uri uri, string bearerToken, HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content };

            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            var response = await httpClient.SendAsync(request);

            return response;
        }

        public static async Task<HttpResponseMessage> PutAsync(this HttpClient httpClient, Uri uri, string bearerToken, HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri) { Content = content };

            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            var response = await httpClient.SendAsync(request);

            return response;
        }
        
        public static async Task<HttpResponseMessage> DeleteRequestAsync(this HttpClient httpClient, Uri uri, string bearerToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);

            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            var response = await httpClient.SendAsync(request);

            return response;
        }
    }
}