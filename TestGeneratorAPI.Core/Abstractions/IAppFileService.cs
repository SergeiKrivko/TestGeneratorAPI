using Microsoft.AspNetCore.Http;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IAppFileService
{
    public Task<List<string>> FilterFiles(string runtime, ICollection<AppFileDownload> files);
    public Task<Guid> UploadReleaseZip(Version version, string runtime, IFormFile stream, string[] files);

    public Task<ReleaseZipRead> CreateReleaseZip(AppFileDownload[] files, string runtime);

    public Task<Version> GetLatestVersion(string runtime);
}