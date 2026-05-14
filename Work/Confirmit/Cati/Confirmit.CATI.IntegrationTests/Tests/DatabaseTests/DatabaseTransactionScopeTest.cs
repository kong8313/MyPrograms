using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Common;
using System.Data;
using System.Threading;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.IntegrationTests.Tests.ReportsTests.Tools;
using System.Data.SqlClient;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseTests
{
    [TestClass]
    public class DatabaseTransactionScopeTest : BaseMockedIntegrationTest
    {
        private IPersonRepository _personRepository;

        private static BvPersonEntity GetPersonEntity( string name )
        {
            return new BvPersonEntity
            {
                Name = name,
                ManualSelection = 0,
                Description = "",
                CallCenterID = CallCenterTools.DefaultId
            };
        }

        public override void OnPostTestInitialize()
        {
            _personRepository = ServiceLocator.Resolve<IPersonRepository>();
        }

        [TestMethod, Owner(@"FIRM\AlexeyN"), Bug(41472)]
        public void UseDatabaseTransactionScopeWithDifferentDeadlockPriority_ExecuteQueries_Success()
        {
            using ( var transaction = new DatabaseTransactionScope( "IntegrationTests.InsertPerson1", DeadlockPriority.Supervisor ) )
            {
                _personRepository.Insert(GetPersonEntity("person1"));

                transaction.Commit();
            }

            using ( var transaction = new DatabaseTransactionScope( "IntegrationTests.InsertPerson2", DeadlockPriority.SchedulingProcedure ) )
            {
                _personRepository.Insert(GetPersonEntity("person2"));

                transaction.Commit();
            }

            using ( var transaction = new DatabaseTransactionScope( "IntegrationTests.InsertPerson3", DeadlockPriority.PeriodicalThread ) )
            {
                _personRepository.Insert(GetPersonEntity("person3"));

                transaction.Commit();
            }

            using ( var transaction = new DatabaseTransactionScope( "IntegrationTests.InsertPerson4", DeadlockPriority.Normal ) )
            {
                _personRepository.Insert(GetPersonEntity("person4"));

                transaction.Commit();
            }
        }

        private void AlterSPToGenerateDeadlock()
        {
            var db = new DatabaseEngine();

            //create tables
            string query = @"
create table ta(a1 int, a2 int)
create table tb(b1 int, b2 int)

insert into ta values(1, 1)
insert into tb values(1, 1)
";
            db.ExecuteNonQuery(query, CommandType.Text);

            //change SP BvSpLogin_SpinUp. This SP will work with tables
            //created above. Adapter for this sp will be the same
            query = @"
alter proc BvSpLogin_SpinUp
   @PersonSID int
as
    if @PersonSID = 3
       insert into ta values(2, 2)
    if @PersonSID = 2
       insert into tb values(2, 2)
    if @PersonSID = 1
       delete from ta
    if @PersonSID = 0
       delete from tb
";
            db.ExecuteNonQuery(query, CommandType.Text);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Bug(47713)]
        public void DeadlockPriorityInfluenceOnVictim_DeadlockIsOccured_VictimIsTransactionWithLowPriority()
        {
            AlterSPToGenerateDeadlock();
            var isExceptionWasThrown1 = new Deferred<bool>();
            var isExceptionWasThrown2 = new Deferred<bool>();

            var firstResetEvent = new ManualResetEvent(false);
            var secondResetEvent = new ManualResetEvent(false);

            //should start first in result make more changes than second thread.
            //if both thread will have the same deadlock priority than second thread should be aborted
            var t1 = new Thread(
                x =>
                    {
                        try
                        {
                            using (var d = new DatabaseTransactionScope("Aborted", (DeadlockPriority)(-4)))
                            {
                                BvSpLogin_SpinUpAdapter.ExecuteNonQuery(2);
                                firstResetEvent.Set();
                                secondResetEvent.WaitOne();
                                BvSpLogin_SpinUpAdapter.ExecuteNonQuery(1);
                                d.Commit();
                            }
                        }
                        catch(SqlException)
                        {
                            var isExceptionWasThrown = x as Deferred<bool>;
                            isExceptionWasThrown.Value = true;
                        }
                    });

            var t2 = new Thread(
                x =>
                {
                    try
                    {
                        firstResetEvent.WaitOne();
                        using (var d = new DatabaseTransactionScope("NNN", (DeadlockPriority)(4)))
                        {
                            BvSpLogin_SpinUpAdapter.ExecuteNonQuery(3);
                            secondResetEvent.Set();
                            Thread.Sleep(5);
                            BvSpLogin_SpinUpAdapter.ExecuteNonQuery(0);
                            d.Commit();
                        }
                    }
                    catch (SqlException)
                    {
                        var isExceptionWasThrown = x as Deferred<bool>;
                        isExceptionWasThrown.Value = true;
                    }
                });

            t1.Start(isExceptionWasThrown1);
            t2.Start(isExceptionWasThrown2);

            t1.Join();
            t2.Join();

            Assert.IsFalse(isExceptionWasThrown2.Value, "Process with high deadlock priority should not be aborted");
            Assert.IsTrue(isExceptionWasThrown1.Value, "Process with low deadlock priority should be aborted");
        }
    }
}
