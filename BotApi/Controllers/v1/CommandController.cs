using System.Threading.Tasks;
using BotApi.Model;
using BotApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BotApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CommandController
    {
        private readonly ICommandService commandService;

        public CommandController(ICommandService commandService)
        {
            this.commandService = commandService;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Execute([FromBody] Command post)
        {
            await commandService.ExecuteCommand(post.Name, post.Parameter);
            return true;
        }
    }
}
