using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using Dapper;

namespace LoadTestUpdateActiveQuestionSpCaller;

internal class Program
{
    static async Task Main(string[] args)
    {
        var configuration = ConfigurationProvider.GetConfiguration();
        var delays = new ConcurrentBag<int>();
        var cancellationToken = new CancellationTokenSource();

        for (int i = 0; i < configuration.InterviewersAmount; i++)
        {
            var interviewerId = configuration.InitialInterviewerSID + i;
            await StartThread(configuration, interviewerId, delays, cancellationToken.Token);
        }

        await Task.Delay(TimeSpan.FromSeconds(configuration.TestDurationInSeconds));
        cancellationToken.Cancel();
        await Task.Delay(2000);

        for (int i = 0; i < configuration.InterviewersAmount; i++)
        {
            var interviewerId = configuration.InitialInterviewerSID + i;
            await DeleteTask(interviewerId);
        }

        Console.WriteLine($"requests per second: {2 * 1000 * configuration.InterviewersAmount / (configuration.SpCallMinIntervalInMilliseconds + configuration.SpCallMaxIntervalInMilliseconds)}");
        Console.WriteLine($"average delay: {delays.Average()} ms");
    }

    private static async Task StartThread(Configuration configuration, int interviewerId, ConcurrentBag<int> delays, CancellationToken cancellationToken)
    {
        var minDelay = configuration.SpCallMinIntervalInMilliseconds;
        var maxDelay = configuration.SpCallMaxIntervalInMilliseconds;

        await CreateTask(interviewerId);

        async Task CallSPLoop()
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                
                var delayTime = TimeSpan.FromMilliseconds(Randomizer.Next(minDelay, maxDelay));
                await Task.Delay(delayTime);

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                var connectionProvider = new SqlConnectionProvider();
                using (var sqlConnection = connectionProvider.GetConnection())
                {
                    await sqlConnection.ExecuteAsync("BvSpTask_UpdateActiveQuestion", commandType: CommandType.StoredProcedure, param: new {
                        catiInterviewerId = interviewerId,
                        projectId = "",
                        qID = $"question_{Randomizer.Next()}",
                        showTime = DateTime.UtcNow
                    });
                }

                stopWatch.Stop();

                delays.Add(stopWatch.Elapsed.Milliseconds);
                Console.WriteLine($"interviewer {interviewerId} {DateTime.UtcNow} BvSpTask_UpdateActiveQuestion duration: {stopWatch.Elapsed.Milliseconds}");
            }
        }

        await new TaskFactory().StartNew(CallSPLoop);
    }

    private static async Task CreateTask(int interviewerId)
    {
        var connectionProvider = new SqlConnectionProvider();
        using (var sqlConnection = connectionProvider.GetConnection())
        {
            var sql = @"
            INSERT INTO BvTasks([SurveySID]
            ,[InterviewID]
            ,[PersonSID]
            ,[TzID]
            ,[DiallingMode]
            ,[CallOutcome]
            ,[InterviewState]
            ,[StatusLogout]
            ,[LoggedInToDialerState]
            ,[IsLoginRCToDialer]
            ,[ProblemId]
            ,[StationId]
            ,[StartSessionTime]
            ,[EncryptionKey]
            ,[EncryptionIV]
            ,[DialerId]
            ,[StationExtensionNumber]
            ,[IsDialerAgentLocal]
            ,[CallCenterID]
            ,[SessionId]
            ,[NewSurveySID]
            ,[DialTypeId]
            ,[CallType]
            ,[CallConnectionState]
            ,[IsWebConsole]) VALUES(0,0,@InterviewerId,0,1,-1,1,2,0,0,0,'ivr01', @StartSessionTime,0,0,1,1,0,1,@InterviewerId,0,0,0,0,0)";

            await sqlConnection.ExecuteAsync(sql, new {
                InterviewerId = interviewerId,
                StartSessionTime = DateTime.UtcNow
            });
        }

        Console.WriteLine($"Task for {interviewerId} Created");
    }

    private static async Task DeleteTask(int interviewerId)
    {
        var connectionProvider = new SqlConnectionProvider();
        using (var sqlConnection = connectionProvider.GetConnection())
        {
            var sql = @"DELETE FROM BvTasks WHERE PersonSID=@InterviewerId";
            await sqlConnection.ExecuteAsync(sql, new {
                InterviewerId = interviewerId
            });
        }

        Console.WriteLine($"Task for {interviewerId} Deleted");
    }
}