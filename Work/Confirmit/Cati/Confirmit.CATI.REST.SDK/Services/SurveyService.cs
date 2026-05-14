using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Constants;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;
using Newtonsoft.Json;

namespace Confirmit.CATI.REST.SDK.Services
{
    /// <summary>
    /// Class to work with surveys
    /// </summary>
    public class SurveyService : ISurveyService
    {
        private readonly IRestClient _restClient;

        /// <summary>
        /// Creates and initializes an instance of SurveyService class
        /// </summary>
        /// <param name="restClient">The instance of the rest client object</param>
        public SurveyService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Get surveys using OData filter
        /// </summary>
        /// <param name="odataQuery">The string with an OData query</param>
        /// <returns>List of Survey objects</returns>
        public async Task<List<Survey>> GetAsync(string odataQuery)
        {
            return await _restClient.GetAsyncMany<Survey>(UrlConstants.SurveysUrlItem, odataQuery);
        }

        /// <summary>
        /// Get survey by the survey ID
        /// </summary>
        /// <param name="surveyId">Survey ID looks like pXXXXXXXXXX</param>
        /// <returns>Survey object</returns>
        public async Task<Survey> GetAsyncByKey(string surveyId)
        {
            return await _restClient.GetAsyncSingle<Survey>(UrlConstants.SurveysUrlItem, surveyId);
        }

        /// <summary>
        /// Open the survey
        /// </summary>
        /// <param name="surveyId">Survey ID looks like pXXXXXXXXXX</param>
        /// <returns></returns>
        public async Task Open(string surveyId)
        {
            await _restClient.InvokeActionForVoid(UrlConstants.SurveysUrlItem, surveyId, SurveyActions.Namespace, SurveyActions.Open);
        }

        /// <summary>
        /// Close the survey
        /// </summary>
        /// <param name="surveyId">Survey ID looks like pXXXXXXXXXX</param>
        /// <returns></returns>
        public async Task Close(string surveyId)
        {
            await _restClient.InvokeActionForVoid(UrlConstants.SurveysUrlItem, surveyId, SurveyActions.Namespace, SurveyActions.Close);
        }

        /// <summary>
        /// Shutdown the survey
        /// </summary>
        /// <param name="surveyId">Survey ID looks like pXXXXXXXXXX</param>
        /// <returns></returns>
        public async Task Shutdown(string surveyId)
        {
            await _restClient.InvokeActionForVoid(UrlConstants.SurveysUrlItem, surveyId, SurveyActions.Namespace, SurveyActions.Shutdown);
        }

        /// <summary>
        /// Get all assignment information related to the specified survey
        /// </summary>
        /// <param name="surveyId">Survey ID looks like pXXXXXXXXXX</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns>List of ResourceAssignment objects</returns>
        public async Task<List<ResourceAssignment>> GetAssignments(string surveyId, int callCenterId)
        {
            return await _restClient.InvokeActionForList<ResourceAssignment>(
                UrlConstants.SurveysUrlItem,
                surveyId,
                SurveyActions.Namespace,
                SurveyActions.GetAssignments,
                $"callCenterId={callCenterId}");
        }

        /// <summary>
        /// Remove all assignments related to the specified survey
        /// </summary>
        /// <param name="surveyId">Survey ID looks like pXXXXXXXXXX</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        public async Task CleanAssignments(string surveyId, int callCenterId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.SurveysUrlItem, 
                surveyId, 
                SurveyActions.Namespace, 
                SurveyActions.CleanAssignments);
        }

        /// <summary>
        /// Get basic properties of the survey
        /// </summary>
        /// <param name="surveyId">Survey ID looks like pXXXXXXXXXX</param>
        /// <returns>SurveyBasicProperties object</returns>
        public async Task<SurveyBasicProperties> GetBasicProperties(string surveyId)
        {
            return await _restClient.InvokeActionForSingle<SurveyBasicProperties>(
                UrlConstants.SurveysUrlItem,
                surveyId,
                SurveyActions.Namespace,
                SurveyActions.GetBasicProperties);
        }

        /// <summary>
        /// Update basic properties of the survey
        /// </summary>
        /// <param name="properties">Object with the survey properties</param>
        /// <returns></returns>
        public async Task PutBasicProperties(SurveyBasicProperties properties)
        {
            await _restClient.InvokeActionForSingle<SurveyBasicProperties>(
                UrlConstants.SurveysUrlItem,
                properties.SurveyId,
                SurveyActions.Namespace,
                SurveyActions.PutBasicProperties,
                $"properties='{JsonConvert.SerializeObject(properties)}'");
        }
    }
}