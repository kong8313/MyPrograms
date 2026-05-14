using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using System.Data;

namespace Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter
{
    public partial class BvClr_QuotaService_GetMinFilledCellsAdapter
    {
        public static List<CellEntity> ExecuteEntityList(
                int surveyID,
                int quotaId,
                int count)
        {
            using (var rd = ExecuteReader(
                    surveyID,
                    quotaId,
                    count))
            {
                return ReadList(rd);
            }
        }

        public static List<CellEntity> ReadList(IDataReader rd)
        {
            var BvClr_QuotaService_GetMinFilledCellsEntityList = new List<CellEntity>();

            int cellIdOrdinal = rd.GetOrdinal("CellId");
            int occupancyOrdinal = rd.GetOrdinal("Occupancy");

            while (true)
            {
                bool isRead = rd.Read();

                if (isRead == false)
                    break;

                var entity = new CellEntity();

                entity.CellId = rd.GetInt32(cellIdOrdinal);
                entity.Occupancy = rd.GetInt32(occupancyOrdinal);

                BvClr_QuotaService_GetMinFilledCellsEntityList.Add(entity);
            }

            return BvClr_QuotaService_GetMinFilledCellsEntityList;
        }
    }
}
