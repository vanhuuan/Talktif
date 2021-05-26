using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using Talktif.Models;
namespace Talktif.Controllers
{
    public partial class SignalRClient
    {
        HubConnection connection;
        public SignalRClient()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("/chathub")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0,5) * 1000);
                await connection.StartAsync();
            };
        }
        private async void connect(List<Message> messagesList)
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {

                Message m = new Message
                {
                    User = user,
                    Content = message,
                    SentAt = DateTime.Now
                };
                messagesList.Add(m);
            });

            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void ClientSendMessage(string user, string message)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", 
                    user, message);
            }
            catch (Exception ex)
            {                
                Console.WriteLine(ex.Message);          
            }
        }
    }
}