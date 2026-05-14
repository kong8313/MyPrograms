using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.IntegrationTests.Framework.Fakes;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class DateTimeMocker
    {
        private readonly IntegrationTestingFramework _framework;
        private List<Tuple<DateTime, Task>> _postponedTasks = new List<Tuple<DateTime, Task>>();

        public DateTimeMocker(IntegrationTestingFramework framework)
        {
            _framework = framework;
        }

        public DateTimeMocker(DateTime specificDateTime)
        {
            _framework = IntegrationTestingFramework.Instance;
            MockDate(specificDateTime);
        }

        public static DateTimeMocker StartNew()
        {
            return new DateTimeMocker("2018-12-07T08:00:00");
        }

        /// <summary>
        /// Example DateTimeMocker("2018-12-07T08:00:00")
        /// </summary>
        /// <param name="specificDateTime"></param>
        public DateTimeMocker(string specificDateTime)
            : this(DateTime.Parse(specificDateTime))
        {
        }

        public void Reset()
        {
            var sql = @"ALTER TABLE BvTasks Drop column [CurrentUtcTime]";
            _framework.DbEngine.ExecuteNonQuery(sql, CommandType.Text);

            sql = @"
ALTER FUNCTION [dbo].[GetUtcNow] ()
RETURNS DATETIME
WITH SCHEMABINDING
BEGIN
    RETURN GETUTCDATE()
END";
            _framework.DbEngine.ExecuteNonQuery(sql, CommandType.Text);

            sql = @"ALTER TABLE BvTasks ADD [CurrentUtcTime] AS dbo.GetUtcNow()";
            _framework.DbEngine.ExecuteNonQuery(sql, CommandType.Text);

            ServiceLocator.RegisterInstance<ITimeService>(new TimeService());
        }

        public void MockDate(string specificDateTime)
        {
            MockDate(DateTime.Parse(specificDateTime));
        }

        public void MockDate(DateTime specificDateTime)
        {
            var timeService = new TestTimeService(specificDateTime);
            ServiceLocator.RegisterInstance<ITimeService>(timeService);

            ((FakeAsyncManager)ServiceLocator.Resolve<IAsyncManager>()).OnScheduleTask = (delay, task) =>
                _postponedTasks.Add(Tuple.Create(ServiceLocator.Resolve<ITimeService>().GetUtcNow().Add(delay), task));

            SetTime(timeService , specificDateTime);

        }

        private void SetTime(TestTimeService timeService, DateTime specificDateTime)
        {
            using (var transaction = new DatabaseTransactionScope("DateTimeMocker.SetDbTime"))
            {
                var sql = @"ALTER TABLE BvTasks Drop column [CurrentUtcTime]";
                _framework.DbEngine.ExecuteNonQuery(sql, CommandType.Text);

                sql = string.Format(@"
    ALTER FUNCTION [dbo].[GetUtcNow] ()
    RETURNS DATETIME
    WITH SCHEMABINDING
    BEGIN
        RETURN '{0}'
    END", specificDateTime.ToString("yyyy-MM-dd HH:mm:ss"));

                _framework.DbEngine.ExecuteNonQuery(sql, CommandType.Text);

                sql = @"ALTER TABLE BvTasks ADD [CurrentUtcTime] AS dbo.GetUtcNow()";
                _framework.DbEngine.ExecuteNonQuery(sql, CommandType.Text);

                transaction.Commit();
            }

            timeService.SetDateTime(specificDateTime);

            var readyToExecuteTasks = _postponedTasks.Where(x => x.Item1 <= specificDateTime).ToArray();

            foreach (var task in readyToExecuteTasks)
            {
                task.Item2.RunSynchronously();
                _postponedTasks.Remove(task);
            }
        }

        public void AddTime(string time)
        {
            AddTime(TimeSpan.Parse(time));
        }

        public void AddTime(TimeSpan time)
        {
            var timeService = (TestTimeService)ServiceLocator.Resolve<ITimeService>();
            DateTime specificDateTime = timeService.GetUtcNow().Add(time);

            SetTime(timeService, specificDateTime);
        }

        public void MockOffset(int seconds)
        {
            var sql =@"ALTER TABLE BvTasks Drop column [CurrentUtcTime]";
            _framework.DbEngine.ExecuteNonQuery(sql, CommandType.Text);

            sql = string.Format(@"
ALTER FUNCTION [dbo].[GetUtcNow] ()
RETURNS DATETIME
WITH SCHEMABINDING
BEGIN
    RETURN DATEADD(second, {0}, GETUTCDATE())
END
", seconds);
            _framework.DbEngine.ExecuteNonQuery(sql, CommandType.Text);

            sql = @"ALTER TABLE BvTasks ADD [CurrentUtcTime] AS dbo.GetUtcNow()";
            _framework.DbEngine.ExecuteNonQuery(sql, CommandType.Text);

            ServiceLocator.RegisterInstance<ITimeService>(new TestTimeService(seconds));
        }

        public void Set(string specificDateTime)
        {
            MockDate(specificDateTime);
        }

        public void Pass(string timeSpan)
        {
            AddTime(TimeSpan.Parse(timeSpan));
        }
    }
}