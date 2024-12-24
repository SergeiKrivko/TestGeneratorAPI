using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Web.Schemas;

public class UploadReleaseRequest
{
    public required string[] Files { get; init; }
    public required IFormFile Zip { get; init; }
}