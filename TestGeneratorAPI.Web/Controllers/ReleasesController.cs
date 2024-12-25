using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Enums;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.Web.Schemas;
using Guid = System.Guid;

namespace TestGeneratorAPI.Web.Controllers;

[ApiController]
[Route("/api/v1/releases")]
public class ReleasesController : ControllerBase
{
    private readonly IAppFileService _appFileService;
    private readonly ITokensService _tokensService;

    public ReleasesController(IAppFileService appFileService, ITokensService usersService)
    {
        _appFileService = appFileService;
        _tokensService = usersService;
    }

    [HttpPost("filter")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(typeof(ResponseSchema<ICollection<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<ICollection<string>>>> FilterRelease(
        [FromBody] AppFileDownload[] files,
        [FromQuery, Required] string runtime)
    {
        try
        {
            var user = await _tokensService.GetUser(User);
            if (user == null || !user.HavePermission(TokenPermission.CreateTestGeneratorRelease))
                return Unauthorized();
            var res = await _appFileService.FilterFiles(runtime, files);

            return Ok(new ResponseSchema<ICollection<string>>
            {
                Data = res,
                Detail = "Release assets were uploaded."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method CreateRelease", details = ex.Message });
        }
    }

    [HttpPost("upload")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [RequestSizeLimit(104857600)]
    [ProducesResponseType(typeof(ResponseSchema<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<Guid>>> UploadRelease(UploadReleaseRequest request,
        [FromQuery, Required] string runtime, [FromQuery, Required] Version version)
    {
        try
        {
            var user = await _tokensService.GetUser(User);
            if (user == null || !user.HavePermission(TokenPermission.CreateTestGeneratorRelease))
                return Unauthorized();

            var res = await _appFileService.UploadReleaseZip(version, runtime, request.Zip, request.Files);

            return Ok(new ResponseSchema<Guid>
            {
                Data = res,
                Detail = "Release assets were uploaded."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method CreateRelease", details = ex.Message });
        }
    }

    [HttpPost("download")]
    [ProducesResponseType(typeof(ResponseSchema<ReleaseZipRead>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<ReleaseZipRead>>> DownloadRelease([FromBody] AppFileDownload[] files,
        [FromQuery, Required] string runtime)
    {
        try
        {
            var res = await _appFileService.CreateReleaseZip(files, runtime);

            return Ok(new ResponseSchema<ReleaseZipRead>
            {
                Data = res,
                Detail = "Release assets were uploaded."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method CreateRelease", details = ex.Message });
        }
    }

    [HttpGet("latest")]
    [ProducesResponseType(typeof(ResponseSchema<Version>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<Version>>> GetLatestVersion([FromQuery, Required] string runtime)
    {
        try
        {
            var res = await _appFileService.GetLatestVersion(runtime);

            return Ok(new ResponseSchema<Version>
            {
                Data = res,
                Detail = "Latest version was selected."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method CreateRelease", details = ex.Message });
        }
    }
}