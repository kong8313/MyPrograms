using System;

namespace Confirmit.Test.Common.Attributes
{
    /// <summary>
    /// Should be used in the tests when test emulates the multi user environment.
    /// For example execution of the methods in parallel threads.
    /// </summary>
    public class MultiUserTestAttribute : Attribute
    {
    }
}
