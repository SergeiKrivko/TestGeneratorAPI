using System.ComponentModel.DataAnnotations;
using AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.Web.Schemas;

namespace TestGeneratorAPI.Web.Controllers;

[ApiController]
[Route("/api/v1/tokens")]
public class TokensController : ControllerBase
{
    private readonly ITokensService _tokensService;
    private readonly IUsersService _usersService;
    
    public TokensController(ITokensService tokensService, IUsersService usersService)
    {
        _tokensService = tokensService;
        _usersService = usersService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = BasicDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ResponseSchema<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<string>>> CreateToken([FromBody, Required] TokenCreate request)
    {
        try
        {
            var user = await _usersService.Get(User.Identity?.Name ?? "");
            
            var token = await _tokensService.CreateToken(request, user.UserId);

            return Ok(new ResponseSchema<string>
            {
                Data = token,
                Detail = "Token was created."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method CreateToken", details = ex.Message });
        }
    }
}