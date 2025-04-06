using VacationManagementApi.Models;

namespace VacationManagementApi.Policies;

public class UsaVacationPolicy : IVacationPolicy
{

    private const int MandatorMinimumVacationDays = 0;

    public int GetTotalVacationDays(Employee employee, int year)
    {
        var employeeOverrideDays = employee.VacationDaysOverride;
        var companyOverrideDays = employee.Company?.CompanyVacationPolicy?.MinimumVacationDaysOverride ?? 0;
        return Math.Max(Math.Max(employeeOverrideDays, companyOverrideDays), MandatorMinimumVacationDays);
    }

    public List<DateOnly> GetPublicVacationDays(int year)
    {
        // TODO: Implement a way to add Company recognized vacation days. Commonly used in USA.
        return [];
    }
}
