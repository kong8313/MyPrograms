using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;


namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class ImportTelephoneNumbersEventParameters : ManagementActivityEventDetails
    {
        public int TotalNumbersCount { get; set; }
        public int ValidNumbersCount { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ImportTelephoneNumbersToBlacklist)]
    public class ImportTelephoneNumbersToBlacklistEvent : ManagementActivityEvent<ImportTelephoneNumbersEventParameters>
    {
        public ImportTelephoneNumbersToBlacklistEvent(int totalNumbersCount):
            base(ManagementEventCategory.Blacklist, ManagementEvent.ImportTelephoneNumbersToBlacklist)
        {
            Details = new ImportTelephoneNumbersEventParameters
            {
                TotalNumbersCount = totalNumbersCount
            };
        }
    }

    [ManagementEventAttribute(ManagementEvent.AddTelephoneNumberToBlacklist)]
    public class AddTelephoneNumberToBlacklistEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public AddTelephoneNumberToBlacklistEvent(string number):
            base(ManagementEventCategory.Blacklist, ManagementEvent.AddTelephoneNumberToBlacklist)
        {
            ObjectName = number;
        }
    }

    [ManagementEventAttribute(ManagementEvent.UpdateTelephoneNumberInBlacklist)]
    public class UpdateTelephoneNumberInBlacklistEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public UpdateTelephoneNumberInBlacklistEvent(int id, string number):
            base(ManagementEventCategory.Blacklist, ManagementEvent.UpdateTelephoneNumberInBlacklist)
        {
            ObjectId = id;
            ObjectName = number;
        }
    }

    [Serializable]
    public class DeleteTelephoneNumbersFromBlacklistEventParameters : ManagementActivityEventDetails
    {
        public int DeletedNumbersCount { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteTelephoneNumbersFromBlacklist)]
    public class DeleteTelephoneNumbersFromBlacklistEvent : ManagementActivityEvent<DeleteTelephoneNumbersFromBlacklistEventParameters>
    {
        public DeleteTelephoneNumbersFromBlacklistEvent(int deletedNumbersCount):
            base(ManagementEventCategory.Blacklist, ManagementEvent.DeleteTelephoneNumbersFromBlacklist)
        {
            Details = new DeleteTelephoneNumbersFromBlacklistEventParameters { DeletedNumbersCount = deletedNumbersCount };
        }
    }

    [Serializable]
    public class ExportTelephoneNumbersFromBlacklistEventParameters : ManagementActivityEventDetails
    {
        public int ExportedNumbersCount { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ExportTelephoneNumbersFromBlacklist)]
    public class ExportTelephoneNumbersFromBlacklistEvent : ManagementActivityEvent<ExportTelephoneNumbersFromBlacklistEventParameters>
    {
        public ExportTelephoneNumbersFromBlacklistEvent(int exportedNumbersCount):
            base(ManagementEventCategory.Blacklist, ManagementEvent.ExportTelephoneNumbersFromBlacklist)
        {
            Details = new ExportTelephoneNumbersFromBlacklistEventParameters
            {
                ExportedNumbersCount = exportedNumbersCount
            };
        }
    }
}