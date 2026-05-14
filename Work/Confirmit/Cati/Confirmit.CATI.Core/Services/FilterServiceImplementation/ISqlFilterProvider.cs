using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation
{
    public interface ISqlFilterProvider
    {
        List<BvFilterFieldsEntity> GetFields(int filterId);
        SqlFilter GetFilter(int filterId, int surveyId);
        SqlFilter TryToGetFilter(int? filterId, int surveyId);
    }
}