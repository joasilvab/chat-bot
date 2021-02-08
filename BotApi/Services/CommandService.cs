using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Csv;
using RabbitMQ.Client;

namespace BotApi.Services
{
    public class CommandService : ICommandService
    {
        public IHttpClientFactory clientFactory;
        public CommandService(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }
        public async Task ExecuteCommand(string command, string parameter)
        {
            if (command == "stock")
            {
                await SendStock(parameter);
            }
            else
            {
                SendMessage("I don't know how to process that!");
            }
        }

        private async Task SendStock(string stockCode)
        {
            var client = clientFactory.CreateClient();
            var response = await client.GetAsync($"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv");
            using var stream = await response.Content.ReadAsStreamAsync();
            var csvValues = Csv.CsvReader.ReadFromStream(stream).ToList();
            var shareValue = csvValues[0]["Close"];
            var stockSymbol = csvValues[0]["Symbol"];
            var message = "";
            if (shareValue == "N/D")
            {
                message = $"No information available for {stockSymbol}";

            }
            else
            {
                message = $"{stockSymbol} quote is ${shareValue} per share";
            }
            SendMessage(message);
        }

        private void SendMessage(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "chatbot",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "chatbot",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
