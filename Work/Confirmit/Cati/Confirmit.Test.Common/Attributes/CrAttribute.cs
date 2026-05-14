using System;

namespace Confirmit.Test.Common.Attributes
{
    /// <summary>
    /// Should be used in the tests when test created for the CR.
    /// Usefull when we neeed find test for the specific CR.
    /// </summary>
    public class CrAttribute : Attribute
    {
        public int CrId { get; set; }

        public CrAttribute(int crId)
        {
            CrId = crId;
        }
    }
}
