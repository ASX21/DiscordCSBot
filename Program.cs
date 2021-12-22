using System;
using System.Threading.Tasks;
using System.IO;
using Discord.WebSocket;
using Discord;
using Discord.Rest;

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
            _client.UserVoiceStateUpdated += _client_UserVoiceStateUpdated;

            await Task.Delay(-1);
        }

        private Task _client_UserVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            Task.Run(async () => {
                if (arg3.VoiceChannel == null) return;
                SocketVoiceChannel vc = arg3.VoiceChannel;
                SocketVoiceChannel prevc = arg2.VoiceChannel;
                if (prevc != null) {
                        if (prevc.Name.ToLower().Contains("chuck-channel"))
                        {
                        if (prevc.Users.Count == 0)
                        {
                            await prevc.DeleteAsync();
                        }
                        return;
                    }
                }
                if (vc.Name != "Join to create a new VC") return;

                int num = 1;
                foreach (var vchannel in vc.Guild.GetCategoryChannel(vc.Category.Id).Channels)
                {
                    if (vchannel.Name.ToLower().Contains("chuck-channel")) num++;
                }
                RestVoiceChannel vcFin = await vc.Guild.CreateVoiceChannelAsync($"Chuck-channel #{num}", pr => pr.CategoryId = vc.CategoryId);
                if (vc.Users.Count > 0)
                {
                    var users = vc.Users.GetEnumerator();
                    while (users.MoveNext())
                    {
                        await users.Current.ModifyAsync(user => user.ChannelId = vcFin.Id);
                    }
                }
            });
            return Task.CompletedTask;
        }

        private Task _client_MessageReceived(SocketMessage arg)
        {
            string content = arg.Content.ToLower();
            if (arg.Author.IsBot ||
                (arg.Author.Id == _client.CurrentUser.Id)) return Task.CompletedTask;
            if ((content.Contains("chuck") || content.Contains("norris")))
            {
                Task.Run(async () => {
                    var joke = await DataSource.RandomJoke();
                    await arg.Channel.SendMessageAsync("Did anyone say something about Chuck Norris?\n" +
                        $"{joke}");
                });
            }
            else if (content.StartsWith("!cnjoke"))
            {
                var args = content.Split(' ');
                if (args.Length == 2)
                {
                    Task.Run(async () => {
                        try
                        {
                                var joke = await DataSource.CatJoke(args[1]);
                                await arg.Channel.SendMessageAsync(joke);
                        }
                        catch (Exception ex)
                        {
                                await arg.Channel.SendMessageAsync("Something doesn't seem right");
                        }
                     
                    });
                }
            }
            else if (content.StartsWith("!cncat"))
            {
                Task.Run(async () => {
                    var cats = String.Join("\n- ", (await DataSource.GetCathegories()));
                    await arg.Channel.SendMessageAsync($"Cathegories:\n{cats}");
                });
            }
            else if (content.StartsWith("!cnhelp"))
            {
                Task.Run(async () => {
                    await arg.Channel.SendMessageAsync("Type !cnjoke for a joke by cathegory " +
                    "or !cncat for the list of cathegories.");
                });
            }
            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
