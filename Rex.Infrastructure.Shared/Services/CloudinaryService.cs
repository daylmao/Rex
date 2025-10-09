using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Rex.Application.Interfaces;
using CloudinaryConfiguration = Rex.Configurations.CloudinaryConfiguration;

namespace Rex.Infrastructure.Shared.Services;

public sealed class CloudinaryService(IOptions<CloudinaryConfiguration> cloudinaryOptions): ICloudinaryService
{
    private CloudinaryConfiguration _cloudinaryConfiguration { get; } = cloudinaryOptions.Value;

    public async Task<string> UploadArchiveAsync(Stream archive, string imageName, CancellationToken cancellationToken)
    {
        Cloudinary cloudinary = new Cloudinary(_cloudinaryConfiguration.CloudinaryUrl);
        RawUploadParams uploadArchive = new RawUploadParams()
        {
            File = new FileDescription(imageName, archive),
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true
        };
        
        RawUploadResult uploadResult = await cloudinary.UploadAsync(uploadArchive, "raw", cancellationToken);
        return uploadResult.SecureUrl.ToString();
    }

    public async Task<string> UploadImageAsync(Stream archive, string imageName, CancellationToken cancellationToken)
    {
        var cloudinary = new Cloudinary(_cloudinaryConfiguration.CloudinaryUrl);
        ImageUploadParams uploadImage = new()
        {
            File = new FileDescription(imageName, archive),
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true
        };
        ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadImage, cancellationToken);
        return uploadResult.SecureUrl.ToString();
    }
    
    public async Task<string> UploadVideoAsync(Stream archive, string imageName, CancellationToken cancellationToken)
    {
        var cloudinary = new Cloudinary(_cloudinaryConfiguration.CloudinaryUrl);
        VideoUploadParams uploadVideo = new()
        {
            File = new FileDescription(imageName, archive),
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true
        };
        VideoUploadResult uploadResult = await cloudinary.UploadAsync(uploadVideo, cancellationToken);
        return uploadResult.SecureUrl.ToString();
    }
}