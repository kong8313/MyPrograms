using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Confirmit.CATI.REST.SDK.Interfaces
{
    /// <summary>
    /// Interface for general class to send requests to CATI REST API.
    /// This class is used from SDK service classes (like GroupService, SurveyService and others) and it is not supposed to use this class directly.
    /// </summary>
    public interface IRestClient : IDisposable
    {
        /// <summary>
        /// Send Get request with result as list of integer values
        /// </summary>
        /// <param name="urlItem">Rest API url part before key value</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="urlSubItem">Rest API url part after key value</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>List of integer values</returns>
        Task<List<int>> GetAsyncIds<T>(string urlItem, int key, string urlSubItem);

        /// <summary>
        /// Send Get request with result as list of objects
        /// </summary>
        /// <param name="urlItem">Rest API url part before key value</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="urlSubItem">Rest API url part after key value</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>List of objects</returns>
        Task<List<T>> GetAsyncSubItem<T>(string urlItem, int key, string urlSubItem);

        /// <summary>
        /// Send Get request with result as list of objects using OData request
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="odataQuery">String with OData filter</param>
        /// <typeparam name="T">Type of object to deserialize</typeparam>
        /// <returns>List of objects</returns>
        Task<List<T>> GetAsyncMany<T>(string urlItem, string odataQuery);

        /// <summary>
        /// Send Get request with result as object by integer key
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Object</returns>
        Task<T> GetAsyncSingle<T>(string urlItem, int key);

        /// <summary>
        /// Send Get request with result as object by string key
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Object</returns>
        Task<T> GetAsyncSingle<T>(string urlItem, string key);

        /// <summary>
        /// Send Post request with result as integer value
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="t">Type of object</param>
        /// <param name="key">Key value to identify object (must be 0 in this case)</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Integer value</returns>
        Task<int> PostAsync<T>(string urlItem, T t, int key);

        /// <summary>
        /// Sends POST request without key of the entity and returns a success status code
        /// </summary>
        /// <param name="urlItem">REST API url part</param>
        /// <param name="t">Type of object</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
       Task PostAsync<T>(string urlItem, T t);

        /// <summary>
        ///  Send Put request with result as integer value
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="t">Type of object</param>
        /// <param name="key">Key value to identify object</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Integer value</returns>
        Task<int> PutAsync<T>(string urlItem, T t, int key);

        /// <summary>
        /// Send Delete request
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <returns></returns>
        Task DeleteAsync(string urlItem, int key);

        /// <summary>
        /// Send Get request without result value by integer key
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="nameSpace">Namespace</param>
        /// <param name="action">Action</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        Task InvokeActionForVoid(string urlItem, int key, string nameSpace, string action, string parameters = null);

        /// <summary>
        /// Send Get request without result value by string key
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="nameSpace">Namespace</param>
        /// <param name="action">Action</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        Task InvokeActionForVoid(string urlItem, string key, string nameSpace, string action, string parameters = null);

        /// <summary>
        /// Send Get request with result as list of objects by string key
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="nameSpace">Namespace</param>
        /// <param name="action">Action</param>
        /// <param name="parameters">Parameters</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>List of objects</returns>
        Task<List<T>> InvokeActionForList<T>(string urlItem, int key, string nameSpace, string action, string parameters = null);

        /// <summary>
        /// Send Get request with result as list of objects by integer key
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="nameSpace">Namespace</param>
        /// <param name="action">Action</param>
        /// <param name="parameters">Parameters</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>List of objects</returns>
        Task<List<T>> InvokeActionForList<T>(string urlItem, string key, string nameSpace, string action, string parameters = null);

        /// <summary>
        /// Send Get request with result as objects by string key
        /// </summary>
        /// <param name="urlItem">Rest API url part</param>
        /// <param name="key">Key value to identify object</param>
        /// <param name="nameSpace">Namespace</param>
        /// <param name="action">Action</param>
        /// <param name="parameters">Parameters</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Object</returns>
        Task<T> InvokeActionForSingle<T>(string urlItem, string key, string nameSpace, string action, string parameters = null);

        /// <summary>
        /// Http client object to send requests
        /// </summary>
        HttpClient HttpClient { get; }
    }
}