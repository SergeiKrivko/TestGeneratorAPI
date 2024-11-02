using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.Core.Abstractions;
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

    public PluginReleasesController(IPluginReleasesService pluginReleasesService, IPluginsService pluginsService,
        IUsersService usersService)
    {
        _pluginReleasesService = pluginReleasesService;
        _usersService = usersService;
        _pluginsService = pluginsService;
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
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} {claim.Value}");
            }

            var tokenId = Guid.Parse(User.Claims.Where(c => c.Type == "Id").Single().Value);
            var userId = Guid.Parse(User.Claims.Where(c => c.Type == "UserId").Single().Value);
            var pluginIds =
                JsonSerializer.Deserialize<Guid[]>(User.Claims.Where(c => c.Type == "Plugins").Single().Value);
            var plugin = await _pluginsService.GetPluginByKey(request.Key);

            Console.WriteLine(pluginIds?[0]);

            if (!pluginIds?.Contains(plugin.PluginId) == true)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { error = "Permission denied" });
            }

            var id = await _pluginReleasesService.CreatePluginRelease(request, userId);

            return Ok(new ResponseSchema<Guid>
            {
                Data = id,
                Detail = "PluginRelease was created."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method CreatePluginRelease", details = ex.Message });
        }
    }

    [HttpGet("latest")]
    [ProducesResponseType(typeof(ResponseSchema<PluginReleaseRead>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<PluginReleaseRead>>> GetLatestRelease([FromHeader] [Required] string key,
        [FromHeader] [Required] string runtime)
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