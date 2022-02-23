using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace demo_app2.Controllers
{
    [ApiController]
    [Route("api/message/update")]
    public class TelegramBotController : ControllerBase
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ProtectedLocalStorage _localStorage;

        public TelegramBotController(TelegramBot telegramBot, IHttpClientFactory httpClientFactory, ProtectedLocalStorage localStorage)
        {
            _telegramBotClient = telegramBot.GetBot().Result;
            _httpClientFactory = httpClientFactory;
            _localStorage = localStorage;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello Team3!");
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Update update)
        {
            if (update?.Type != UpdateType.Message)
                return Ok();

            if (update.Message!.Type != MessageType.Text)
                return Ok();
            
            var chatId = update.Message.Chat.Id;
            
            var result = await _localStorage.GetAsync<string>("userLastMessage");
            if (result.Success)
            {
                Message sentMessage2 = await _telegramBotClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Your last value" + result.Value,
                    parseMode: ParseMode.Markdown
                );
            }

            var messageText = update.Message.Text;

            await _localStorage.SetAsync("userLastMessage", messageText);
            
            messageText = messageText ?? "no text";
            Message sentMessage = await _telegramBotClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                parseMode: ParseMode.Markdown
            );
            return Ok();
        }
    }
}
