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

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(typeof(ResponseSchema<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<Guid>>> UploadRelease(IFormFile file,
        [FromQuery, Required] Version version, [FromQuery, Required] string runtime)
    {
        Console.WriteLine("POST api/v2/releases");
        try
        {
            var user = await _tokensService.GetUser(User, TokenPermission.CreateTestGeneratorRelease);
            Console.WriteLine($"UserId = {user?.UserId}");
            if (user == null)
                return Unauthorized();
            var res = await _appFileService.UploadFile(file.FileName, version, runtime, file);

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
    [ProducesResponseType(typeof(ResponseSchema<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<string>>> DownloadRelease([FromBody] AppFileDownload[] files,
        [FromQuery, Required] string runtime)
    {
        try
        {
            var res = await _appFileService.CreateReleaseZip(files, runtime);

            return Ok(new ResponseSchema<string>
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
}