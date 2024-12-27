using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using TestGeneratorAPI.Web.Schemas;

namespace TestGeneratorAPI.Web.Controllers;

[ApiController]
[Route("/api/v1/logs")]
public class LogsController : ControllerBase
{
    private readonly TelegramBotClient _telegramBot;

    public LogsController()
    {
        _telegramBot = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? "");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseSchema<DateTime>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseSchema<DateTime>>> ReceiveErrorLogs([FromBody, Required] string logs)
    {
        try
        {
            foreach (var chatId in Environment.GetEnvironmentVariable("TELEGRAM_CHAT_IDS")?.Split(';')
                         .Select(long.Parse) ?? [])
            {
                var inputFile = new InputFileStream(new MemoryStream(Encoding.UTF8.GetBytes(logs)), "logs.txt");
                await _telegramBot.SendDocument(chatId, inputFile,
                    caption: $"У кого-то упал TestGenerator:\n`{GetFatalDescription(logs)}`");
            }

            return Ok(new ResponseSchema<DateTime>
            {
                Data = DateTime.UtcNow,
                Detail = "User was created."
            });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
        }
    }

    private static string? GetFatalDescription(string logs)
    {
        var fatalIndex = logs.IndexOf("FATAL]", StringComparison.InvariantCulture);
        if (fatalIndex < 0)
            return null;
        var endlIndex = logs.IndexOf('\n', fatalIndex);
        if (endlIndex < 0)
            return logs.Substring(fatalIndex);
        return logs.Substring(fatalIndex, endlIndex - fatalIndex);
    }
}