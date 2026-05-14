using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Inbound
{
    [TestClass]
    public class InboundTelephoneNumberRepositoryTest
    {
        

        const string DdiTelephoneNumber = "12345678901234567890";

        private IInboundTelephoneNumberRepository _inboundTelephoneNumberRepository;
        private IntegrationTestingFramework _framework;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework = IntegrationTestingFramework.Instance;
            _framework.TestInitialize();
            _inboundTelephoneNumberRepository = ServiceLocator.Resolve<InboundTelephoneNumberRepository>();
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void Insert_InsertNewDdiNumberToDatabase_NumberIsAddedCorrectly()
        {
            var context = new TestData
            {                
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Surveys = new[] 
                {
                    new SurveyData
                    {
                        Tag = "S1"
                    }
                }
            }.Create();

            var expectedEntity = new BvInboundTelephoneNumberEntity
            {
                SurveyId = context.GetSurvey("S1").Id,
                DialerId = context.GetDialer("D1").Id,
                TelephoneNumber = DdiTelephoneNumber,
                AudioMessagesJson = "<xml/>"
            };

            _inboundTelephoneNumberRepository.Insert(expectedEntity);

            var actualEntity = _inboundTelephoneNumberRepository.TryGetByTelephoneNumber(DdiTelephoneNumber);

            Assert.IsNotNull(actualEntity);
            Assert.AreEqual(expectedEntity.TelephoneNumber, actualEntity.TelephoneNumber);
            Assert.AreEqual(expectedEntity.DialerId, actualEntity.DialerId);
            Assert.AreEqual(expectedEntity.SurveyId, actualEntity.SurveyId);
            Assert.AreEqual(expectedEntity.AudioMessagesJson, actualEntity.AudioMessagesJson);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void Delete_RemoveOneFromThreeDdiNumbers_TwoNumbersAreRemain()
        {
            var context = new TestData
            {
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "1"
                            },
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "2"
                            },
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "3"
                            }
                        }
                    }
                }
            }.Create();

            _inboundTelephoneNumberRepository.Delete(new[] { DdiTelephoneNumber + "2" });

            var actualDdiNumbers = _inboundTelephoneNumberRepository.GetByTelephoneNumbers(new[] { DdiTelephoneNumber + "1", DdiTelephoneNumber + "3" });

            Assert.AreEqual(2, actualDdiNumbers.Count);
            Assert.AreEqual(DdiTelephoneNumber + "1", actualDdiNumbers[0].TelephoneNumber);
            Assert.AreEqual(DdiTelephoneNumber + "3", actualDdiNumbers[1].TelephoneNumber);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void Delete_RemoveTwoFromThreeDdiNumbers_OnlyOneNumberIsRemain()
        {
            var context = new TestData
            {
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "1"
                            },
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "2"
                            },
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "3"
                            }
                        }
                    }
                }
            }.Create();

            _inboundTelephoneNumberRepository.Delete(new[] { DdiTelephoneNumber + "1", DdiTelephoneNumber + "3" });

            var actualDdiNumbers = _inboundTelephoneNumberRepository.GetBySurveyId(context.GetSurvey("S1").Id);

            Assert.AreEqual(1, actualDdiNumbers.Count);
            Assert.AreEqual(DdiTelephoneNumber + "2", actualDdiNumbers[0].TelephoneNumber);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void Update_AddTwoDdiNumbersAndUpdateOne_CorrectNumberIsUpdated()
        {
            var context = new TestData
            {
                Dialers = new[] 
                {
                    new DialerData { Tag = "D1" },
                    new DialerData { Tag = "D2" }
                },
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "1"
                            },
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "2"
                            }
                        }
                    },
                    new SurveyData
                    {
                        Tag = "S2"
                    }
                }
            }.Create();

            var updatedEntity = new BvInboundTelephoneNumberEntity
            {
                SurveyId = context.GetSurvey("S2").Id,
                DialerId = context.GetDialer("D2").Id,
                TelephoneNumber = DdiTelephoneNumber + "2",
                AudioMessagesJson = "<xml changed=\"true\"/>"
            };

            _inboundTelephoneNumberRepository.Update(updatedEntity);

            var actualDdiNumbers = _inboundTelephoneNumberRepository.GetByTelephoneNumbers(new[] { DdiTelephoneNumber + "1", DdiTelephoneNumber + "2" });

            Assert.AreEqual(2, actualDdiNumbers.Count);

            Assert.AreEqual(DdiTelephoneNumber + "1", actualDdiNumbers[0].TelephoneNumber);
            Assert.AreEqual(context.GetDialer("D1").Id, actualDdiNumbers[0].DialerId);
            Assert.AreEqual(context.GetSurvey("S1").Id, actualDdiNumbers[0].SurveyId);
            Assert.AreEqual(null, actualDdiNumbers[0].AudioMessagesJson);

            Assert.AreEqual(updatedEntity.TelephoneNumber, actualDdiNumbers[1].TelephoneNumber);
            Assert.AreEqual(updatedEntity.DialerId, actualDdiNumbers[1].DialerId);
            Assert.AreEqual(updatedEntity.SurveyId, actualDdiNumbers[1].SurveyId);
            Assert.AreEqual(updatedEntity.AudioMessagesJson, actualDdiNumbers[1].AudioMessagesJson);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetValidByDialerId_AddTwoDdiNumbersToNormalSurveyAndTwoDdiNumbersToSoftDeletedSurveyWithTheSameDialer_OnlyDdiNumbersFromNormalSurveyAreReturned()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData { Tag = "D1" }
                },
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "1"
                            },
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "2"
                            }
                        }
                    },
                    new SurveyData
                    {
                        Tag = "S2",
                        IsSoftDeleted = true,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "3"
                            },
                            new InboundTelephoneNumberData
                            {
                                Dialer = "D1",
                                TelephoneNumber = DdiTelephoneNumber + "4"
                            }
                        }
                    }
                }
            }.Create();

            var actualDdiNumbers = _inboundTelephoneNumberRepository.GetValidByDialerId(context.GetDialer("D1").Id);

            Assert.AreEqual(2, actualDdiNumbers.Count);

            Assert.AreEqual(DdiTelephoneNumber + "1", actualDdiNumbers[0].TelephoneNumber);
            Assert.AreEqual(DdiTelephoneNumber + "2", actualDdiNumbers[1].TelephoneNumber);
        }
    }
}
