using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure
{
    public interface ILookupCallEntity
    {
        int? CallId { get; set; }
        int? SurveyId { get; set; }
        int? InterviewId { get; set; }
        int? ActiveDialId { get; set; }
    }
}
