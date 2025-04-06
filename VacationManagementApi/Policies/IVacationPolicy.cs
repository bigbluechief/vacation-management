using VacationManagementApi.Models;

namespace VacationManagementApi.Policies;

public interface IVacationPolicy
{
    int GetTotalVacationDays(Employee employee, int year);
    List<DateOnly> GetPublicVacationDays(int year);
}
