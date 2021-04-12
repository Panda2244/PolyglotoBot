using Newtonsoft.Json;
using PolyglotoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PolyglotoBot.Services
{
    public class DefinitionService  
    {
        private HttpClient _httpClient;
        public DefinitionService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<List<string>> DefinitionAsync(string entry)
        {
            var response = await _httpClient.GetAsync($"/definition/?entry={entry}");

            response.EnsureSuccessStatusCode();

            var responsejson = await response.Content.ReadAsStringAsync();
            var definition = JsonConvert.DeserializeObject<WordDefinition>(responsejson);
            var listResults = definition.Meaning.Noun.Split("(nou)").ToList();
            listResults.Remove("");
            return listResults;
           

        }
    }
}
