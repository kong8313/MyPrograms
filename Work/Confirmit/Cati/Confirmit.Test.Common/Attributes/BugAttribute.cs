using System;

namespace Confirmit.Test.Common.Attributes
{
    /// <summary>
    /// Should be used in the tests when test created for the bug.
    /// Usefull when we neeed find test for the specific bug.
    /// </summary>
    public class BugAttribute : Attribute
    {
        public int BugId { get; set; }

        public BugAttribute(int bugId)
        {
            BugId = bugId;
        }
    }
}
