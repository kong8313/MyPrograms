using System;

namespace Confirmit.CATI.Common.Contracts.ErrorReportingService
{
    [Serializable]
    public class ErrorMessage
    {
        public ErrorMessage(int companyId, string message)
        {
            CompanyId = companyId;
            Message = message;
        }

        public int CompanyId { get; set; }

        public string Message { get; set; }


    }
}
