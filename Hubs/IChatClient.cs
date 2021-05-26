using System.Threading.Tasks;
namespace Talktif.Hubs 
{
    public interface IChatClient {
        Task ReceiveMessage(string user, string message);
        Task BroadcastMessage(string message);
    }
}