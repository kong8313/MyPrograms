using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Sockets;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Health;

namespace Confirmit.CATI.Common.UnitTests
{
    [TestClass]
    public class HealthStateExceptionHandlerTest
    {
        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void TwoConsecutiveSocketErrors_HealthCheckHandlerNotSetToUnhealthy()
        {
            HealthCheckHandler.SetHealthy();

            HealthStateExceptionHandler<ArgumentNullException>.OnException(new ArgumentNullException());
            HealthStateExceptionHandler<ArgumentNullException>.OnException(new ArgumentNullException());

            Assert.IsTrue(HealthCheckHandler.IsHealthy());
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void ThreeConsecutiveSocketErrors_HealthCheckHandlerSetToUnhealthy()
        {
            HealthCheckHandler.SetHealthy();

            HealthStateExceptionHandler<SocketException>.OnException(new SocketException());
            HealthStateExceptionHandler<SocketException>.OnException(new SocketException());
            HealthStateExceptionHandler<SocketException>.OnException(new SocketException());

            Assert.IsFalse(HealthCheckHandler.IsHealthy());
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void ThreeConsecutiveNonSocketErrors_HealthCheckHandlerNotSetToUnhealthy()
        {
            HealthCheckHandler.SetHealthy();

            HealthStateExceptionHandler<NotImplementedException>.OnException(new Exception());
            HealthStateExceptionHandler<NotImplementedException>.OnException(new Exception());
            HealthStateExceptionHandler<NotImplementedException>.OnException(new Exception());

            Assert.IsTrue(HealthCheckHandler.IsHealthy());
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void ThreeConsecutiveInnerSocketErrors_HealthCheckHandlerSetToUnhealthy()
        {
            HealthCheckHandler.SetHealthy();

            HealthStateExceptionHandler<ArgumentException>.OnException(new Exception("", new ArgumentException()));
            HealthStateExceptionHandler<ArgumentException>.OnException(new Exception("", new Exception("", new ArgumentException())));
            HealthStateExceptionHandler<ArgumentException>.OnException(new Exception(
                "", new Exception(
                    "", new Exception(
                        "", new ArgumentException())
                    )
                )
            );

            Assert.IsFalse(HealthCheckHandler.IsHealthy());
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void TwoHealthStateExceptionHandlerDoesNotAffectEachOther()
        {
            HealthCheckHandler.SetHealthy();

            HealthStateExceptionHandler<Exception>.OnException(new Exception());
            HealthStateExceptionHandler<ApplicationException>.OnException(new ApplicationException());
            HealthStateExceptionHandler<Exception>.OnException(new Exception());

            Assert.IsTrue(HealthCheckHandler.IsHealthy());

            HealthStateExceptionHandler<Exception>.OnException(new Exception());
            Assert.IsFalse(HealthCheckHandler.IsHealthy());
        }
    }
}
