using SharedKernel.Results;

namespace Application.Abstractions.Services;

public interface IImageStorageService
{
    Task<Result<(Uri Url, string PublicId)>> UploadImageAsync(
        string imageInBase64,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteImageAsync(string publicId, CancellationToken cancellationToken = default);
    
    Uri? GenerateImageUrl(string? publicId);
}
