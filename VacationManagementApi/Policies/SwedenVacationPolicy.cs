using VacationManagementApi.Models;

namespace VacationManagementApi.Policies;

public class SwedenVacationPolicy : IVacationPolicy
{

    private const int MandatorMinimumVacationDays = 30;

    public int GetTotalVacationDays(Employee employee, int year)
    {
        var employeeOverrideDays = employee.VacationDaysOverride;
        var companyOverrideDays = employee.Company?.CompanyVacationPolicy?.MinimumVacationDaysOverride ?? 0;
        return Math.Max(Math.Max(employeeOverrideDays, companyOverrideDays), MandatorMinimumVacationDays);
    }

    public List<DateOnly> GetPublicVacationDays(int year)
    {
        // TODO: Implement a more robust way to get public holidays
        return year switch
        {
            2025 => [
                new(2025, 1, 1),
                new(2025, 1, 6),
                new(2025, 4, 18),
                new(2025, 4, 20),
                new(2025, 4, 21),
                new(2025, 5, 1),
                new(2025, 5, 29),
                new(2025, 6, 6),
                new(2025, 6, 8),
                new(2025, 6, 20),
                new(2025, 6, 21),
                new(2025, 11, 1),
                new(2025, 12, 25),
                new(2025, 12, 26)
            ],
            2026 => [
                new(2026, 1, 1),
                new(2026, 1, 6),
                new(2026, 4, 3),
                new(2026, 4, 5),
                new(2026, 4, 6),
                new(2026, 5, 1),
                new(2026, 5, 14),
                new(2026, 5, 24),
                new(2026, 6, 6),
                new(2026, 6, 19),
                new(2026, 6, 20),
                new(2026, 10, 31),
                new(2026, 12, 25),
                new(2026, 12, 26)
            ],
            _ => throw new NotImplementedException($"Public vacation days for year {year} are not added."),
        };
    }
}
