using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    public interface IConsoleWsRequestsAuthoriser
    {
        /// <summary>
        ///  Authorizes an interviewer. Do not checks that tasks exists.
        /// </summary>
        /// <param name="interviewer">Interviewer entity.</param>
        void AuthoriseRequest(out BvPersonEntity interviewer);

        /// <summary>
        /// Authorizes an interviewer and checks that tasks exists,
        /// </summary>
        /// <param name="interviewer">Interviewer entity.</param>
        /// <param name="task">Task entity.</param>
        void AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task);

        /// <summary>
        ///  Just authorizes an interviewer
        /// </summary>
        ///<returns>Interviewer entity.</returns>
        BvPersonEntity AuthoriseRequest();

        /// <summary>
        /// Authorizes an interviewer and checks that tasks exists, if taskMustExist parameter is set.
        /// </summary>
        /// <param name="interviewer">Interviewer entity.</param>
        /// <param name="task">Task entity.</param>
        /// <param name="taskMustExist"></param>
        void AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task, bool taskMustExist);
    }
}