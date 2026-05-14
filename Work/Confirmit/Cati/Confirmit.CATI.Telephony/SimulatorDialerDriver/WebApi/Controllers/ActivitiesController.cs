using System;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using SimulatorDialerDriver.Distribution;
using SimulatorDialerDriver.WebApi.Models;
using SimulatorDialerDriverClass = Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver;

namespace SimulatorDialerDriver.WebApi.Controllers
{
    [RoutePrefix("activities")]
    public class ActivitiesController : ApiController
    {
        public class Activity
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Owner { get; set; }
            public ContextInfo Context { get; set; }
            public string[] Commands { get; set; }
        }


        [HttpGet]
        [Route("")]
        public Activity[] GetAll()
        {
            try
            {
                return SimulatorDialerDriverClass.Instance.Activities.Select( x =>
                    new Activity
                    {
                        Id = x.Value.Id,
                        Name = x.Value.Name,
                        Owner = x.Value.Owner,
                        Context = x.Value.Context,
                        Commands = x.Value.Commands.Keys.ToArray()
                    }).ToArray();
            }
            catch(Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "ActivitiesController.GetAll",
                    ex.ToString());
                throw;
            }
        }

        [HttpPost]
        [Route("{id}")]
        public void Execute(string id, string command, string args)
        {
            try
            {
                SimulatorDialerDriverClass.Instance.Activities.Execute(id, command, args);
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    $"ActivitiesController.Execute(id='{id}', command='{command}', args='{args}'",
                    ex.ToString());
                throw;
            }
        }
    }
}
