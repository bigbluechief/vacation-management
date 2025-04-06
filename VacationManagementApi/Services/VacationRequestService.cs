using VacationManagementApi.Data;
using VacationManagementApi.Enums;
using Microsoft.EntityFrameworkCore;
using VacationManagementApi.Models;
using VacationManagementApi.Policies;
using VacationManagementApi.Utils;

namespace VacationManagementApi.Services;

public class VacationRequestService(VacationDbContext db, EmployeeService employeeService)
{
    private readonly VacationDbContext _db = db;
    private readonly EmployeeService _employeeService = employeeService;

    public async Task<List<VacationRequest>> SearchVacationRequests(
        int year,
        int? month = null,
        HashSet<string>? departments = null,
        HashSet<string>? roles = null,
        HashSet<VacationRequestStatus>? vacationRequestStatuses = null,
        bool ExcludeInactiveEmployees = true)
    {
        var builder = new VacationRequestQueryBuilder(_db)
            .WithYear(year)
            .WithMonth(month)
            .WithDepartments(departments)
            .WithRoles(roles)
            .WithStatuses(vacationRequestStatuses)
            .ExcludeInactiveEmployees(ExcludeInactiveEmployees);

        return await builder.Build().ToListAsync();
    }

    public async Task<VacationRequest?> GetVacationRequestByIdAsync(int id)
    {
        var vacationRequest = await _db.VacationRequests
            .Include(v => v.Employee)
            .Include(a => a.Administrator)
            .FirstOrDefaultAsync(v => v.Id == id);

        return vacationRequest;
    }

    public async Task<RegisterVacationRequestError>
        RegisterVacationRequestAsync(VacationRequest request)
    {
        Console.WriteLine($"Registering vacation request for employee {request.EmployeeId}");
        var employee = await _employeeService.GetEmployeeAsync(request.EmployeeId);
        if (employee == null)
        {
            return RegisterVacationRequestError.EmployeeNotFound;
        }

        if (request.StartDate.Year != request.EndDate.Year)
        {
            return RegisterVacationRequestError.InvalidDateRange;
        }

        var policy = VacationPolicyFactory.For(employee);
        int year = request.StartDate.Year;

        Console.WriteLine($"Employee {employee.Id} has {employee.VacationBalances.Count} vacation balances.");

        int requestedEffectiveDays = VacationDayCalculator.CalculateEffectiveDays(
            request.StartDate,
            request.EndDate,
            policy.GetPublicVacationDays(year),
            employee.Company?.CompanyVacationPolicy?.WeekendCountsAsVacation ?? false
        );

        Console.WriteLine($"Requested effective days: {requestedEffectiveDays}");

        int used = employee.VacationBalances.FirstOrDefault(vb => vb.Year == year)?.DaysUsed ?? 0;

        Console.WriteLine($"Used vacation days: {used}");

        var pendingRequests = await _db.VacationRequests
            .Where(v => v.EmployeeId == employee.Id &&
                        v.Status == VacationRequestStatus.Pending &&
                        v.StartDate.Year == year)
            .ToListAsync();

        Console.WriteLine($"Pending vacation requests: {pendingRequests.Count}");

        int pendingDays = pendingRequests.Sum(p => VacationDayCalculator.CalculateEffectiveDays(
            p.StartDate,
            p.EndDate,
            policy.GetPublicVacationDays(year),
            employee.Company?.CompanyVacationPolicy?.WeekendCountsAsVacation ?? false
        ));

        Console.WriteLine($"Pending vacation days: {pendingDays}");

        int entitlement = policy.GetTotalVacationDays(employee, year);

        Console.WriteLine($"Entitlement: {entitlement}");

        if (used + pendingDays + requestedEffectiveDays > entitlement)
        {
            return RegisterVacationRequestError.ExceedsEntitlement;
        }

        _db.VacationRequests.Add(request);
        await _db.SaveChangesAsync();

        return RegisterVacationRequestError.None;
    }

