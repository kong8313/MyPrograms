using System;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Backend.WebApiServices.Services
{
    public class BlackListHelper
    {
        public static BvTelephoneBlacklistEntity GetBvTelephoneBlacklistEntity(TelephoneBlacklistItem item)
        {
            if (!IsTelephoneBlackListItemValid(item))
            {
                return null;
            }

            return new BvTelephoneBlacklistEntity
            {
                TelephoneNumber = item.TelephoneNumber,
                Type = (byte)item.Type
            };
        }

        public static bool IsTelephoneBlackListItemValid(TelephoneBlacklistItem item)
        {
            return item.TelephoneNumber.All(char.IsDigit) && item.TelephoneNumber.Length < 255 &&
                   Enum.IsDefined(typeof(BlacklistPatternType), item.Type);
        }
    }
}
