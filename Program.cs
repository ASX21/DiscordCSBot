using System;
using System.Threading.Tasks;
using System.IO;

namespace DiscordCSBot
{
    class Program
    {
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
            Console.WriteLine(token);
        }
    }
}
