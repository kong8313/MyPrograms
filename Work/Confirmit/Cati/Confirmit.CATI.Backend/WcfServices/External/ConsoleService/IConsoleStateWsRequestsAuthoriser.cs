using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    public interface IConsoleStateWsRequestsAuthoriser
    {
        void AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task);
    }
}