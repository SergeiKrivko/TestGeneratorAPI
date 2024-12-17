using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Enums;
using TestGeneratorAPI.Core.Exceptions.Services;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.Web.Schemas;

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
    [ProducesResponseType(typeof(ResponseSchema<List<Guid>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<List<Guid>>>> UploadRelease(IFormFile file,
        [FromQuery, Required] Version version, [FromQuery, Required] string runtime)
    {
        try
        {
            var user = await _tokensService.GetUser(User, TokenPermission.CreateTestGeneratorRelease);
            if (user == null)
                return Unauthorized();
            var res = new List<Guid>();
            // foreach (var file in files)
            // {
            //     res.Add(await _appFileService.UploadFile(file.FileName, version, runtime, file));
            // }
            res.Add(await _appFileService.UploadFile(file.FileName, version, runtime, file));

            return Ok(new ResponseSchema<List<Guid>>
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