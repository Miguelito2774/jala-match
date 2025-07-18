using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Enums;
using Domain.Entities.Invitations;
using Domain.Entities.Privacy;
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
    private readonly INotificationService _notificationService;

    public AuthService(
        IApplicationDbContext context,
        IOptions<JwtSettings> jwtSettings,
        INotificationService notificationService
    )
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
        _notificationService = notificationService;
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

        // Create default privacy consent for new user
        var privacyConsent = new UserPrivacyConsent
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TeamMatchingAnalysis = true,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            Version = "1.0"
        };

        _context.Users.Add(user);
        _context.EmployeeProfiles.Add(profile);
        _context.UserPrivacyConsents.Add(privacyConsent);
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

        // Create default privacy consent for new manager
        var privacyConsent = new UserPrivacyConsent
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TeamMatchingAnalysis = false, // Managers typically don't participate in team matching
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            Version = "1.0"
        };

        invitation.IsUsed = true;
        invitation.UsedAt = DateTime.UtcNow;

        _context.Users.Add(user);
        _context.UserPrivacyConsents.Add(privacyConsent);
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

        string invitationLink = $"http://localhost:3000/register?token={invitationToken}";

        await _notificationService.SendInvitationNotificationAsync(
            email,
            email.Split('@')[0],
            invitationLink,
            admin.Email,
            cancellationToken
        );

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

    public async Task<Result<InvitationValidationResponse>> ValidateInvitationTokenAsync(
        string token,
        CancellationToken cancellationToken
    )
    {
        InvitationLink? invitation = await _context.InvitationLinks.FirstOrDefaultAsync(
            i => i.Token == token && !i.IsUsed && i.ExpiresAt > DateTime.UtcNow,
            cancellationToken
        );

        return invitation != null
            ? Result.Success(new InvitationValidationResponse 
            { 
                IsValid = true, 
                TargetRole = invitation.TargetRole 
            })
            : Result.Failure<InvitationValidationResponse>(
                new Error(
                    "Auth.InvalidInvitation",
                    "Token de invitación inválido o expirado",
                    ErrorType.Validation
                )
            );
    }

    public async Task<Result<bool>> RequestPasswordResetAsync(
        string email,
        CancellationToken cancellationToken
    )
    {
        User? user = await _context.Users.FirstOrDefaultAsync(
            u => EF.Functions.ILike(u.Email, email),
            cancellationToken
        );

        if (user == null)
        {
            // Return success even if user doesn't exist to prevent email enumeration
            return Result.Success(true);
        }

        // Generate password reset token
        string resetToken = Guid.NewGuid().ToString("N");

        var passwordResetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = resetToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = false,
            Email = user.Email,
            User = user,
        };

        _context.PasswordResetTokens.Add(passwordResetToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate reset link
        string resetLink = $"http://localhost:3000/reset-password?token={resetToken}";

        // Send password reset email
        await _notificationService.SendPasswordResetNotificationAsync(
            user.Email,
            user.Email.Split('@')[0], // Use part before @ as name
            resetLink,
            cancellationToken
        );

        return Result.Success(true);
    }

    public async Task<Result<bool>> ResetPasswordAsync(
        string token,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        PasswordResetToken? passwordResetToken = await _context
            .PasswordResetTokens.Include(p => p.User)
            .FirstOrDefaultAsync(
                p => p.Token == token && !p.IsUsed && p.ExpiresAt > DateTime.UtcNow,
                cancellationToken
            );

        if (passwordResetToken == null)
        {
            return Result.Failure<bool>(
                new Error(
                    "Auth.InvalidResetToken",
                    "Token de recuperación inválido o expirado",
                    ErrorType.Validation
                )
            );
        }

        // Update user password
        passwordResetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        passwordResetToken.IsUsed = true;
        passwordResetToken.UsedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
