using Microsoft.AspNetCore.Authorization;

namespace HotelManagement.Infrastructure.Authorization;

public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;