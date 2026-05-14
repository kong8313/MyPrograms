namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IPersonGroupService
    {
        bool IsExistsAndNotAdministrative(int[] groupIds);

        bool IsGroupContainsInterviewer(int interviewerId, string groupName);
    }
}
