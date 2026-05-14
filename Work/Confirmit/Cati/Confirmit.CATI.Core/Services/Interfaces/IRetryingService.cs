using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IRetryingService
    {
        void Retry(string description, Action action);
        T Retry<T>(string description, Func<T> action);

        void Retry(int countOfAttemt, string description, Action action);
        T Retry<T>(int countOfAttemt, string description, Func<T> action);
    }
}
