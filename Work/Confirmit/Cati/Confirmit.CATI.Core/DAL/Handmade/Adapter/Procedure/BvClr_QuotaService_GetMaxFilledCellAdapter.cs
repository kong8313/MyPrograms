using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using System.Data;

namespace Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter
{
    public partial class BvClr_QuotaService_GetMaxFilledCellAdapter
    {
        public static CellEntity ExecuteEntity(
                int surveyID,
                int quotaId)
        {
            using (var rd = ExecuteReader(
                    surveyID,
                    quotaId))
            {
                return ReadEntity(rd);
            }
        }

        public static CellEntity ReadEntity(IDataReader rd)
        {
            int cellIdOrdinal = rd.GetOrdinal("CellId");
            int occupancyOrdinal = rd.GetOrdinal("Occupancy");

            if (rd.Read())
            {
                var cellEntity = new CellEntity();
                cellEntity.CellId = rd.GetInt32(cellIdOrdinal);
                cellEntity.Occupancy = rd.GetInt32(occupancyOrdinal);

                return cellEntity;
            }

            return null;
        }
    }
}
