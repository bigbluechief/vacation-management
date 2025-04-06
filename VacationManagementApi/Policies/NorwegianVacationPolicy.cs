using VacationManagementApi.Models;

namespace VacationManagementApi.Policies;

public class NorwegianVacationPolicy : IVacationPolicy
{

    private const int MandatorMinimumVacationDays = 25;

    public int GetTotalVacationDays(Employee employee, int year)
    {
        var extraDays = CalculateExtraDays(employee, year);
        var employeeOverrideDays = employee.VacationDaysOverride;
        var companyOverrideDays = employee.Company?.CompanyVacationPolicy?.MinimumVacationDaysOverride ?? 0;
        return Math.Max(Math.Max(employeeOverrideDays, companyOverrideDays), MandatorMinimumVacationDays) + extraDays;
    }

    private static int CalculateExtraDays(Employee employee, int year)
    {
        var extraDaysForAge = CalculateExtraDaysForAge(employee, year);
        var extraDays = extraDaysForAge;
        return extraDays;
    }

    private static int CalculateExtraDaysForAge(Employee employee, int year)
    {
        /* var extra = employee.BirthDate.AddYears(60) <= new DateOnly(year, 12, 31) ? 5 : 0; */
        var extraDaysForAge = 0;
        return extraDaysForAge;
    }

    public List<DateOnly> GetPublicVacationDays(int year)
    {
        // TODO: Implement a more robust way to get public holidays
        return year switch
        {
            2025 => [
                        new(2025, 1, 1),
                        new(2025, 4, 17),
                        new(2025, 4, 18),
                        new(2025, 4, 20),
                        new(2025, 4, 21),
                        new(2025, 5, 1),
                        new(2025, 5, 17),
                        new(2025, 5, 29),
                        new(2025, 6, 8),
                        new(2025, 6, 9),
                        new(2025, 12, 25),
                        new(2025, 12, 26)
                    ],
            2026 => [
                        new(2026, 1, 1),
                        new(2026, 4, 17),
                        new(2026, 4, 18),
                        new(2026, 4, 20),
                        new(2026, 4, 21),
                        new(2026, 5, 1),
                        new(2026, 5, 17),
                        new(2026, 5, 29),
                        new(2026, 6, 8),
                        new(2026, 6, 9),
                        new(2026, 12, 25),
                        new(2026, 12, 26)
                    ],
            _ => throw new NotImplementedException($"Public vacation days for year {year} are not added."),
        };
    }
}
