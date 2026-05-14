using System;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
    /// <summary>
    /// Represents common interface of objects which can be verified.
    /// </summary>
    public interface IVerifiable
    {
        /// <summary>
        /// Verifies object for correctness. Validation errors are returned in
        /// validation error collection.
        /// </summary>
        /// <param name="errors">Collection of validation error.</param>
        /// <returns>true, if object is in valid state; otherwise false.</returns>
        bool Validate(out ErrorCollection errors);
    }
}
