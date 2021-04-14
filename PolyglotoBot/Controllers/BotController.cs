using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using PolyglotoBot.Services;
using System.Threading.Tasks;

namespace PolyglotoBot.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter Adapter;
        private readonly IBot Bot;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot, IServicebusTrigger servicebusTrigger)
        {
            Adapter = adapter;
            Bot = bot;
            Task.Run(async () => await servicebusTrigger.Start().ConfigureAwait(false));
        }

        [HttpPost, HttpGet]
        public async Task PostAsync()
        {
            await Adapter.ProcessAsync(Request, Response, Bot);
        }
    }
}
