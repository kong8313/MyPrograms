using System;
using System.IO;
using System.Web;
using System.Web.Hosting;

using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.IpLockDown;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes;
using Confirmit.CATI.Core.ServiceLocationExtention;
using Confirmit.CATI.Core.SystemSettings;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.ServiceLocation
{
    public partial interface IDependencyBase1
    {
        string Method1();
    }

    public interface IDependencyBase2
    {
        string Method2();
    }

    public interface IDependencyObject : IDependencyBase1, IDependencyBase2
    {
    }

    public partial class DependencyObject : IDependencyObject
    {
        public string Method1()
        {
            return "DependencyObject.Method1";
        }

        public string Method2()
        {
            return "DependencyObject.Method2";
        }
    }

    public interface IMainObject
    {
        IDependencyObject GetDependencyObject();
    }

    public class MainObject : IMainObject
    {
        private readonly IDependencyObject dependencyObject;

        public MainObject(IDependencyObject dependencyObject)
        {
            this.dependencyObject = dependencyObject;
        }

        public IDependencyObject GetDependencyObject()
        {
            return dependencyObject;
        }
    }
    
    /// <summary>
    /// Used to simulate an HttpRequest.
    /// </summary>
    public class SimulatedHttpRequest : SimpleWorkerRequest
    {
        private readonly string host;

        public SimulatedHttpRequest(
            string appVirtualDir, string appPhysicalDir, string page, string query, TextWriter output, string host)
            : base(appVirtualDir, appPhysicalDir, page, query, output)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException("host", @"Host cannot be null nor empty.");
            }

            this.host = host;
        }

        public static void SetHttpContextWithSimulatedRequest(string host, string application)
        {
            string appVirtualDir = "/";
            string appPhysicalDir = @"c:\projects\SubtextSystem\Subtext.Web\";
            string page = application.Replace("/", string.Empty) + "/default.aspx";
            string query = string.Empty;
            TextWriter output = null;

            var workerRequest = new SimulatedHttpRequest(
                appVirtualDir, appPhysicalDir, page, query, output, host);
            HttpContext.Current = new HttpContext(workerRequest);
        }

        public override string GetServerName()
        {
            return this.host;
        }

        public override string MapPath(string virtualPath)
        {
            return Path.Combine(this.GetAppPath(), virtualPath);
        }
    }

    [TestClass]
    public class HttpContextTests
    {
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Resolve_ComplexObject_InterfaceIsResolved()
        {
            SimulatedHttpRequest.SetHttpContextWithSimulatedRequest("MyHost", "MyBlog");

            var serviceLocator = new ServiceLocator();
            try
            {
                serviceLocator.Cleanup();
                serviceLocator.Initialize();

                serviceLocator.RegisterSingletonPerHttpContext<IDependencyObject, DependencyObject>();
                serviceLocator.RegisterSingletonPerHttpContext<IDependencyBase1, DependencyObject>();
                serviceLocator.RegisterSingletonPerHttpContext<IMainObject, MainObject>();

                var main = ServiceLocator.Resolve<IMainObject>();

            }
            finally
            {
                serviceLocator.Cleanup();
            }
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Resolve_SystemSettings_InterfaceIsResolved()
        {
            SimulatedHttpRequest.SetHttpContextWithSimulatedRequest("MyHost", "MyBlog");

            var serviceLocator = new ServiceLocator();

            try
            {
                serviceLocator.Cleanup();
                serviceLocator.Initialize();

                new SystemSettingSupervisorRegistrator().RegisterTypes(serviceLocator);
                new IpLockDownRegistry().RegisterTypes(serviceLocator);
                ServiceLocator.Register<ISqlTableUpdatedPublisher, StubISqlTableUpdatedPublisher>();
                
                var main = ServiceLocator.Resolve<ISystemSettings>();

            }
            finally
            {
                serviceLocator.Cleanup();
            }
        }
    }
}
