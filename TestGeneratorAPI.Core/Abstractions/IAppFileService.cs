using Microsoft.AspNetCore.Http;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IAppFileService
{
    public Task<Guid> UploadFile(string filename, Version version, string runtime, IFormFile stream);
}