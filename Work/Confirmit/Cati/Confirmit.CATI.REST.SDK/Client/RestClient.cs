using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Exceptions;
using Confirmit.CATI.REST.SDK.Interfaces;
using Newtonsoft.Json;

using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Client
{
    /// <summary>
    /// General class to send requests to CATI REST API.
    /// This class is used from SDK services (such as GroupService, SurveyService and others) and is not supposed to be used directly.
    /// </summary>
    public class RestClient : IRestClient
    {
        /// <summary>
        /// HttpClient object that is used to send requests to CATI REST API
        /// </summary>
        public HttpClient HttpClient { get; private set; }

        private readonly string _address;
        private readonly int _companyId;

        private static HttpClient CreateHttpClient(string catiServerAddress, string proxyServerAddress, string xConfirmitApiKey)
        {
            HttpClient client;

            if (!string.IsNullOrEmpty(proxyServerAddress))
            {
                var clientHandler = new HttpClientHandler
                {
                    Proxy = new WebProxy(proxyServerAddress),
                };

                client = new HttpClient(clientHandler);
            }
            else
            {
                client = new HttpClient();
            }

            client.BaseAddress = new Uri(catiServerAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(Constants.Constants.XConfirmitApiKeyHeader, xConfirmitApiKey);

            return client;
        }

        private static void EnsureSuccessStatusCode(HttpResponseMessage response, string content)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.Created:
                    break;
                case HttpStatusCode.Forbidden:
                    throw new ForbiddenException(response.RequestMessage.RequestUri.ToString(), response.ReasonPhrase, content);
                case HttpStatusCode.NotFound:
                    throw new NotFoundException(response.RequestMessage.RequestUri.ToString(), response.ReasonPhrase, content);
                case HttpStatusCode.ServiceUnavailable:
                    throw new ServiceUnavailableException(response.RequestMessage.RequestUri.ToString(), response.ReasonPhrase, content);
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException(response.RequestMessage.RequestUri.ToString(), response.ReasonPhrase, content);
                case HttpStatusCode.InternalServerError:
                    throw new InternalServerErrorException(response.RequestMessage.RequestUri.ToString(), response.ReasonPhrase, content);
                default:
                    throw new RestClientException(response.RequestMessage.RequestUri.ToString(), response.StatusCode, response.ReasonPhrase, content);
            }
        }

        /// <summary>
        /// Creates and initializes an instance of RestClient class by address, proxyAddress, xConfirmitApiKey and companyId
        /// </summary>
        /// <param name="address">Url address to CATI REST API</param>
        /// <param name="proxyAddress">Proxy address if needed</param>
        /// <param name="xConfirmitApiKey">xConfirmitApiKey value</param>
        /// <param name="companyId">Unique identifier of the company</param>
        public RestClient(
            string address, 
            string proxyAddress, 
            string xConfirmitApiKey, 
            int companyId)
        {
            _address = address;
            _companyId = companyId;
            HttpClient = CreateHttpClient(address, proxyAddress, xConfirmitApiKey);
            
        }

        /// <summary>
        /// Sends GET request and returns a result as a list of integer values
        /// </summary>
        /// <param name="urlItem">REST API url part before key value</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="urlSubItem">REST API url part after key value</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>List of integer values</returns>
        public async Task<List<int>> GetAsyncIds<T>(string urlItem, int key, string urlSubItem)
        {
            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}({key})/{urlSubItem}";

            using (var response = await HttpClient.GetAsync(url))
            {
                var json = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, json);

                var itemsRoot = JsonConvert.DeserializeObject<Root<List<int>>>(json);

                return itemsRoot.Value;
            }
        }

        /// <summary>
        /// Sends GET request and returns a result as a list of objects
        /// </summary>
        /// <param name="urlItem">REST API url part before key value</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="urlSubItem">REST API url part after key value</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>List of objects</returns>
        public async Task<List<T>> GetAsyncSubItem<T>(string urlItem, int key, string urlSubItem)
        {
            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}({key})/{urlSubItem}";

            using (var response = await HttpClient.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);

                var itemsRoot = JsonConvert.DeserializeObject<Root<List<T>>>(content);

                return itemsRoot.Value;
            }
        }

        /// <summary>
        /// Sends GET request and returns a result as a list of objects using OData query
        /// </summary>
        /// <param name="urlItem">REST API url part</param>
        /// <param name="odataQuery">String with OData filter</param>
        /// <typeparam name="T">Type of object to deserialize</typeparam>
        /// <returns>List of objects</returns>
        public async Task<List<T>> GetAsyncMany<T>(string urlItem, string odataQuery)
        {
            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}?{odataQuery}";

            using (var response = await HttpClient.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);

                var itemsRoot = JsonConvert.DeserializeObject<Root<List<T>>>(content);

                return itemsRoot.Value;
            }
        }

        /// <summary>
        /// Sends GET request and returns a result as an object using integer item key
        /// </summary>
        /// <param name="urlItem">REST API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Object</returns>
        public async Task<T> GetAsyncSingle<T>(string urlItem, int key)
        {
            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}({key})";

            using (var response = await HttpClient.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);

                var item = JsonConvert.DeserializeObject<T>(content);

                return item;
            }
        }

        /// <summary>
        /// Sends GET request and returns a result as an object by string item key
        /// </summary>
        /// <param name="urlItem">REST API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Object</returns>
        public async Task<T> GetAsyncSingle<T>(string urlItem, string key)
        {
            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}('{key}')";

            using (var response = await HttpClient.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);

                var item = JsonConvert.DeserializeObject<T>(content);

                return item;
            }
        }

        /// <summary>
        /// Sends POST request and returns a result as an integer value
        /// </summary>
        /// <param name="urlItem">REST API url part</param>
        /// <param name="t">Type of object</param>
        /// <param name="key">Key value to identify object (must be 0 in this case)</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Integer value</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> PostAsync<T>(string urlItem, T t, int key)
        {
            if (key != 0)
            {
                throw new ArgumentException("key");
            }

            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}";

            using (var response = await HttpClient.PostAsJsonAsync(url, t))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);

                var idRoot = JsonConvert.DeserializeObject<Root<int>>(content);

                return idRoot.Value;
            }
        }

        /// <summary>
        /// Sends POST request without key and checks success status code.
        /// </summary>
        /// <param name="urlItem">REST API url part</param>
        /// <param name="t">Type of object</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task PostAsync<T>(string urlItem, T t)
        {
            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}";

            using (var response = await HttpClient.PostAsJsonAsync(url, t))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);
            }
        }

        /// <summary>
        /// Sends PUT request and returns a result as an integer value
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="t">Type of object</param>
        /// <param name="key">Key value to identify object</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Integer value</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> PutAsync<T>(string urlItem, T t, int key)
        {
            if (key == 0)
            {
                throw new ArgumentException("key");
            }

            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}({key})";

            using (var response = await HttpClient.PutAsJsonAsync(url, t))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);

                var idRoot = JsonConvert.DeserializeObject<Root<int>>(content);

                return idRoot.Value;
            }
        }

        /// <summary>
        /// Sends DELETE request
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task DeleteAsync(string urlItem, int key)
        {
            if (key == 0)
            {
                throw new ArgumentException("key");
            }

            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}({key})";

            using (var response = await HttpClient.DeleteAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);
            }
        }

        /// <summary>
        /// Sends GET request using integer item key
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="nameSpace">Namespace</param>
        /// <param name="action">Action</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task InvokeActionForVoid(string urlItem, int key, string nameSpace, string action, string parameters = null)
        {
            if (key == 0)
            {
                throw new ArgumentException("key");
            }

            if (parameters != null)
            {
                parameters = "(" + parameters + ")";
            }

            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}({key})/{nameSpace}.{action}{parameters}";

            using (var response = await HttpClient.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);
            }
        }

        /// <summary>
        /// Sends GET request using string item key
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="nameSpace">Namespace</param>
        /// <param name="action">Action</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task InvokeActionForVoid(string urlItem, string key, string nameSpace, string action, string parameters = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key");
            }

            if (parameters != null)
            {
                parameters = "(" + parameters + ")";
            }

            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}('{key}')/{nameSpace}.{action}{parameters}";

            using (var response = await HttpClient.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);
            }
        }

        /// <summary>
        /// Sends GET request and returns a result as a list of objects using string item key
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="nameSpace">Namespace</param>
        /// <param name="action">Action</param>
        /// <param name="parameters">Parameters</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>List of objects</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<List<T>> InvokeActionForList<T>(string urlItem, string key, string nameSpace, string action, string parameters = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key");
            }

            if (parameters != null)
            {
                parameters = "(" + parameters + ")";
            }

            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}('{key}')/{nameSpace}.{action}{parameters}";

            using (var response = await HttpClient.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);

                var itemsRoot = JsonConvert.DeserializeObject<Root<List<T>>>(content);

                return itemsRoot.Value;
            }
        }

        /// <summary>
        /// Sends GET request and returns a result as a list of objects using integer item key
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="nameSpace">Namespace</param>
        /// <param name="action">Action</param>
        /// <param name="parameters">Parameters</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>List of objects</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<List<T>> InvokeActionForList<T>(string urlItem, int key, string nameSpace, string action, string parameters = null)
        {
            if (key == 0)
            {
                throw new ArgumentException("key");
            }

            if (parameters != null)
            {
                parameters = "(" + parameters + ")";
            }

            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}({key})/{nameSpace}.{action}{parameters}";

            using (var response = await HttpClient.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);

                var itemsRoot = JsonConvert.DeserializeObject<Root<List<T>>>(content);

                return itemsRoot.Value;
            }
        }

        /// <summary>
        /// Sends GET request and returns a result as an object using string item key
        /// </summary>
        /// <param name="urlItem">REST API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="nameSpace">Namespace</param>
        /// <param name="action">Action</param>
        /// <param name="parameters">Parameters</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Object</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<T> InvokeActionForSingle<T>(string urlItem, string key, string nameSpace, string action, string parameters = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key");
            }

            if (parameters != null)
            {
                parameters = "(" + parameters + ")";
            }

            var url = $"{_address}catiapi/companies/{_companyId}/{urlItem}('{key}')/{nameSpace}.{action}{parameters}";

            using (var response = await HttpClient.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();

                EnsureSuccessStatusCode(response, content);

                var value = JsonConvert.DeserializeObject<T>(content);

                return value;
            }
        }

        /// <summary>
        /// Release used resources
        /// </summary>
        public void Dispose()
        {
            var client = HttpClient;
            if (client != null)
            {
                client.Dispose();
                HttpClient = null;
            }
        }
    }
}
