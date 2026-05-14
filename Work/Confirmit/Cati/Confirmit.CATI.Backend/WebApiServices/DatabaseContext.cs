using System.Data.Entity;
using System.Text;
using Confirmit.CATI.Backend.WebApiServices.Models;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        private readonly StringBuilder _executionLog;

        public DatabaseContext(string connectionString)
            : base(connectionString)
        {
            _executionLog = new StringBuilder();
            Database.Log = s => _executionLog.AppendLine(s);
        }

        public string ExecutionLog
        {
            get
            {
                return _executionLog.ToString();
            }    
        }

        public virtual DbSet<BreakHistory> BreakHistory { get; set; }
        public virtual DbSet<CallHistory> CallHistory { get; set; }
        public virtual DbSet<InterviewerSessionHistory> InterviewerSessionHistory { get; set; }
        public virtual DbSet<Interviewer> Interviewer { get; set; }
        public virtual DbSet<Group> InterviewerGroup { get; set; }
        public virtual DbSet<Membership> Membership { get; set; }
        public virtual DbSet<Survey> Survey { get; set; }
        public virtual DbSet<TelephoneBlacklistItem> TelephoneBlacklist { get; set; }
        
    }
}
