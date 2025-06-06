using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Enums;
using Domain.Entities.Invitations;
using Domain.Entities.Profiles;
using Domain.Entities.Users;
using Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IApplicationDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<AuthResponse>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken
    )
    {
        User? user = await _context
            .Users.Include(u => u.EmployeeProfile)
            .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, email), cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return Result.Failure<AuthResponse>(
                new Error("Auth.InvalidCredentials", "Credenciales inválidas", ErrorType.Validation)
            );
        }

        string token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            User = MapToUserDto(user),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
        };
    }

    public async Task<Result<AuthResponse>> RegisterEmployeeAsync(
        RegisterEmployeeRequest request,
        CancellationToken cancellationToken
    )
    {
        if (
            await _context.Users.AnyAsync(
                u => EF.Functions.ILike(u.Email, request.Email),
                cancellationToken
            )
        )
        {
            return Result.Failure<AuthResponse>(
                new Error("Auth.EmailExists", "El email ya está registrado", ErrorType.Validation)
            );
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.Employee,
            ProfilePictureUrl = null,
        };

        var profile = new EmployeeProfile
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FirstName = string.Empty,
            LastName = string.Empty,
            Availability = false,
            Country = string.Empty,
            Timezone = string.Empty,
            SfiaLevelGeneral = 1,
            Mbti = string.Empty,
            Languages = [],
            SpecializedRoles = [],
            Technologies = [],
            WorkExperiences = [],
            PersonalInterests = [],
            VerificationStatus = VerificationStatus.NotRequested,
            VerificationNotes = null,
        };

        _context.Users.Add(user);
        _context.EmployeeProfiles.Add(profile);
        await _context.SaveChangesAsync(cancellationToken);

        string token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            User = MapToUserDto(user),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
        };
    }

    public async Task<Result<AuthResponse>> RegisterManagerAsync(
        RegisterManagerRequest request,
        string invitationToken,
        CancellationToken cancellationToken
    )
    {
        InvitationLink? invitation = await _context.InvitationLinks.FirstOrDefaultAsync(
            i => i.Token == invitationToken && !i.IsUsed && i.ExpiresAt > DateTime.UtcNow,
            cancellationToken
        );

        if (invitation == null)
        {
            return Result.Failure<AuthResponse>(
                new Error(
                    "Auth.InvalidInvitation",
                    "Token de invitación inválido o expirado",
                    ErrorType.Validation
                )
            );
        }

        if (!invitation.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<AuthResponse>(
                new Error(
                    "Auth.EmailMismatch",
                    "El email no coincide con la invitación",
                    ErrorType.Validation
                )
            );
        }

        if (
            await _context.Users.AnyAsync(
                u => EF.Functions.ILike(u.Email, request.Email),
                cancellationToken
            )
        )
        {
            return Result.Failure<AuthResponse>(
                new Error("Auth.EmailExists", "El email ya está registrado", ErrorType.Validation)
            );
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = invitation.TargetRole,
            ProfilePictureUrl = null,
        };

        invitation.IsUsed = true;
        invitation.UsedAt = DateTime.UtcNow;

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        string token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            User = MapToUserDto(user),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
        };
    }

    public async Task<Result<string>> CreateInvitationLinkAsync(
        Guid adminId,
        string email,
        Role targetRole,
        CancellationToken cancellationToken
    )
    {
        User? admin = await _context.Users.FindAsync(
            new object?[] { adminId },
            cancellationToken: cancellationToken
        );
        if (admin?.Role != Role.Admin)
        {
            return Result.Failure<string>(
                new Error(
                    "Auth.Unauthorized",
                    "Solo los administradores pueden crear invitaciones",
                    ErrorType.Forbidden
                )
            );
        }

        string invitationToken = Guid.NewGuid().ToString("N");

        var invitation = new InvitationLink
        {
            Id = Guid.NewGuid(),
            Token = invitationToken,
            CreatedById = adminId,
            TargetRole = targetRole,
            Email = email,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedBy = null,
        };

        _context.InvitationLinks.Add(invitation);
        await _context.SaveChangesAsync(cancellationToken);

        return invitationToken;
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        string roleName = Enum.GetName(user.Role) ?? "Employee";

        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, roleName),
            new Claim("userId", user.Id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role,
            HasProfile = user.EmployeeProfile != null,
            IsProfileVerified =
                user.EmployeeProfile?.VerificationStatus == VerificationStatus.Approved,
        };
    }

    public async Task<Result<bool>> ValidateInvitationTokenAsync(
        string token,
        CancellationToken cancellationToken
    )
    {
        InvitationLink? invitation = await _context.InvitationLinks.FirstOrDefaultAsync(
            i => i.Token == token && !i.IsUsed && i.ExpiresAt > DateTime.UtcNow,
            cancellationToken
        );

        return invitation != null
            ? Result.Success(true)
            : Result.Failure<bool>(
                new Error(
                    "Auth.InvalidInvitation",
                    "Token de invitación inválido o expirado",
                    ErrorType.Validation
                )
            );
    }
}
