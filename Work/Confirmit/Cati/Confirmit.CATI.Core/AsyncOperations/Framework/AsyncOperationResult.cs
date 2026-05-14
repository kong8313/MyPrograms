using System;
using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationResult
    {
        public int Id { get; set; }
        public List<Exception> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public AsyncOperationState State { get; set; }
        public int ProcessedItemsCount { get; set; }
        public int FailedItemsCount { get; set; }

        public AsyncOperationResult()
        {
            Errors = new List<Exception>();
            Warnings = new List<string>();
        }
        public override string ToString()
        {
            return $"State: {State}; " +
                   $"Processed: {ProcessedItemsCount}; " +
                   $"Failed: {FailedItemsCount}; " +
                   (Warnings != null && Warnings.Count > 0
                       ? $"Warnings: {string.Join(", ", Warnings)}; "
                       : "") +
                   (Errors != null && Errors.Count > 0
                       ? $"Errors: {string.Join(", ", Errors.Select(e => e.Message))}; "
                       : "");
        }
    }
}
