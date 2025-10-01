using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Helpers;

public static class ProcessFiles
{
    public static async Task<ResultT<ResponseDto>> ProcessFilesAsync(
        ILogger logger,
        IEnumerable<IFormFile> files,
        Guid entityId,
        IFileRepository fileRepository,
        IEntityFileRepository entityFileRepository,
        ICloudinaryService cloudinaryService,
        CancellationToken cancellationToken)
    {
        var tasks = files.Select(async file =>
        {
            await using var stream = file.OpenReadStream();

            FileType? fileType = file.ContentType.StartsWith("image/") ? FileType.Image
                : file.ContentType.StartsWith("video/") ? FileType.Video
                : null;

            if (fileType is null)
            {
                logger.LogWarning("File type not supported: '{FileName}'", file.FileName);
                return false; 
            }

            string url = fileType == FileType.Image
                ? await cloudinaryService.UploadImageAsync(stream, file.FileName, cancellationToken)
                : await cloudinaryService.UploadVideoAsync(stream, file.FileName, cancellationToken);

            var newFile = new Models.File
            {
                Id = Guid.NewGuid(),
                Url = url,
                Type = fileType.ToString(),
                UploadedAt = DateTime.UtcNow
            };
            await fileRepository.CreateAsync(newFile, cancellationToken);

            var entityFile = new EntityFile
            {
                Id = Guid.NewGuid(),
                FileId = newFile.Id,
                TargetId = entityId,
                TargetType = TargetType.Post.ToString()
            };
            await entityFileRepository.CreateAsync(entityFile, cancellationToken);

            logger.LogInformation("File '{FileName}' uploaded successfully!", file.FileName);
            return true; 
        });

        var results = await Task.WhenAll(tasks);

        if (results.Any(r => !r))
            return ResultT<ResponseDto>.Failure(Error.Failure("415",
                "Some files could not be processed. Only images and videos are allowed."));

        return ResultT<ResponseDto>.Success(new ResponseDto("All your files have been uploaded successfully!"));
    }
}