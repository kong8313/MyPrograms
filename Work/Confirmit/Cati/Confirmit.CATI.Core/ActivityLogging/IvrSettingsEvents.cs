using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;


namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class UpdateIvrSettingsEventParameters : ManagementActivityEventDetails
    {
        public int LanguageId { get; set; }
        public string LanguageDescription { get; set; }
        public string WrongInputAudioUrl { get; set; }
        public string WrongInputText { get; set; }
        public string WrongInputExitAudioUrl { get; set; }
        public string WrongInputExitText { get; set; }
    }

    [ManagementEvent(ManagementEvent.AddIvrSetting)]
    public class AddIvrSettingEvent : ManagementActivityEvent<UpdateIvrSettingsEventParameters>
    {
        public AddIvrSettingEvent(BvIvrSettingsEntity ivrSettingsEntity):
            base(ManagementEventCategory.IvrSettings, ManagementEvent.AddIvrSetting)
        {
            Details = new UpdateIvrSettingsEventParameters
            {
                LanguageId = ivrSettingsEntity.LanguageId,
                LanguageDescription = ivrSettingsEntity.LanguageDescription,
                WrongInputAudioUrl = ivrSettingsEntity.WrongInputAudioUrl,
                WrongInputText = ivrSettingsEntity.WrongInputText,
                WrongInputExitAudioUrl = ivrSettingsEntity.WrongInputExitAudioUrl,
                WrongInputExitText = ivrSettingsEntity.WrongInputExitText
            };
        }
    }

    [ManagementEvent(ManagementEvent.UpdateIvrSetting)]
    public class UpdateIvrSettingEvent : ManagementActivityEvent<UpdateIvrSettingsEventParameters>
    {
        public UpdateIvrSettingEvent(int prevLanguageId, BvIvrSettingsEntity ivrSettingsEntity):
            base(ManagementEventCategory.IvrSettings, ManagementEvent.UpdateIvrSetting)
        {
            ObjectId = prevLanguageId;
            Details = new UpdateIvrSettingsEventParameters
            {
                LanguageId = ivrSettingsEntity.LanguageId,
                LanguageDescription = ivrSettingsEntity.LanguageDescription,
                WrongInputAudioUrl = ivrSettingsEntity.WrongInputAudioUrl,
                WrongInputText = ivrSettingsEntity.WrongInputText,
                WrongInputExitAudioUrl = ivrSettingsEntity.WrongInputExitAudioUrl,
                WrongInputExitText = ivrSettingsEntity.WrongInputExitText
            };
        }
    }

    [Serializable]
    public class DeleteIvrSettingsEventParameters : ManagementActivityEventDetails
    {
        public string DeletedIds { get; set; }
    }

    [ManagementEvent(ManagementEvent.DeleteIvrSettings)]
    public class DeleteIvrSettingsEvent : ManagementActivityEvent<DeleteIvrSettingsEventParameters>
    {
        public DeleteIvrSettingsEvent(string deletedIds):
            base(ManagementEventCategory.IvrSettings, ManagementEvent.DeleteIvrSettings)
        {
            Details = new DeleteIvrSettingsEventParameters { DeletedIds = deletedIds };
        }
    }
}