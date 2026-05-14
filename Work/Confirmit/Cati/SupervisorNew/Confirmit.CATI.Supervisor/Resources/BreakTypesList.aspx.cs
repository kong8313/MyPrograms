using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.ServerControls;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class BreakTypesList : BaseForm
    {
        private readonly IBreakTypeRepository _breakTypeRepository;

        public override string TopTitle => Strings.BreakTypes;

        public BreakTypesList()
        {
            _breakTypeRepository = ServiceLocator.Resolve<IBreakTypeRepository>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_grid.GetPage += GetPage;
            var column = m_grid.Columns.FromKey("Type") as GeneralGridColumn;

            column.Items.Add(new ListItem(Strings.Paid));
            column.Items.Add(new ListItem(Strings.Unpaid));
        }

        private object GetPage(out int totalCount)
        {
            List<BvBreakTypeEntity> bvBreakEntities = _breakTypeRepository.GetAll();

            var models = bvBreakEntities.Select(entity =>
            {
                return new GridBreakModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    Type = entity.IsPaid ? Strings.Paid : Strings.Unpaid,
                    YellowThreshold = (int?)TimeService.ConvertSecToMin(entity.YellowThreshold),
                    RedThreshold = (int?)TimeService.ConvertSecToMin(entity.RedThreshold)
                };
            });

            return BaseMethods.GetPage(models, m_grid.PageArguments, out totalCount);
        }

        protected void DeleteBreakTypes(object sender, EventArgs e)
        {
            if (m_grid.SelectedKeys.Length == 0)
            {
                return;
            }

            try
            {
                var keys = m_grid.SelectedKeys.Select(x => Convert.ToInt32(x)).ToList();

                _breakTypeRepository.Delete(keys);

                m_grid.ClearSelectedKeys();
            }
            catch (UserMessageException)
            {
                Context.AddError(new UserMessageException(Strings.WarningAtLeastOneBreakShouldBeLeft));
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected class GridBreakModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public int? YellowThreshold { get; set; }
            public int? RedThreshold { get; set; }
        }
    }
}