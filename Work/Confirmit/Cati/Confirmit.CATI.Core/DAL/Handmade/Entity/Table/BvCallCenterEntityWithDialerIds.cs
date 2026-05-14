using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.DAL.Handmade.Entity.Table
{
    public class BvCallCenterEntityWithDialerIds : BvCallCenterEntity
    {
        public int[] DialerIds { get; set; }
        public string DialerIdsText
        {
            get
            {
                return string.Join(" ", DialerIds);
            }
        }

        public BvCallCenterEntityWithDialerIds(BvCallCenterEntity bvCallCenter, int[] dialerIds)
        {
            ID = bvCallCenter.ID;
            Name = bvCallCenter.Name;
            Description = bvCallCenter.Description;
            IsDefault = bvCallCenter.IsDefault;
            CanBeDeleted = bvCallCenter.CanBeDeleted;
            LocalTimezoneId = bvCallCenter.LocalTimezoneId;
            DialerId = bvCallCenter.DialerId;
            HidePii = bvCallCenter.HidePii;
            DialerIds = dialerIds;
        }

        public BvCallCenterEntityWithDialerIds(): base()
        {
            DialerIds = new int[0];
        }
    }
}
