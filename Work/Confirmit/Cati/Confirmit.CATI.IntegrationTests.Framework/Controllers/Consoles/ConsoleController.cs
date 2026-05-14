using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles
{
    public class ConsoleController
    {
        public string StationId { get; set; }


        public DiallerInfo DialerInfo { get; set; }
        public PersonInfo PersonInfo { get; set; }
        public CatiConsolePropertiesContainer Properties { get; set; }

        public TestDataContext Context { get; set; }
        public PersonController Person { get; set; }
        public SurveyController Survey { get; set; }
        public DialerController Dialer { get; set; }

        public State State { get; set; }
        public InterviewController Interview { get; set; }

        public CatiWsHelper Services;

        public static ConsoleController Create(PersonController person)
        {
            return new ConsoleController
            {
                Context = person.Context,
                Person = person
            };
        }
    }
}