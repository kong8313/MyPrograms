using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Core.Confirmit
{
    /// <summary>
    /// Represent information about confirmit question with type and ability to mark this question checked/unchecked.    
    /// </summary>    
    [Serializable]
    public class SearchableFieldOrderedForSelection : BvSearchableFieldsOrderedEntity
    {
        public string DisplayName { get; set; }
        public string FieldType { get; set; }

        public SearchableFieldOrderedForSelection(BvSearchableFieldsOrderedEntity entity, string fieldType)
        {
            FieldName = entity.FieldName;
            SurveyId = entity.SurveyId;
            IsSystem = entity.IsSystem;
            IsEnabled = entity.IsEnabled;
            OrderNumber = entity.OrderNumber;
            
            DisplayName = entity.FieldName;
            FieldType = fieldType ?? "System";
        }
    }
}