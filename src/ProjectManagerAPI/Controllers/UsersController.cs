using Application.DTOs.Users;
using Application.Exceptions;
using Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProjectManagerAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUserRepository userRepository, IProjectRepository projectRepository) : ControllerBase
{
    [HttpGet("me/stats")]
    public async Task<ActionResult<UserStatsResponse>> GetUserStats(CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();

        var user = await userRepository.GetById(actorUserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var projectCount = await projectRepository.CountByUser(actorUserId, cancellationToken);

        return Ok(new UserStatsResponse(projectCount, user.ProjectLimit, user.Plan));
    }

    private Guid GetActorUserId()
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(nameIdentifier, out var actorUserId))
        {
            throw new UnauthorizedException("User token is missing a valid identifier claim.");
        }

        return actorUserId;
    }
}
