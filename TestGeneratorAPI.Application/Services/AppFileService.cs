using System.IO.Compression;
using System.Security.Cryptography;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Exceptions.Repositories;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Application.Services;

public class AppFileService : IAppFileService
{
    private readonly AmazonS3Client _s3Client;
    private readonly IAppFilesRepository _appFilesRepository;

    private const string MainBucket = "bucket-testgenerator";
    private const string ZipBucket = "testgenerator-zip";

    public AppFileService(IAppFilesRepository appFilesRepository)
    {
        _s3Client = new AmazonS3Client(
            new BasicAWSCredentials(Environment.GetEnvironmentVariable("S3_ACCESS_KEY"),
                Environment.GetEnvironmentVariable("S3_SECRET_KEY")),
            new AmazonS3Config
                { ServiceURL = "https://s3.cloud.ru", AuthenticationRegion = "ru-central-1", ForcePathStyle = true });
        _appFilesRepository = appFilesRepository;
    }

    public async Task<Guid> UploadFile(string filename, Version version, string runtime, IFormFile file)
    {
        var id = Guid.NewGuid();

        string hash;
        using (var sha256 = SHA256.Create())
        {
            await using (var stream = file.OpenReadStream())
            {
                var hashBytes = await sha256.ComputeHashAsync(stream);
                hash = BitConverter.ToString(hashBytes);
            }
        }

        try
        {
            var existing = await _appFilesRepository.GetLatest(filename, runtime);
            if (existing.Hash == hash)
                return existing.Id;
        }
        catch (AppFilesRepositoryException)
        {
        }

        await using (var stream = file.OpenReadStream())
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = MainBucket,
                Key = id.ToString(),
                InputStream = stream,
                ContentType = "application/octet-stream"
            };

            await _s3Client.PutObjectAsync(putRequest);
        }

        await _appFilesRepository.Create(id, filename, version, runtime, hash);

        return id;
    }

    public async Task<Stream?> GetFile(AppFileDownload file, string runtime)
    {
        var fileEntity = await _appFilesRepository.GetLatest(file.Filename, runtime);
        if (fileEntity.Hash == file.Hash)
            return null;
        return (await _s3Client.GetObjectAsync(MainBucket, fileEntity.Id.ToString())).ResponseStream;
    }

    public async Task<string> CreateReleaseZip(AppFileDownload[] files, string runtime)
    {
        var zipFilePath = Path.GetTempFileName() + ".zip"; // Создаем временный файл

        await using (var zipStream = new FileStream(zipFilePath, FileMode.Create))
        using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            var fileEntities = await _appFilesRepository.GetAllLatest(runtime);
            Console.WriteLine(string.Join("; ", fileEntities.Select(e => e.Filename)));
            foreach (var fileEntity in fileEntities)
            {
                var file = files.FirstOrDefault(f => f.Filename == fileEntity.Filename);
                if (file?.Hash == fileEntity.Hash)
                    continue;
                using (var stream = (await _s3Client.GetObjectAsync(MainBucket, fileEntity.Id.ToString()))
                       .ResponseStream)
                {
                    var zipEntry = zipArchive.CreateEntry(fileEntity.Filename, CompressionLevel.Optimal);
                    using (var entryStream = zipEntry.Open())
                    {
                        await stream.CopyToAsync(entryStream);
                    }
                }
            }
        }

        var zipId = Guid.NewGuid();
        await using (var stream = File.OpenRead(zipFilePath))
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = MainBucket,
                Key = zipId.ToString(),
                InputStream = stream,
                ContentType = "application/zip"
            };

            await _s3Client.PutObjectAsync(putRequest);
        }

        File.Delete(zipFilePath);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = ZipBucket,
            Key = zipId.ToString(),
            Expires = DateTime.UtcNow.AddHours(1)
        };
        return await _s3Client.GetPreSignedURLAsync(request);
    }
}