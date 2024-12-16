using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Enums;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.Web.Schemas;

namespace TestGeneratorAPI.Web.Controllers;

[ApiController]
[Route("/api/v1/plugins/releases")]
public class PluginReleasesController : ControllerBase
{
    private readonly IPluginReleasesService _pluginReleasesService;
    private readonly IUsersService _usersService;
    private readonly IPluginsService _pluginsService;
    private readonly ITokensService _tokensService;

    public PluginReleasesController(IPluginReleasesService pluginReleasesService, IPluginsService pluginsService,
        IUsersService usersService, ITokensService tokensService)
    {
        _pluginReleasesService = pluginReleasesService;
        _usersService = usersService;
        _pluginsService = pluginsService;
        _tokensService = tokensService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ResponseSchema<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<Guid>>> CreatePluginRelease(
        [FromBody] [Required] PluginReleaseCreate request)
    {
        try
        {
            var plugin = await _pluginsService.GetPluginByKey(request.Key);

            if (!await _tokensService.CheckPermissions(User, TokenPermission.CreateRelease, plugin.PluginId))
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { error = "Permission denied" });
            }

            if (await _pluginReleasesService.PluginReleaseExists(plugin.PluginId, request.Version, request.Runtime))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new
                    {
                        error = "Already exists",
                        detail = $"Release of this plugin with version '{request.Version}' " +
                                 $"and runtime {request.Runtime} already exists"
                    });

            var id = await _pluginReleasesService.CreatePluginRelease(request,
                Guid.Parse(User.Claims.Single(c => c.Type == "UserId").Value));

            return Ok(new ResponseSchema<Guid>
            {
                Data = id,
                Detail = "PluginRelease was created."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = "Bad request", detail = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Internal Server Error", detail = ex.Message });
        }
    }

    [HttpGet("latest")]
    [ProducesResponseType(typeof(ResponseSchema<PluginReleaseRead>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<PluginReleaseRead>>> GetLatestRelease(
        [FromQuery] [Required] string key,
        [FromQuery] [Required] string runtime)
    {
        try
        {
            var plugin = await _pluginsService.GetPluginByKey(key);
            var release = await _pluginReleasesService.GetLatestRelease(plugin.PluginId, runtime);

            return Ok(new ResponseSchema<PluginReleaseRead>
            {
                Data = release,
                Detail = $"Latest release of plugin {plugin.PluginId} was selected."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method GetLatestRelease", details = ex.Message });
        }
    }
}