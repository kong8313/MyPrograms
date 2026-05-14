using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using Confirmit.CATI.Backend.WebApiServices.Filters;
using Confirmit.CATI.Backend.WebApiServices.Logging;
using Confirmit.CATI.Backend.WebApiServices.Middleware;
using Confirmit.CATI.Common.ServiceLocation;
using Owin;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Routing.Conventions;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.SystemSettings;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using Swashbuckle.Application;
using Swashbuckle.OData;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // By default in the code first approach EF creates database, so, need to avoid it...
            Database.SetInitializer(new DoNotCreateDatabase<DatabaseContext>());

            // Configure Web API for self-host. 
            var config = new HttpConfiguration
            {
                
                DependencyResolver = new DependencyResolver(ServiceLocator.CreateChildContainer())
            };
            
            if (ServiceLocator.Resolve<IWebApiSettings>().EnableSwagger)
            {
                SetupSwagger(config);
            }

            config.Filters.Add(new AuthorizationFilter());
            config.Filters.Add(new RestApiMonitorFilter());
            config.Filters.Add(new ExceptionsFilter(new ExceptionLogger()));
            
            config.MessageHandlers.Add((DelegatingHandler)config.DependencyResolver.GetService(typeof(IRestApiMonitorHandler)));
            
            
            
            config.Routes.MapHttpRoute(
                "Root",
                "",
                new { controller = "Root", action = "GetOperations" } );
            
            config.Routes.MapHttpRoute(
                "healthzReady",
                "healthz/ready",
                new { controller = "Healthz", action = "Ready" } );

            config.Routes.MapHttpRoute(
                "healthzLive",
                "healthz/live",
                new { controller = "Healthz", action = "Live" });
            
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<CallHistory>("callhistory");
            builder.EntitySet<BreakHistory>("breakhistory");
            builder.EntitySet<InterviewerSessionHistory>("interviewersessionhistory");
            builder.EntitySet<Interviewer>("interviewers");
            builder.EntitySet<Group>("groups");
            builder.EntitySet<SurveyAssignment>("surveyassignments");
            builder.EntitySet<ResourceAssignment>("resourceassignment");
            builder.EntitySet<InterviewerProperties>("interviewerproperties");
            builder.EntitySet<CallHistoryWithVariables>("callhistorywithvariables");
            builder.EntitySet<TelephoneBlacklistItem>("blacklist");

            FunctionConfiguration f;

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            builder.EntitySet<Survey>("surveys");

            builder.Namespace = "Controller";

            builder.EntityType<TelephoneBlacklistItem>().Collection.Action("Import").Parameter<TelephoneBlacklistItems>("BlackListItems");

            builder.EntityType<Survey>().Function("GetBasicProperties").Returns<SurveyBasicProperties>();

            f = builder.EntityType<Survey>().Function("PutBasicProperties").Returns<bool>();
            f.Parameter<string>("properties");

            f = builder.EntityType<Survey>().Function("GetAssignments").ReturnsCollectionFromEntitySet<ResourceAssignment>("resourceassignment");
            f.Parameter<int>("callCenterId");

            builder.EntityType<Survey>().Function("Open").Returns<bool>();
            builder.EntityType<Survey>().Function("Close").Returns<bool>();
            builder.EntityType<Survey>().Function("Shutdown").Returns<bool>();
            builder.EntityType<Survey>().Function("CleanAssignments").Returns<bool>();

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            builder.EntityType<Interviewer>().Function("GetGroups").ReturnsCollectionFromEntitySet<Group>("groups");

            builder.EntityType<Interviewer>().Function("GetAssignments").ReturnsCollectionFromEntitySet<SurveyAssignment>("surveyassignments");

            f = builder.EntityType<Interviewer>().Function("AssignOnSurvey").Returns<bool>();
            f.Parameter<string>("surveyId");

            f = builder.EntityType<Interviewer>().Function("DeAssignFromSurvey").Returns<bool>();
            f.Parameter<string>("surveyId");

            f = builder.EntityType<Interviewer>().Function("AssignOnCall").Returns<bool>();
            f.Parameter<string>("surveyId");
            f.Parameter<int>("interviewId");

            f = builder.EntityType<Interviewer>().Function("DeAssignFromCalls").Returns<bool>();
            f.Parameter<string>("surveyId");

            builder.EntityType<Interviewer>().Function("CleanAssignments").Returns<bool>();

            builder.EntityType<Interviewer>().Function("Lock").Returns<bool>();

            builder.EntityType<Interviewer>().Function("Unlock").Returns<bool>();

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            f = builder.EntityType<Group>().Function("GetInterviewers").ReturnsCollectionFromEntitySet<Interviewer>("interviewers");
            f.Parameter<int>("callCenterId");

            f = builder.EntityType<Group>().Function("GetAssignments").ReturnsCollectionFromEntitySet<SurveyAssignment>("surveyassignments");
            f.Parameter<int>("callCenterId");

            f = builder.EntityType<Group>().Function("AssignOnSurvey").Returns<bool>();
            f.Parameter<string>("surveyId");
            f.Parameter<int>("callCenterId");

            f = builder.EntityType<Group>().Function("DeAssignFromSurvey").Returns<bool>();
            f.Parameter<string>("surveyId");
            f.Parameter<int>("callCenterId");

            f = builder.EntityType<Group>().Function("AssignOnCall").Returns<bool>();
            f.Parameter<string>("surveyId");
            f.Parameter<int>("interviewId");
            f.Parameter<int>("callCenterId");

            f = builder.EntityType<Group>().Function("DeAssignFromCalls").Returns<bool>();
            f.Parameter<string>("surveyId");
            f.Parameter<int>("callCenterId");

            builder.EnumType<SurveyState>().Namespace = "Confirmit.CATI.Backend.WebApiServices.Models";
            builder.EnumType<AssignmentListMode>().Namespace = "Confirmit.CATI.Backend.WebApiServices.Models";
            builder.EnumType<DialType>().Namespace = "Confirmit.CATI.Backend.WebApiServices.Models";
            builder.EnumType<TaskChoiceMode>().Namespace = "Confirmit.CATI.Backend.WebApiServices.Models";
            
            builder.EnumType<Confirmit.CATI.Common.TaskChoicePermissions>().Namespace = "Confirmit.CATI.Common";
            builder.EnumType<Confirmit.CATI.Common.AssignmentType>().Namespace = "<Confirmit.CATI.Common";
            builder.EnumType<Confirmit.CATI.Common.CallDeliveryMode>().Namespace = "<Confirmit.CATI.Common";
            builder.EnumType<Confirmit.CATI.Common.BlacklistPatternType>().Namespace = "<Confirmit.CATI.Common";
            
            var routingConventions = ODataRoutingConventions
                .CreateDefaultWithAttributeRouting("OData", config)
                .AsEnumerable();

            config.MapODataServiceRoute(
                routeName: "OData",
                routePrefix: null,
                configureAction: containerBuilder => containerBuilder
                    .AddService(ServiceLifetime.Singleton, sp => builder.GetEdmModel())
                    .AddService(ServiceLifetime.Singleton, sp => routingConventions)
                    .AddService(ServiceLifetime.Singleton, typeof(ODataUriResolver),
                        sp => new StringAsEnumResolver { EnableCaseInsensitive = true })
            );

            appBuilder.Use<CommonRequestProcessingLogicMiddleware>(
                config.DependencyResolver.GetService(typeof(IRequestInfo)));
            appBuilder.Use<RateLimitingMiddleware>();

            appBuilder.UseWebApi(config);
        }

        private void SetupSwagger(HttpConfiguration config)
        {
            config.EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "The CATI API")
                        .Description("The API enables you to use CATI features")
                        .Contact(x => x.Name("CATI Team"));

                    c.RootUrl(x =>
                        x.RequestUri.GetLeftPart(UriPartial.Authority) +
                        x.GetRequestContext().VirtualPathRoot.TrimEnd('/'));

                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        @"Confirmit.CATI.Backend.xml");
                    c.IncludeXmlComments(path);

                    c.OperationFilter<ODataOptionDocumentFilter>();
                    c.DescribeAllEnumsAsStrings();

                    c.CustomProvider(defaultProvider => new ODataSwaggerProvider(defaultProvider, c, config));
                })
                .EnableSwaggerUi(c => c.DisableValidator());
        }
    }
}
