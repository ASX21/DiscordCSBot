using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordCSBot
{
    class DataSource
    {
        private static string urlRand = $"https://api.chucknorris.io/jokes/random";
        private static string urlCat = $"https://api.chucknorris.io/jokes/random?category=";
        private static string urlCats = $"https://api.chucknorris.io/jokes/categories";

        public static async Task<string> RandomJoke()
        {
            string res = "";
            using (var client = new HttpClient())
            {
                var request = await client.GetAsync(urlRand);
                var json = request.Content.ReadAsStringAsync().Result;
                var datos = (JContainer)JsonConvert.DeserializeObject(json);
                res = (string)datos["value"];
            }
            return res;
        }
        public static async Task<List<string>> GetCathegories()
        {
            List<string> res = new List<string>();
            using (var client = new HttpClient())
            {
                var request = await client.GetAsync(urlCats);
                var json = request.Content.ReadAsStringAsync().Result;
                var datos = (JContainer)JsonConvert.DeserializeObject(json);
                foreach (var dato in datos.Values())
                {
                    res.Add((string)dato);
                }
            }
            return res;
        }
        public static async Task<string> CatJoke(string cat)
        {
            string res = "";
            using (var client = new HttpClient())
            {
                var request = await client.GetAsync(urlCat + cat);
                var json = request.Content.ReadAsStringAsync().Result;
                var datos = (JContainer)JsonConvert.DeserializeObject(json);
                res = (string)datos["value"];
            }
            return res;
        }
    }
}
