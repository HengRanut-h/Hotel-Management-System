using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;

namespace HotelManagement.Application.Common.Security;

public static class CurrentUserExtensions
{
    public static Guid RequireHotelId(
        this ICurrentUser currentUser) =>
        currentUser.HotelId
        ?? throw new ForbiddenException(
            "The current account is not assigned to a hotel.");

    public static Guid RequireBranchId(
        this ICurrentUser currentUser) =>
        currentUser.BranchId
        ?? throw new ForbiddenException(
            "The current account is not assigned to a branch.");
}