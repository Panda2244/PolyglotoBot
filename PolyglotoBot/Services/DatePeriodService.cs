using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Services
{
    public class DatePeriodService : IDatePeriodService
    {
        public List<string> GetPeriods(int countOfTheRetry, int startTime, int lastTime)
        {
            var periods = new List<string>();
            int firstTime = startTime * 60;
            int secondTime = lastTime * 60;
            int timeInterval = secondTime - firstTime;
            int stepMinute = timeInterval / countOfTheRetry;

            for (; firstTime < secondTime; firstTime += stepMinute)
            {
                periods.Add($"{firstTime / 60}:{firstTime % 60}");
            }
            return periods;
        }
        public List<DateTime> GetPeriods(int countOfTheRetry, DateTime startTime, DateTime lastTime)
        {
            var periods = new List<DateTime>();

            int firstTime = startTime.TimeOfDay.Hours * 60;
            int secondTime = lastTime.TimeOfDay.Hours * 60;
            int timeInterval = secondTime - firstTime;
            int stepMinute = timeInterval / countOfTheRetry;

            for (; firstTime < secondTime; firstTime += stepMinute)
            {
                periods.Add(new DateTime(DateTime.Now.Year, 2, 4, firstTime / 60, firstTime % 60, 0));
            }

            return periods;
        }
    }
}