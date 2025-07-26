using Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Infrastructure;
using SharedKernel.Results;
using System.Security.Claims;
using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Users;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IImageStorageService _imageStorageService;
    private readonly IApplicationDbContext _dbContext;

    public ImagesController(IImageStorageService imageStorageService, IApplicationDbContext dbContext)
    {
        _imageStorageService = imageStorageService;
        _dbContext = dbContext;
    }

    [HttpPost("upload")]
    [Authorize]
    public async Task<IResult> UploadImage(
        [FromBody] UploadImageRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.ImageBase64))
        {
            return Results.BadRequest("La imagen en base64 es requerida");
        }

        Result<(Uri Url, string PublicId)> result = await _imageStorageService.UploadImageAsync(
            request.ImageBase64,
            cancellationToken
        );

        return result.IsSuccess
            ? Results.Ok(new { Url = result.Value.Url.ToString(), result.Value.PublicId })
            : CustomResults.Problem(result);
    }

    [HttpDelete("{publicId}")]
    [Authorize]
    public async Task<IResult> DeleteImage(string publicId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(publicId))
        {
            return Results.BadRequest("El ID público es requerido");
        }

        bool deleted = await _imageStorageService.DeleteImageAsync(publicId, cancellationToken);

        return deleted
            ? Results.Ok(new { Message = "Imagen eliminada exitosamente" })
            : Results.BadRequest(new { Message = "No se pudo eliminar la imagen" });
    }

    /// <summary>
    /// Actualizar foto de perfil del usuario autenticado
    /// </summary>
    [HttpPost("profile-picture")]
    [Authorize]
    public async Task<IResult> UpdateProfilePicture(
        [FromBody] UploadImageRequest request,
        CancellationToken cancellationToken
    )
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Results.Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.ImageBase64))
        {
            return Results.BadRequest("La imagen en base64 es requerida");
        }

        // Subir nueva imagen a Cloudinary
        Result<(Uri Url, string PublicId)> uploadResult = await _imageStorageService.UploadImageAsync(
            request.ImageBase64,
            cancellationToken
        );

        if (!uploadResult.IsSuccess)
        {
            return CustomResults.Problem(uploadResult);
        }

        // Buscar usuario y actualizar foto de perfil
        User? user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return Results.NotFound("Usuario no encontrado");
        }

        // Eliminar imagen anterior si existe
        if (!string.IsNullOrEmpty(user.ProfilePicturePublicId))
        {
            await _imageStorageService.DeleteImageAsync(user.ProfilePicturePublicId, cancellationToken);
        }

        // Actualizar usuario con nueva imagen
        user.ProfilePictureUrl = uploadResult.Value.Url;
        user.ProfilePicturePublicId = uploadResult.Value.PublicId;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new { 
            Url = uploadResult.Value.Url.ToString(), 
            uploadResult.Value.PublicId,
            Message = "Foto de perfil actualizada exitosamente"
        });
    }

    /// <summary>
    /// Eliminar foto de perfil del usuario autenticado
    /// </summary>
    [HttpDelete("profile-picture")]
    [Authorize]
    public async Task<IResult> DeleteProfilePicture(CancellationToken cancellationToken)
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Results.Unauthorized();
        }

        User? user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return Results.NotFound("Usuario no encontrado");
        }

        if (string.IsNullOrEmpty(user.ProfilePicturePublicId))
        {
            return Results.BadRequest("El usuario no tiene foto de perfil");
        }

        // Eliminar de Cloudinary
        bool deleted = await _imageStorageService.DeleteImageAsync(user.ProfilePicturePublicId, cancellationToken);
        
        if (deleted)
        {
            // Limpiar campos en la base de datos
            user.ProfilePictureUrl = null;
            user.ProfilePicturePublicId = null;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { Message = "Foto de perfil eliminada exitosamente" });
        }

        return Results.BadRequest(new { Message = "No se pudo eliminar la foto de perfil" });
    }

    [HttpGet("profiles")]
    [Authorize]
    public async Task<IResult> GetProfileImages(
        [FromQuery] string employeeIds,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(employeeIds))
            {
                return Results.BadRequest(new { Message = "employeeIds parameter is required" });
            }

            var idsList = employeeIds.Split(',')
                .Where(id => Guid.TryParse(id.Trim(), out _))
                .Select(id => Guid.Parse(id.Trim()))
                .ToList();

            if (!idsList.Any())
            {
                return Results.BadRequest(new { Message = "No valid employee IDs provided" });
            }

            var employeeProfiles = await _dbContext.EmployeeProfiles
                .Include(ep => ep.User)
                .Where(ep => idsList.Contains(ep.Id))
                .Select(ep => new
                {
                    EmployeeId = ep.Id,
                    ProfilePictureUrl = ep.User != null && ep.User.ProfilePicturePublicId != null
                        ? _imageStorageService.GenerateImageUrl(ep.User.ProfilePicturePublicId)!.ToString()
                        : null
                })
                .ToListAsync(cancellationToken);

            return Results.Ok(employeeProfiles);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving profile images: {ex.Message}");
        }
    }
}

public record UploadImageRequest(string ImageBase64);
