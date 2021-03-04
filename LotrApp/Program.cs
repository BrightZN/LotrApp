using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LotrApp
{
    class Program
    {
        private static readonly HttpClient http = new HttpClient();

        static async Task Main(string[] args)
        {
            // url for characters: https://the-one-api.dev/v2/character
            try
            {
                var config = GetConfig();

                ApplyApiKey(config);

                var charactersApi = new GetLotrCharacters(http);

                var charactersResponse = await charactersApi.GetCharacterList();

                PrintCharacterList(charactersResponse);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        private static IConfigurationRoot GetConfig()
        {
            return new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
        }

        private static void ApplyApiKey(IConfigurationRoot config)
        {
            var lotrApiKey = config["LotrApiKey"];

            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {lotrApiKey}");
        }

        private static void PrintCharacterList(CharacterListResponse characterResp)
        {
            foreach (var character in characterResp.Characters.OrderBy(c => c.Name))
            {
                Console.WriteLine(character.Name);
            }
        }

        public class GetLotrCharacters 
        {
            private HttpClient http; 

            public GetLotrCharacters(HttpClient http)
            {
                this.http = http;
            }

            public async Task<CharacterListResponse> GetCharacterList()
            {
                var responseBody = await http.GetStringAsync("https://the-one-api.dev/v2/character");

                var characterResp = JsonConvert.DeserializeObject<CharacterListResponse>(responseBody);

                return characterResp;
            }
        }

        public class Character
        {
            [JsonProperty("_id")]
            public string Id { get; set; }

            [JsonProperty("height")]
            public string Height { get; set; }

            [JsonProperty("race")]
            public string Race { get; set; }

            [JsonProperty("gender")]
            public string Gender { get; set; }

            [JsonProperty("birth")]
            public string Birth { get; set; }

            [JsonProperty("spouse")]
            public string Spouse { get; set; }

            [JsonProperty("death")]
            public string Death { get; set; }

            [JsonProperty("realm")]
            public string Realm { get; set; }

            [JsonProperty("hair")]
            public string Hair { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("wikiUrl")]
            public string WikiUrl { get; set; }
        }

        public class CharacterListResponse
        {
            [JsonProperty("docs")]
            public List<Character> Characters { get; set; }

            [JsonProperty("total")]
            public int Total { get; set; }

            [JsonProperty("limit")]
            public int Limit { get; set; }

            [JsonProperty("offset")]
            public int Offset { get; set; }

            [JsonProperty("page")]
            public int Page { get; set; }

            [JsonProperty("pages")]
            public int Pages { get; set; }
        }
    }
}
