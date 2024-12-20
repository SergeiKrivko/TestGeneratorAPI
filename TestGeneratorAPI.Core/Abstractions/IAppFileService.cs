using Microsoft.AspNetCore.Http;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IAppFileService
{
    public Task<Guid> UploadFile(string filename, Version version, string runtime, IFormFile stream);
    public Task<Stream?> GetFile(AppFileDownload file, string runtime);

    public Task<string> CreateReleaseZip(AppFileDownload[] files, string runtime);

    public Task<Version> GetLatestVersion(string runtime);
}