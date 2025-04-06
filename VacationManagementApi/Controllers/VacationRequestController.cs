using Microsoft.AspNetCore.Mvc;
using VacationManagementApi.Services;
using VacationManagementApi.Dtos;
using VacationManagementApi.Models;
using Microsoft.Extensions.Localization;
using VacationManagementApi.Enums;

namespace VacationManagementApi.Controllers;

[ApiController]
[Route("[controller]")]
public class VacationRequestController(
    VacationRequestService vacationRequestService,
     IStringLocalizer<VacationRequestController> localizer) : ControllerBase
{

    private readonly VacationRequestService _vacationRequestService = vacationRequestService;
    private readonly IStringLocalizer<VacationRequestController> _localizer = localizer;

    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<VacationRequest>>> SearchVacationReqeusts(SearchVacationRequestDto dto)
    {
        var vacationRequests = await _vacationRequestService.SearchVacationRequests(
            dto.Year,
            dto.Month,
            dto.Departments,
            dto.Roles,
            dto.Statuses,
            dto.ExcludeInactiveEmployees
        );
        return Ok(vacationRequests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VacationRequest>> GetVacationRequestById(int id)
    {
        var vacationRequest = await _vacationRequestService.GetVacationRequestByIdAsync(id);

        if (vacationRequest == null)
            return NotFound(_localizer["VacationRequestNotFound", id].Value);

        return Ok(vacationRequest);
    }

    [HttpPost]
    public async Task<ActionResult<VacationRequest>> Create(CreateVacationRequestDto dto)
    {
        var request = new VacationRequest
        {
            EmployeeId = dto.EmployeeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Note = dto.Note,
            ApproverAdminId = dto.ApproverAdminId,
            Status = VacationRequestStatus.Pending
        };

        var error = await _vacationRequestService.RegisterVacationRequestAsync(request);

        if (error != RegisterVacationRequestError.None)
        {
            return error switch
            {
                RegisterVacationRequestError.EmployeeNotFound => NotFound(_localizer["EmployeeNotFound", dto.EmployeeId].Value),
                RegisterVacationRequestError.InvalidDateRange => BadRequest(_localizer["InvalidDateRange"].Value),
                RegisterVacationRequestError.ExceedsEntitlement => Conflict(_localizer["ExceedsEntitlement"].Value),
                _ => StatusCode(500, _localizer["UnexpectedErrorOccured"].Value)
            };
        }

        return CreatedAtAction(nameof(GetVacationRequestById),
            new { id = request.EmployeeId, requestId = request.Id },
            request);
    }

    [HttpPost("{id}/approve")]
    public async Task<ActionResult<VacationRequest>> ApproveVacationRequest(int id, ApproveVacationRequestDto dto)
    {
        var (error, vacationRequest) = await _vacationRequestService.ApproveVacationRequestAsync(id, dto.AdministratorId);

        if (error != ApproveVacationRequestError.None)
        {
            return error switch
            {
                ApproveVacationRequestError.VacationRequestNotFound => NotFound(_localizer["VacationRequestNotFound", id].Value),
                ApproveVacationRequestError.VacationRequestNotPending => BadRequest(_localizer["VacationRequestNotPending", id].Value),
                ApproveVacationRequestError.ExceedsEntitlement => Conflict(_localizer["VacationRequestExceedsEntitlement", id].Value),
                _ => StatusCode(500, _localizer["UnexpectedErrorOccured"].Value)
            };
        }

        return Ok(vacationRequest);
    }

    [HttpPost("{id}/reject")]
    public async Task<ActionResult<VacationRequest>> RejectVacationRequest(int id, RejectVacationRequestDto dto)
    {
        var (error, vacationRequest) = await _vacationRequestService.RejectVacationRequestAsync(id, dto.AdministratorId);

        if (error != RejectVacationRequestError.None)
        {
            return error switch
            {
                RejectVacationRequestError.VacationRequestNotFound => NotFound(_localizer["VacationRequestNotFound", id].Value),
                RejectVacationRequestError.VacationRequestNotPending => BadRequest(_localizer["VacationRequestNotPending", id].Value),
                _ => StatusCode(500, _localizer["UnexpectedErrorOccured"].Value)
            };
        }

        return Ok(vacationRequest);
    }

}
