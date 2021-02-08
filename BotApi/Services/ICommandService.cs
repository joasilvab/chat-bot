using System.Threading.Tasks;

namespace BotApi.Services
{
    public interface ICommandService
    {
        Task ExecuteCommand(string command, string parameter);
    }
}
