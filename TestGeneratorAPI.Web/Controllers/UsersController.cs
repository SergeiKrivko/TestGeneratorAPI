using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.Core.Models;
using TestGeneratorAPI.Web.Schemas;

namespace TestGeneratorAPI.Web.Controllers;

[ApiController]
[Route("/api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    
    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseSchema<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<Guid>>> CreateUser([FromBody] [Required] UserCreate request)
    {
        try
        {
            var id = await _usersService.CreateUser(request);

            return Ok(new ResponseSchema<Guid>
            {
                Data = id,
                Detail = "User was created."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in method CreateUser", details = ex.Message });
        }
    }
}