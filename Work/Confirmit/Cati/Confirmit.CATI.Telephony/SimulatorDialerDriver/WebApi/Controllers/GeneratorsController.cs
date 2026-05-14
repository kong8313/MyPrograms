using System;
using System.Linq;
using System.Web.Http;
using SimulatorDialerDriver.Distribution;
using SimulatorDialerDriverClass = Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver;

namespace SimulatorDialerDriver.WebApi.Controllers
{
    [RoutePrefix("generators")]
    public class GeneratorsController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IGenerator[] GetAll()
        {
            try
            {
                return Generators.All.ToArray();
            }
            catch(Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "GeneratorsController.GetAll",
                    ex.ToString());
                throw;
            }
        }

        [HttpGet]
        [Route("{name}")]
        public IGenerator Get(string name)
        {
            try
            {
                return Generators.All.Single(x => x.Name == name);
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "GeneratorsController.Get",
                    ex.ToString());
                throw;
            }
        }

        [HttpPost]
        [Route("{name}/behaviors")]
        public void AddBehavior(string name,  GeneratorBehavior behavior)
        {
            try
            {
                behavior.Id = Guid.NewGuid().ToString();
                var generator = Generators.All.Single(x => x.Name == name);
                generator.Behaviors.Add(behavior);
                generator.Behaviors = generator.Behaviors.OrderByDescending(x => x.Filter?.Priority ?? 0).ToList();
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "GeneratorsController.AddBehavior",
                    ex.ToString());
                throw;
            }
        }

        [HttpDelete]
        [Route("{name}/behaviors/{id}")]
        public void DeleteBehavior(string name, string id)
        {
            try
            {
                var generator = Generators.All.Single(x => x.Name == name);
                var behavior = generator.Behaviors.Single(x => x.Id == id);
                generator.Behaviors.Remove(behavior);
            }
            catch (Exception ex)
            {
                SimulatorDialerDriverClass.Instance.Logger.Error(
                    "GeneratorsController.DeleteBehavior",
                    ex.ToString());
                throw;
            }
        }
    }
}
