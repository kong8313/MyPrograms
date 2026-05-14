using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Confirmit.CATI.Backend.WebApiServices.Models;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public interface IDatabaseContext : IDisposable
    {
        string ExecutionLog {get;}
        DbSet<BreakHistory> BreakHistory { get; set; }
        DbSet<CallHistory> CallHistory { get; set; }
        DbSet<InterviewerSessionHistory> InterviewerSessionHistory { get; set; }
        DbSet<Interviewer> Interviewer { get; set; }
        DbSet<Group> InterviewerGroup { get; set; }
        DbSet<Membership> Membership { get; set; }
        DbSet<Survey> Survey { get; set; }
        DbSet<TelephoneBlacklistItem> TelephoneBlacklist { get; set; }
    }
}