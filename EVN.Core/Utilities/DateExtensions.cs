using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Utilities
{
    public static class DateExtensions
    {
        /// <summary>
        /// Ngày âm lịch
        /// </summary>
        /// <param name="dateTime">Ngày cần convert</param>
        /// <returns></returns>
        public static DateLunarModel GetLunarDate(DateTime dateTime)
        {
            ChineseLunisolarCalendar chinese = new ChineseLunisolarCalendar();
            GregorianCalendar gregorian = new GregorianCalendar();


            // Get Chinese New Year of current UTC date/time
            DateTime chineseNewYear = chinese.ToDateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);

            // Convert back to Gregorian (you could just query properties of `chineseNewYear` directly, but I prefer to use `GregorianCalendar` for consistency:
            DateLunarModel result = new DateLunarModel();
            result.Year = gregorian.GetYear(chineseNewYear);
            result.Month = gregorian.GetMonth(chineseNewYear);
            result.Day = gregorian.GetDayOfMonth(chineseNewYear);

            return result;
        }

        /// <summary>
        /// Lấy ngày cuối tuần
        /// </summary>
        /// <param name="startDate">Từ ngày</param>
        /// <param name="endDate">Đến ngày</param>
        /// <returns></returns>
        public static List<DateTime> GetWeekendDates(DateTime startDate, DateTime endDate)
        {
            List<DateTime> weekendList = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                    weekendList.Add(date.Date);
            }

            return weekendList;
        }

    }

    public class DateLunarModel
    {
        public int Year { set; get; }
        public int Month { set; get; }
        public int Day { set; get; }

    }
}
