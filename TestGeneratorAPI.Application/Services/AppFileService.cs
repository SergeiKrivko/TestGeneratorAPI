using System.IO.Compression;
using System.Security.Cryptography;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Application.Services;

public class AppFileService : IAppFileService
{
    private readonly AmazonS3Client _s3Client;
    private readonly IReleaseRepository _releaseRepository;
    private readonly IAppFilesRepository _appFilesRepository;

    private const string MainBucket = "bucket-testgenerator";
    private const string ZipBucket = "testgenerator-zip";

    public AppFileService(IReleaseRepository releaseRepository, IAppFilesRepository appFilesRepository)
    {
        _s3Client = new AmazonS3Client(
            new BasicAWSCredentials(Environment.GetEnvironmentVariable("S3_ACCESS_KEY"),
                Environment.GetEnvironmentVariable("S3_SECRET_KEY")),
            new AmazonS3Config
                { ServiceURL = "https://s3.cloud.ru", AuthenticationRegion = "ru-central-1", ForcePathStyle = true });
        _releaseRepository = releaseRepository;
        _appFilesRepository = appFilesRepository;
    }

    public async Task<List<string>> FilterFiles(string runtime, ICollection<AppFileDownload> files)
    {
        var res = new List<string>();

        try
        {
            var release = await _releaseRepository.GetLatest(runtime);
            var existingFiles = await _appFilesRepository.GetAll(release.ReleaseId);
            foreach (var file in files)
            {
                var existing = existingFiles.FirstOrDefault(e => e.Filename == file.Filename);
                if (!string.Equals(existing?.Hash, file.Hash, StringComparison.OrdinalIgnoreCase))
                    res.Add(file.Filename);
            }

            return res;
        }
        catch (InvalidOperationException)
        {
            return files.Select(f => f.Filename).ToList();
        }
    }

    public async Task<Guid> UploadReleaseZip(Version version, string runtime, IFormFile zipFile, string[] files)
    {
        var releaseId = Guid.NewGuid();
        List<AppFileRead> existingFiles = [];
        try
        {
            var previousRelease = await _releaseRepository.GetLatest(runtime);
            existingFiles = await _appFilesRepository.GetAll(previousRelease.ReleaseId);
        }
        catch (InvalidOperationException)
        {
        }

        await _releaseRepository.CreateRelease(releaseId, runtime, version);

        Console.WriteLine(string.Join("; ", files));

        var zipPath = Path.GetTempFileName();
        if (File.Exists(zipPath))
            File.Delete(zipPath);
        if (Directory.Exists(zipPath))
            Directory.Delete(zipPath, recursive: true);
        await using (var stream = zipFile.OpenReadStream())
        {
            await Task.Run(() => ZipFile.ExtractToDirectory(stream, zipPath));
        }

        try
        {
            foreach (var filename in files)
            {
                var entryPath = Path.Join(zipPath, filename);
                if (!File.Exists(entryPath))
                {
                    var existing = existingFiles.First(e => e.Filename == filename);
                    await _appFilesRepository.Create(Guid.NewGuid(), releaseId, existing.S3Id, filename, existing.Hash);
                }
                else
                {
                    string hash;
                    await using (var fileStream = File.OpenRead(entryPath))
                    using (var sha256 = SHA256.Create())
                    {
                        var hashBytes = await sha256.ComputeHashAsync(fileStream);
                        hash = BitConverter.ToString(hashBytes).Replace("-", "");
                    }

                    var s3Id = Guid.NewGuid();

                    await using (var stream = File.OpenRead(entryPath))
                    {
                        var putRequest = new PutObjectRequest
                        {
                            BucketName = MainBucket,
                            Key = s3Id.ToString(),
                            InputStream = stream,
                            ContentType = "application/octet-stream"
                        };

                        await _s3Client.PutObjectAsync(putRequest);
                    }

                    await _appFilesRepository.Create(Guid.NewGuid(), releaseId, s3Id, filename, hash);
                }
            }
        }
        catch (Exception)
        {
            Directory.Delete(zipPath, recursive: true);
            throw;
        }

        Directory.Delete(zipPath, recursive: true);

        return releaseId;
    }

    public async Task<string> CreateReleaseZip(AppFileDownload[] files, string runtime)
    {
        var zipFilePath = Path.GetTempFileName() + ".zip"; // Создаем временный файл

        var release = await _releaseRepository.GetLatest(runtime);

        await using (var zipStream = new FileStream(zipFilePath, FileMode.Create))
        using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            var fileEntities = await _appFilesRepository.GetAll(release.ReleaseId);
            foreach (var fileEntity in fileEntities)
            {
                var file = files.FirstOrDefault(f => f.Filename == fileEntity.Filename);
                if (string.Equals(file?.Hash.Replace("-", ""), fileEntity.Hash, StringComparison.OrdinalIgnoreCase))
                    continue;
                Console.WriteLine($"Adding {file?.Filename}");
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
                BucketName = ZipBucket,
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

    public async Task<Version> GetLatestVersion(string runtime)
    {
        return (await _releaseRepository.GetLatest(runtime)).Version;
    }
}