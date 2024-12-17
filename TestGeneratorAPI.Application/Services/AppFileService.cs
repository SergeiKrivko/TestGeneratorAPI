using System.Security.Cryptography;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Exceptions.Repositories;

namespace TestGeneratorAPI.Application.Services;

public class AppFileService : IAppFileService
{
    private readonly AmazonS3Client _s3Client;
    private readonly IAppFilesRepository _appFilesRepository;

    private const string MainBucket = "bucket-testgenerator";

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
            var putRequest = new Amazon.S3.Model.PutObjectRequest
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
}