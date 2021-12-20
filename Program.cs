using System;
using System.Threading.Tasks;
using System.IO;
using Discord.WebSocket;
using Discord;

namespace DiscordCSBot
{
    class Program
    {
        private DiscordSocketClient _client;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            string token = String.Empty;
            try
            {
                token = File.ReadAllText("APIKey.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading the file \"APIKey.txt\"");
                Console.WriteLine(e.Message);
            }

            _client = new DiscordSocketClient(
                new DiscordSocketConfig { MessageCacheSize = 100 });
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.MessageReceived += _client_MessageReceived;

            await Task.Delay(-1);
        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            string content = arg.Content.ToLower();
            if (arg.Author.IsBot ||
                (arg.Author.Id == _client.CurrentUser.Id)) return;
            if ((content.Contains("chuck") || content.Contains("norris")))
            {
                var joke = await DataSource.RandomJoke();
                await arg.Channel.SendMessageAsync("Did anyone say something about Chuck Norris?\n" +
                    $"{joke}");
            }
            else if (content.StartsWith("!cnjoke"))
            {
                var args = content.Split(' ');
                if (args.Length == 2)
                {
                    try
                    {
                        var joke = await DataSource.CatJoke(args[1]);
                        await arg.Channel.SendMessageAsync(joke);
                    }
                    catch (Exception ex)
                    {
                        await arg.Channel.SendMessageAsync("Something doesn't seem right");
                    }
                }
            }
            else if (content.StartsWith("!cncat"))
            {
                var cats = String.Join("\n- ", (await DataSource.GetCathegories()));
                await arg.Channel.SendMessageAsync($"Cathegories:\n{cats}");
            }
            else if (content.StartsWith("!cnhelp"))
            {
                await arg.Channel.SendMessageAsync("Type !cnjoke for a joke by cathegory " +
                    "or !cncat for the list of cathegories.");
            }

        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
