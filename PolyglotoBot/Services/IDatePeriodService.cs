using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Services
{
    public interface IDatePeriodService
    {
        public List<string> GetPeriods(int countOfTheRetry, int startTime, int lastTime);
        public List<DateTime> GetPeriods(int countOfTheRetry, DateTime startTime, DateTime lastTime);
    }
}