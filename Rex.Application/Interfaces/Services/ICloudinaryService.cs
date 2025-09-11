namespace Rex.Application.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadArchiveAsync(Stream archive, string archiveName, CancellationToken cancellationToken);
    Task<string> UploadImageAsync(Stream archive, string imageName, CancellationToken cancellationToken);
    
}