using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Interfaces
{
    /// <summary>
    /// Interface to work with surveys
    /// </summary>
    public interface ISurveyService
    {
        /// <summary>
        /// Get surveys using OData filter
        /// </summary>
        /// <param name="odataQuery">The string with an OData query</param>
        /// <returns>List of Survey objects</returns>
        Task<List<Survey>> GetAsync(string odataQuery);

        /// <summary>
        /// Get survey by unique identifier of the survey
        /// </summary>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns>Survey object</returns>
        Task<Survey> GetAsyncByKey(string surveyId);

        /// <summary>
        /// Open the survey
        /// </summary>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns></returns>
        Task Open(string surveyId);

        /// <summary>
        /// Close the survey
        /// </summary>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns></returns>
        Task Close(string surveyId);

        /// <summary>
        /// Shutdown the survey
        /// </summary>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns></returns>
        Task Shutdown(string surveyId);

        /// <summary>
        /// Get all assignment information related to the specified survey
        /// </summary>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns>List of ResourceAssignment objects</returns>
        Task<List<ResourceAssignment>> GetAssignments(string surveyId, int callCenterId);

        /// <summary>
        /// Remove all assignments related to the specified survey
        /// </summary>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        Task CleanAssignments(string surveyId, int callCenterId);

        /// <summary>
        /// Get basic properties of the survey
        /// </summary>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns>SurveyBasicProperties object</returns>
        Task<SurveyBasicProperties> GetBasicProperties(string surveyId);

        /// <summary>
        /// Update basic properties of the survey
        /// </summary>
        /// <param name="properties">Object with the survey properties</param>
        /// <returns></returns>
        Task PutBasicProperties(SurveyBasicProperties properties);
    }
}