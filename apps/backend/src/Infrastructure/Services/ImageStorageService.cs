using System.Net;
using Application.Abstractions.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using SharedKernel;
using SharedKernel.Results;
using Error = SharedKernel.Errors.Error;

namespace Infrastructure.Services;

internal sealed class ImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;

    public ImageStorageService(IOptions<CloudinarySettings> opts)
    {
        CloudinarySettings s = opts.Value;

        if (string.IsNullOrEmpty(s.CloudName) || string.IsNullOrEmpty(s.ApiKey) || string.IsNullOrEmpty(s.ApiSecret))
        {
            throw new InvalidOperationException(
                "Cloudinary is not properly configured. Please set CLOUDINARY_CLOUD_NAME, CLOUDINARY_API_KEY, and CLOUDINARY_API_SECRET environment variables."
            );
        }

        _cloudinary = new Cloudinary(new Account(s.CloudName, s.ApiKey, s.ApiSecret));
    }

    public async Task<Result<(Uri Url, string PublicId)>> UploadImageAsync(
        string imageInBase64,
        CancellationToken cancellationToken = default
    )
    {
        await using Stream stream = Utilities.StreamHelper.Base64ToStream(imageInBase64);
        string imageId = Guid.NewGuid().ToString();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(imageId, stream),
            UseFilename = false,
            Overwrite = false,
        };

        ImageUploadResult? result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        return result.StatusCode != HttpStatusCode.OK
            ? Result.Failure<(Uri Url, string PublicId)>(
                Error.Failure("ImageUpload.Failed", "Failed to upload image to Cloudinary")
            )
            : (result.SecureUrl, result.PublicId);
    }

    public async Task<bool> DeleteImageAsync(
        string publicId,
        CancellationToken cancellationToken = default
    )
    {
        var deletionParams = new DeletionParams(publicId);

        DeletionResult? result = await _cloudinary.DestroyAsync(deletionParams);

        return result.Result == "ok";
    }
    
    public Uri? GenerateImageUrl(string? publicId)
    {
        if (string.IsNullOrEmpty(publicId))
        {
            return null;
        }
            
        string urlString = _cloudinary.Api.UrlImgUp.BuildUrl(publicId);
        return new Uri(urlString);
    }

}
