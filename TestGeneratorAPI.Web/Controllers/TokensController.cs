using System.ComponentModel.DataAnnotations;
using AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Enums;
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
    [Authorize(AuthenticationSchemes = "Basic,Bearer")]
    [ProducesResponseType(typeof(ResponseSchema<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<string>>> CreateToken([FromBody, Required] TokenCreate request)
    {
        try
        {
            if (request.Type == TokenType.Admin)
                return StatusCode(StatusCodes.Status401Unauthorized,
                    new { error = "Permission denied: can not create admin token here" });
            
            var user = await _tokensService.GetUser(User, TokenPermission.UpdateUser);
            if (user == null)
                return Unauthorized();

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

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Basic,Bearer")]
    [ProducesResponseType(typeof(ResponseSchema<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<List<TokenRead>>>> GetTokens()
    {
        try
        {
            var user = await _tokensService.GetUser(User);
            if (user == null)
                return Unauthorized();

            var tokens = await _tokensService.GetTokensOfUser(user.UserId);

            return Ok(new ResponseSchema<List<TokenRead>>
            {
                Data = tokens,
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
                new { error = "Error in method Get tokens", details = ex.Message });
        }
    }

    [HttpGet("permissions")]
    [ProducesResponseType(typeof(ResponseSchema<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<List<TokenPermission>>>> GetPermissions()
    {
        try
        {
            return Ok(new ResponseSchema<List<TokenPermission>>
            {
                Data =
                [
                    TokenPermission.UpdateUser,
                    TokenPermission.CreatePlugin,
                    TokenPermission.RemovePlugin,
                    TokenPermission.CreateRelease,
                    TokenPermission.RemoveRelease,
                    TokenPermission.CreateTestGeneratorRelease,
                ],
                Detail = "Token permissions was selected."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method Get tokens", details = ex.Message });
        }
    }

    [HttpDelete("{tokenId:guid}")]
    [Authorize(AuthenticationSchemes = "Basic,Bearer")]
    [ProducesResponseType(typeof(ResponseSchema<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<Guid>>> DeleteToken(Guid tokenId)
    {
        try
        {
            var user = await _tokensService.GetUser(User, TokenPermission.UpdateUser);
            if (user == null)
                return Unauthorized();

            var token = await _tokensService.GetToken(tokenId);
            if (token.UserId != user.UserId)
                return StatusCode(StatusCodes.Status401Unauthorized);

            await _tokensService.DeleteToken(tokenId);

            return Ok(new ResponseSchema<Guid>
            {
                Data = tokenId,
                Detail = "Token was deleted."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method Get tokens", details = ex.Message });
        }
    }
}