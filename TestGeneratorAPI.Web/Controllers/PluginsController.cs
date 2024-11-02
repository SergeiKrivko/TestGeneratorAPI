using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Exceptions.Services;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.Web.Schemas;

namespace TestGeneratorAPI.Web.Controllers;

[ApiController]
[Route("/api/v1/plugins")]
public class PluginsController : ControllerBase
{
    private readonly IPluginsService _pluginsService;
    private readonly IUsersService _usersService;
    
    public PluginsController(IPluginsService pluginsService, IUsersService usersService)
    {
        _pluginsService = pluginsService;
        _usersService = usersService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = BasicDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ResponseSchema<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<Guid>>> CreatePlugin([FromBody] [Required] PluginCreate request)
    {
        try
        {
            var user = await _usersService.Get(User.Identity?.Name ?? "");
            var id = await _pluginsService.CreatePlugin(request, user.UserId);

            return Ok(new ResponseSchema<Guid>
            {
                Data = id,
                Detail = "Plugin was created."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (PluginsServiceException e)
        {
            return StatusCode(StatusCodes.Status409Conflict, new { error = e.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method CreatePlugin", details = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseSchema<PluginRead[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<PluginRead[]>>> GetPlugins()
    {
        try
        {
            var plugins = await _pluginsService.GetAllPlugins();

            return Ok(new ResponseSchema<PluginRead[]>
            {
                Data = plugins.ToArray(),
                Detail = "Plugin was created."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (PluginsServiceException e)
        {
            return StatusCode(StatusCodes.Status409Conflict, new { error = e.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method CreatePlugin", details = ex.Message });
        }
    }
}