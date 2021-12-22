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
        /**
         * Variable that represents the bot's user
         */
        private DiscordSocketClient _client;

        /**
         * Application's entry point
         */
        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            /* 
             * Reading the discord connection token
             * From the file "APIKey.txt"
            */
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

            /*
             * Creating a new instance of the client
             */
            _client = new DiscordSocketClient(
                new DiscordSocketConfig { MessageCacheSize = 100 });

            /*
             * Adding event handling to
             * the Log event of the client.
             * The method shows the Log's message.
             */
            _client.Log += Log;

            /*
             * Login of the client to Discord 
             */
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            /*
             * Adding event handling to the
             * events of MessageReceived and
             * UserVoiceStateUpdated to check
             * when a new message has been sent
             * or a user has connected, disconnected
             * or moved from a voice channel.
             */
            _client.MessageReceived += _client_MessageReceived;
            _client.UserVoiceStateUpdated += _client_UserVoiceStateUpdated;

            await Task.Delay(-1);
        }

        /**
         * Handles the connection, disconnection
         * or change of a user's voice channel.
         */
        private Task _client_UserVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            /* 
             * Running the method's content
             * as a different Task to prevent
             * an event blocking another one.
             */
            Task.Run(async () => {
                // Checks if the user has disconnected or not
                if (arg3.VoiceChannel == null) return;
                SocketVoiceChannel vc = arg3.VoiceChannel;
                SocketVoiceChannel prevc = arg2.VoiceChannel;
                /* 
                 * If the previous channel has been created
                 * by the bot and is empty it'll be deleted.
                 */
                if (prevc != null) {
                        if (prevc.Name.ToLower().Contains("chuck-channel"))
                        {
                        if (prevc.Users.Count == 0)
                        {
                            await prevc.DeleteAsync();
                        }
                    }
                }
                /* 
                 * Checks if the channel the user has
                 * connected to has the right name.
                 */
                if (vc.Name != "Join to create a new VC") return;

                /*
                 * Assignment of the channel number
                 */
                int num = 1;
                foreach (var vchannel in vc.Guild.GetCategoryChannel(vc.Category.Id).Channels)
                {
                    if (vchannel.Name.ToLower().Contains("chuck-channel")) num++;
                }
                /* 
                 * Creation of the new Voice Channel
                 * in the cathegory selected
                 */
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

        /**
         * Handles the event of a
         * new message being sent.
         */
        private Task _client_MessageReceived(SocketMessage arg)
        {
            string content = arg.Content.ToLower();
            /*
             * Checks if the bot or another
             * bot is the author of the message.
             */
            if (arg.Author.IsBot ||
                (arg.Author.Id == _client.CurrentUser.Id)) return Task.CompletedTask;
            /*
             * If the message contains the word "chuck" or "norris",
             * it sends a random joke delivered from DataSource class.
             */
            if ((content.Contains("chuck") || content.Contains("norris")))
            {
                Task.Run(async () => {
                    var joke = await DataSource.RandomJoke();
                    await arg.Channel.SendMessageAsync("Did anyone say something about Chuck Norris?\n" +
                        $"{joke}");
                });
            }
            /*
             * Checks if the message starts with
             * !cnjoke, if so, checks if the format
             * and category of the joke is correct
             * and sends a joke as a response.
             */
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
            /*
             * Checks if the message
             * starts with !cncat and
             * if it does sends a list
             * of the joke categories.
             */
            else if (content.StartsWith("!cncat"))
            {
                Task.Run(async () => {
                    var cats = String.Join("\n- ", (await DataSource.GetCathegories()));
                    await arg.Channel.SendMessageAsync($"Cathegories:\n{cats}");
                });
            }
            /*
             * Checks if the message
             * starts with !cnhelp
             * and if it does sends
             * a message with info
             * about the commands.
             */
            else if (content.StartsWith("!cnhelp"))
            {
                Task.Run(async () => {
                    await arg.Channel.SendMessageAsync("Type !cnjoke for a joke by cathegory " +
                    "or !cncat for the list of cathegories.");
                });
            }
            return Task.CompletedTask;
        }

        /*
         * Method for showing the received Log's Message
         */
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
