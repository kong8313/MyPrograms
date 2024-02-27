using System;

namespace SurgeryHelper.Entities
{
    /// <summary>
    /// Класс с информацией по имплантату
    /// </summary>
    public class ImplantClass : MedicalClass
    {
        public ImplantClass()
        {
        }

        public ImplantClass(ImplantClass implantInfo)
        {
            Id = implantInfo.Id;
            LastNameWithInitials = implantInfo.LastNameWithInitials;
        }

        public static int Compare(ImplantClass implantInfo1, ImplantClass implantInfo2)
        {
            return string.Compare(implantInfo1.LastNameWithInitials, implantInfo2.LastNameWithInitials, StringComparison.InvariantCulture);
        }
    }
}
