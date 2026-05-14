using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class TimeBreakService : ITimeBreakService
    {
        private readonly IBreakTypeRepository _breakTypeRepository;

        public TimeBreakService(IBreakTypeRepository breakTypeRepository)
        {
            _breakTypeRepository = breakTypeRepository;
        }

        public string GetBreakTypeName(int? breakTypeId)
        {
            BvBreakTypeEntity breakType = null;

            if (breakTypeId.HasValue)
            {
                breakType = _breakTypeRepository.TryGetById(breakTypeId.Value);
            }

            return breakType == null ? string.Empty : breakType.Name;
        }
    }
}