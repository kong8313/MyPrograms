using System;

namespace SurgeryHelper.Entities
{
    /// <summary>
    /// Класс с информацией по типам анестезий
    /// </summary>
    public class AnesthesiaTypesClass : MedicalClass
    {
        public AnesthesiaTypesClass()
        {
        }

        public AnesthesiaTypesClass(AnesthesiaTypesClass anesthesiaTypesInfo)
        {
            Id = anesthesiaTypesInfo.Id;
            LastNameWithInitials = anesthesiaTypesInfo.LastNameWithInitials;
        }

        public static int Compare(AnesthesiaTypesClass anesthesiaTypesInfo1, AnesthesiaTypesClass anesthesiaTypesInfo2)
        {
            return string.Compare(anesthesiaTypesInfo1.LastNameWithInitials, anesthesiaTypesInfo2.LastNameWithInitials, StringComparison.InvariantCulture);
        }
    }
}
