using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Enums;
using TestGeneratorAPI.Core.Exceptions.Services;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.Web.Schemas;

namespace TestGeneratorAPI.Web.Controllers;

[ApiController]
[Route("/api/v1/plugins")]
public class PluginsController : ControllerBase
{
    private readonly IPluginsService _pluginsService;
    private readonly ITokensService _tokensService;

    public PluginsController(IPluginsService pluginsService, ITokensService usersService)
    {
        _pluginsService = pluginsService;
        _tokensService = usersService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Basic,Bearer")]
    [ProducesResponseType(typeof(ResponseSchema<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<Guid>>> CreatePlugin([FromBody] [Required] PluginCreate request)
    {
        try
        {
            var user = await _tokensService.GetUser(User, TokenPermission.CreatePlugin, request.Key);
            if (user == null)
                return Unauthorized();
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
                Detail = "Plugins was selected."
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

    [HttpGet("my")]
    [Authorize(AuthenticationSchemes = "Basic,Bearer")]
    [ProducesResponseType(typeof(ResponseSchema<PluginRead[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<PluginRead[]>>> GetUserPlugins()
    {
        try
        {
            var user = await _tokensService.GetUser(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var plugins = await _pluginsService.GetUserPlugins(user.UserId);

            return Ok(new ResponseSchema<PluginRead[]>
            {
                Data = plugins.ToArray(),
                Detail = "Plugins was selected."
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