    public async Task<(
        ApproveVacationRequestError error,
        VacationRequest? vacationRequest)>
        ApproveVacationRequestAsync(int requestId, int approverId)
    {
        var vacationRequest = await _db.VacationRequests
            .Include(v => v.Employee)
                .ThenInclude(e => e.Company)
            .Include(v => v.Employee.VacationBalances)
            .Include(v => v.Administrator)
            .FirstOrDefaultAsync(v => v.Id == requestId);

        if (vacationRequest == null)
        {
            return (ApproveVacationRequestError.VacationRequestNotFound, null);
        }

        if (vacationRequest.Status != VacationRequestStatus.Pending)
        {
            return (ApproveVacationRequestError.VacationRequestNotPending, vacationRequest);
        }

        var employee = vacationRequest.Employee!;
        var policy = VacationPolicyFactory.For(employee);
        int year = vacationRequest.StartDate.Year;

        int requestedDays = VacationDayCalculator.CalculateEffectiveDays(
            vacationRequest.StartDate,
            vacationRequest.EndDate,
            policy.GetPublicVacationDays(year),
            employee.Company?.CompanyVacationPolicy?.WeekendCountsAsVacation ?? false
        );

        var balance = employee.VacationBalances.FirstOrDefault(b => b.Year == year);
        if (balance == null)
        {
            balance = new VacationBalance
            {
                EmployeeId = employee.Id,
                Year = year,
                DaysUsed = 0,
                DaysRemaining = 0
            };
            _db.VacationBalances.Add(balance);
        }

        int totalUsed = balance.DaysUsed + requestedDays;
        int entitlement = policy.GetTotalVacationDays(employee, year);

        if (totalUsed > entitlement)
        {
            return (ApproveVacationRequestError.ExceedsEntitlement, vacationRequest);
        }

        balance.DaysUsed += requestedDays;
        balance.DaysRemaining = entitlement - balance.DaysUsed;

        vacationRequest.Status = VacationRequestStatus.Approved;
        vacationRequest.ApproverAdminId = approverId;

        await _db.SaveChangesAsync();
        return (ApproveVacationRequestError.None, vacationRequest);
    }

    public async Task<(
        RejectVacationRequestError error,
        VacationRequest? vacationRequest)>
        RejectVacationRequestAsync(int requestId, int approverId)
    {
        var vacationRequest = await _db.VacationRequests
            .Include(v => v.Employee)
            .FirstOrDefaultAsync(v => v.Id == requestId);

        if (vacationRequest == null)
            return (RejectVacationRequestError.VacationRequestNotFound, null);

        if (vacationRequest.Status != VacationRequestStatus.Pending)
        {
            return (RejectVacationRequestError.VacationRequestNotPending, vacationRequest);
        }

        // EmailService.SendEmail(
        //     vacationRequest.Employee.Email,
        //     "Vacation Request Rejected",
        //     $"Your vacation request from {vacationRequest.StartDate} to {vacationRequest.EndDate} has been rejected."
        // );

        _db.Remove(vacationRequest);

        await _db.SaveChangesAsync();

        return (RejectVacationRequestError.None, vacationRequest);
    }

    private class VacationRequestQueryBuilder(VacationDbContext db)
    {
        private IQueryable<VacationRequest> _query = db.VacationRequests
                .Include(v => v.Employee)
                    .ThenInclude(e => e.Company)
                .Include(v => v.Employee.VacationBalances)
                .AsQueryable();

        public VacationRequestQueryBuilder WithYear(int year)
        {
            _query = _query.Where(v => v.StartDate.Year == year);
            return this;
        }

        public VacationRequestQueryBuilder WithMonth(int? month)
        {
            if (month.HasValue)
            {
                _query = _query.Where(v => v.StartDate.Month <= month.Value && v.EndDate.Month >= month.Value);
            }
            return this;
        }

        public VacationRequestQueryBuilder WithDepartments(HashSet<string>? departments)
        {
            if (departments != null && departments.Count > 0)
            {
                _query = _query.Where(v => departments.Contains(v.Employee.Department));
            }
            return this;
        }

        public VacationRequestQueryBuilder WithRoles(HashSet<string>? roles)
        {
            if (roles != null && roles.Count > 0)
            {
                _query = _query.Where(v => roles.Contains(v.Employee.Role));
            }
            return this;
        }

        public VacationRequestQueryBuilder WithStatuses(HashSet<VacationRequestStatus>? statuses)
        {
            if (statuses != null && statuses.Count > 0)
            {
                _query = _query.Where(v => statuses.Contains(v.Status));
            }
            return this;
        }

        public VacationRequestQueryBuilder ExcludeInactiveEmployees(bool exclude)
        {
            if (exclude)
            {
                _query = _query.Where(v => v.Employee.Status == EmployeeStatus.Active);
            }
            return this;
        }

        public IQueryable<VacationRequest> Build()
        {
            return _query;
        }
    }

}
