namespace VacationManagementApi.Utils;

public static class VacationDayCalculator
{
    public static int CalculateEffectiveDays(
        DateOnly start,
        DateOnly end,
        List<DateOnly> publicVacationDays,
        bool weekendCountsAsVacation)
    {
        int days = 0;

        /**
         * Calculate number of effective vacation days. Does this by looping through days from the start date to the end date
         * and checking if the day is a public vacation. Some companies may have a policy that weekends do not count as vacation days,
         * so these are excluded if applicable.
        */
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            bool isWeekend = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
            bool isPublicHoliday = publicVacationDays.Contains(date);

            if (isPublicHoliday || (isWeekend && !weekendCountsAsVacation))
                continue;

            days++;
        }

        return days;
    }
}
