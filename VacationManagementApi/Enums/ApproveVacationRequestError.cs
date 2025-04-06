namespace VacationManagementApi.Enums;

public enum ApproveVacationRequestError
{
    None,
    VacationRequestNotFound,
    VacationRequestNotPending,
    ExceedsEntitlement
}
