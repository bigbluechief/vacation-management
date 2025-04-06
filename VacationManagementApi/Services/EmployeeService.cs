using VacationManagementApi.Data;
using VacationManagementApi.Enums;
using VacationManagementApi.Policies;
using Microsoft.EntityFrameworkCore;
using VacationManagementApi.Utils;
using VacationManagementApi.Models;

namespace VacationManagementApi.Services;

public class EmployeeService(VacationDbContext db)
{
    private readonly VacationDbContext _db = db;

    public int GetEntitledVacationDays(Employee employee, int year)
    {
        var policy = VacationPolicyFactory.For(employee);
        return policy.GetTotalVacationDays(employee, year);
    }

    public async Task<Employee?> GetEmployeeAsync(int employeeId)
    {
        return await _db.Employees
            .Include(e => e.Company)
                .ThenInclude(c => c.CompanyVacationPolicy)
            .Include(e => e.VacationRequests)
            .Include(e => e.VacationBalances)
            .FirstOrDefaultAsync(e => e.Id == employeeId);
    }

    public async Task<(
        VacationBalanceError error,
        List<VacationBalance>? balances)>
        GetVacationBalancesForEmployeeByYearAsync(int employeeId, int year)
    {
        var employee = await _db.Employees
            .Include(e => e.Company)
            .Include(e => e.VacationBalances)
            .FirstOrDefaultAsync(e => e.Id == employeeId);

        if (employee == null)
            return (VacationBalanceError.EmployeeNotFound, null);

        var balance = employee.VacationBalances.FirstOrDefault(b => b.Year == year);

        if (balance == null)
        {
            var policy = VacationPolicyFactory.For(employee);
            var entitlement = policy.GetTotalVacationDays(employee, year);

            balance = new VacationBalance
            {
                EmployeeId = employee.Id,
                Year = year,
                DaysUsed = 0,
                DaysRemaining = entitlement
            };
        }

        return (VacationBalanceError.None, new List<VacationBalance> { balance });
    }

    public async Task<(
        VacationBalanceError error,
        List<VacationBalance>? balances)>
        GetVacationBalancesForEmployeesByYearAsync(List<int> employeeIds, int year)
    {
        if (employeeIds == null || employeeIds.Count == 0)
            return (VacationBalanceError.InvalidEmployeeIds, null);

        var employees = await _db.Employees
            .Include(e => e.Company)
                .ThenInclude(c => c.CompanyVacationPolicy)
            .Include(e => e.VacationBalances)
            .Where(e => employeeIds.Contains(e.Id))
            .ToListAsync();

        if (employees.Count == 0)
            return (VacationBalanceError.EmployeesNotFound, null);

        var resultBalances = new List<VacationBalance>();

        foreach (var employee in employees)
        {
            var balance = employee.VacationBalances.FirstOrDefault(b => b.Year == year);
            if (balance == null)
            {
                var policy = VacationPolicyFactory.For(employee);
                var entitlement = policy.GetTotalVacationDays(employee, year);

                balance = new VacationBalance
                {
                    EmployeeId = employee.Id,
                    Year = year,
                    DaysUsed = 0,
                    DaysRemaining = entitlement
                };
            }

            resultBalances.Add(balance);
        }

        return (VacationBalanceError.None, resultBalances);
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _db.Employees
            .Include(e => e.Company)
            .Include(vr => vr.VacationRequests)
            .Include(e => e.VacationBalances)
            .FirstOrDefaultAsync(e => e.Id == id);

        return employee;
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        var employees = await _db.Employees
             .Include(e => e.Company)
             .Include(e => e.VacationBalances)
             .Include(e => e.VacationRequests)
             .ToListAsync();

        return employees;
    }


}
