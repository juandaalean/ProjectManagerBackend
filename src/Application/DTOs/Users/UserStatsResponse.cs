namespace Application.DTOs.Users;

public record UserStatsResponse(
    int ProjectCount,
    int ProjectLimit,
    string Plan
);
